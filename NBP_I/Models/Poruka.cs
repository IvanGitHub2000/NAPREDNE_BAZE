using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    public class Poruka
    {
        public string Sadrzaj { get; set; }//za ovo da vidimo moze da bude i Image i svasta nesto
        public Korisnik Autor { get; set; }
        public DateTime DatumKreiranja { get; set; }

        public Soba Soba { get; set; }
    }
}
