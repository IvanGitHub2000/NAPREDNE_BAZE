using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    public class Post
    {
        public string Naziv { get; set; }
        public string Sadrzaj { get; set; }
        //public List<string> Tagovi { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public Korisnik Autor { get; set; }
        public List<Komentar> Comments { get; set; }
        public List<Korisnik> Likes { get; set; }
    }
}
