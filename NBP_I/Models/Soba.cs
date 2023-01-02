using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    public class Soba
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }

        public string Tag { get; set; }
        //public List<string> Interesovanja { get; set; }
        public List<Poruka> Poruke { get; set; }
        public List<Korisnik> Korisnici { get; set; }
    }
}
