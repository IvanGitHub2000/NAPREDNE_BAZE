using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    public class Komentar
    {
        public string Sadrzaj { get; set; }
        public Korisnik Kreator { get; set; }
        public DateTime DatumKreiranja { get; set; }

        public Objava Post { get; set; }

        public List<Korisnik> Likes { get; set; }
    }
}
