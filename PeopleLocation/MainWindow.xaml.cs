using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

        public List<Person> person { get; set; }
        public MainWindow()
        {
            InitializeComponent();
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

                Dispatcher.Invoke(() =>
                {
                    isFirst = false;
                    DrawSetFrontView();
                    stopTime = 3000;
                });
            }
        }

        private void DrawSetFrontView()
        {
            GetData();
            var EllipsDoctor = new Ellipse()
            {
                Width = 5,
                Height = 5,
                StrokeThickness = 2,
                Fill = Brushes.Blue
            }; 
            var EllipsClient = new Ellipse()
            {
                Width = 5,
                Height = 5,
                StrokeThickness = 2,
                Fill = Brushes.Green
            };
           foreach(var p in person)
            {
                 var c = grid.FindResource("canvas" + p.LastSecurityPointNumber) as Canvas;
                 if (p.personRole == "сотрудник")
                 {
                     c.Children.Add(EllipsDoctor);
                     SetPosition(c);
                 }
                 else
                 {
                     c.Children.Add(EllipsClient);
                     SetPosition(c);
                 }
            }
        }

        private static void SetPosition(Canvas? c)
        {
            Canvas.SetTop(c, c.ActualHeight - 10);
            Canvas.SetBottom(c, 0);
            Canvas.SetLeft(c, c.ActualWidth - 10);
            Canvas.SetRight(c, 0);
        }

        private async void GetData()
        {
            try
            {
                person = new List<Person>(await Conn.Inst().GetFromJsonAsync<List<Person>>(""));
            } catch(Exception e)
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