using System;

namespace DBC.Models.DB.DBC.Registraties
{
    public class Verblijfsdag
    {
        public int VerblijfsdagID { get; set; }
        public virtual DBCs DBC { get; set; }
        public string Activiteitnummer { get; set; }
        public string ActiviteitCode { get; set; }
        public DateTime Begindatum { get; set; }
        public DateTime Einddatum { get; set; }
        public int AantalDagen { get; set; }
        public int Tarief { get; set; }
        public string UitvoerendeInstelling { get; set; }
        public int Postwijk { get; set; }
        //Import specific
        //[Index]
        public int DBCIDExtern { get; set; }
        //[Index]
        public int VerblijfsdagIDExtern { get; set; }
    }
}