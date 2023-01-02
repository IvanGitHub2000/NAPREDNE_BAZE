using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    public class Korisnik
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Sifra { get; set; }
        //public string Name { get; set; }
        //public string Email { get; set; }
        public int GodinaStudije { get; set; }
        public int Rejting { get; set; }

        public string Smer { get; set; }
        //public List<string> Tagovi { get; set; }//na osnovu taga se vidi koje sobe prati
        //public List<string> Courses { get; set; }
        public List<Post> Posts { get; set; }
        public List<Soba> SubscribedRooms { get; set; }
        public bool IsAdmin { get; set; }// za ovo vidi
        public List<Poruka> Poruke { get; set; }

        //public List<int> Pratioci { get; set; }
    }
}
