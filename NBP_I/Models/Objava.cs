using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    public class Objava
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Sadrzaj { get; set; }
        public string Tag { get; set; } // da probamo tagove umesto kategorije da koristimo
        public DateTime DatumKreiranja { get; set; }
        public int idAutora { get; set; }

        //public List<Komentar> Comments { get; set; } // mislim da ne treba ovde da se dodaju komentari nego tu ce da bude grupni chat ali mozda i gresim, videcemo
        public List<int> Likes { get; set; } // mislim da je to lista ID-ijeva ljudi koji su lajkovali a ne mora da se pamti ceo tip Korisnika

        public double Rejting { get; set; }
    }
}
