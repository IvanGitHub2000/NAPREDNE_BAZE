using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBP_I.Models
{
    //public record Konverzacija(string Kljuc, string NazivObjave = "")
    //{
    //    public static Poruka[] VratiPoruke(string id) => RedisManager<Poruka>.GetAll($"poruke:{id}");
    //    //public static void SaljiPoruku(string id, Poruka msg) => RedisManager<Poruka>.Push($"poruke:{id}", msg);
    //    public static void SaljiPoruku(string kljuc, Poruka poruka)
    //    {
    //        RedisManager<Poruka>.Push($"poruke:{kljuc}", poruka);
    //    }
    //}

    public class Konverzacija
    {
        public string Id;
        public string NazivObjave;
        public Konverzacija(string id, string nazivObjave)
        {
            this.Id = id;
            this.NazivObjave = nazivObjave;
        }

        public Poruka[] VratiPoruke() => RedisManager<Poruka>.GetAll($"poruke:{Id}");
        //public static void SaljiPoruku(string id, Poruka msg) => RedisManager<Poruka>.Push($"poruke:{id}", msg);
        public void SaljiPoruku(Poruka poruka)
        {
            RedisManager<Poruka>.Push($"poruke:{Id}", poruka);
        }
    }
}
