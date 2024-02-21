using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PeopleLocation.Model.Tools
{
    public static class Conn
    {
        static HttpClient client {  get; set; }
        public static HttpClient Inst()
        {
            if(client == null)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:7095/api/PersonLocations");
            }

            return client;
        }
    }
}
