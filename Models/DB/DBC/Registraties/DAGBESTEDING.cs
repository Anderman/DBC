using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Dagbesteding
    {
        public int DagbestedingID { get; set; }
        public virtual DBCs DBC { get; set; }
        public string Activiteitnummer { get; set; }
        public string ActiviteitCode { get; set; }
        public DateTime Datum { get; set; }
        public int AantalUur { get; set; }
        public int Tarief { get; set; }
        public string UitvoerendeInstelling { get; set; }
        public int Postwijk { get; set; }
        //Import specific
        //[Index]
        public int DBCIDExtern { get; set; }
        //[Index]
        public int DagbestedingExtern { get; set; }

    }
}