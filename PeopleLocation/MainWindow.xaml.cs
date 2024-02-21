using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PeopleLocation.Model.Tools;
using WebApplicationSessia2;

namespace PeopleLocation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        List<Canvas> canvasList = new List<Canvas>();

        public List<Person> person { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();

            canvasList.Add(grid.FindName("canvas00") as Canvas);
            for (int i = 0; i < 23; i++)
                canvasList.Add(grid.FindName("canvas" + i) as Canvas);
            DataContext = this;
            DrawLocation();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void Signal([CallerMemberName] string prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        private void DrawLocation()
        {
            DrawSetFrontView();
            var thread = new Thread(Move);
            thread.Start();
        }

        private void Move()
        {
            var random = new Random();
            int stopTime = 10;
            bool isFirst = true;
            while (isFirst)
            {
                Thread.Sleep(stopTime);

                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        //isFirst = false;
                        DrawSetFrontView();
                        stopTime = 3000;
                    });
                }
                catch (Exception ex) { }
            }
        }

        private void DrawSetFrontView()
        {
            GetData();
            Canvas c = null;
            for (int i = 0; i < canvasList.Count; i++)
            {
                canvasList[i].Children.Clear();
            }

            foreach (var p in person)
            {
                bool find = false;
                for (int i = 0; i < canvasList.Count; i++)
                {
                    find = false;
                   
                    c = canvasList[i];

                    if (c != null && c.Name == "canvas" + p.LastSecurityPointNumber && p.LastSecurityPointDirection == "in")
                    {
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    c = canvas00;
                }
                if (p.personRole == "сотрудник")
                {
                    var EllipsDoctor = new Ellipse()
                    {
                        Width = 20,
                        Height = 20,
                        StrokeThickness = 2,
                        Fill = Brushes.Blue
                    };
                    c.Children.Add(EllipsDoctor);
                    SetPosition(c, EllipsDoctor);
                }
                else if (p.personRole == "клиент")
                {
                    var EllipsClient = new Ellipse()
                    {
                        Width = 20,
                        Height = 20,
                        StrokeThickness = 2,
                        Fill = Brushes.Green
                    };
                    c.Children.Add(EllipsClient);
                    SetPosition(c, EllipsClient);
                }
            }
        }

        static Random Random = new Random();
        private static void SetPosition(Canvas? c, Ellipse ellipse)
        {
            SetRndPosition(c, ellipse);
            Ellipse e;
            int x1 = (int)Canvas.GetLeft(ellipse) + 10;
            int y1 = (int)Canvas.GetTop(ellipse) + 10;
            for (int i = 0; i < c.Children.Count; i++)
            {
                e = c.Children[i] as Ellipse;
                if (e != ellipse)
                {
                    int x2 = (int)Canvas.GetLeft(e) + 10;
                    int y2 = (int)Canvas.GetTop(e) + 10;

                    if (Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2)) < 20)
                    {
                        SetPosition(c, ellipse);
                        break;
                    }
                }
            }
        }

        private static void SetRndPosition(Canvas? c, Ellipse ellipse)
        {
            Canvas.SetTop(ellipse, Random.Next(0, (int)c.ActualHeight - 20));
            Canvas.SetLeft(ellipse, Random.Next(0, (int)c.ActualWidth - 20));
        }

        private async void GetData()
        {
            try
            {
                person = new List<Person>(await Conn.Inst().GetFromJsonAsync<List<Person>>(""));
            }
            catch (Exception e)
            {
                Message(e);
            }
        }

        private static void Message(Exception e)
        {
            MessageBox.Show(
                e.Message,
                "Mistake",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning
                );
        }
    }
}