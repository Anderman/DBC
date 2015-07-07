using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Tijdschrijven
    {
        public int TijdschrijvenID { get; set; }
        public virtual DBCs DBC { get; set; }
        public string ActiviteitCode { get; set; }
        public DateTime Datum { get; set; }
        public int DirecteMinuten { get; set; }
        public int IndirecteMinutenReis { get; set; }
        public int IndirecteMinutenAlgemeen { get; set; }
        public int BehandelaarID { get; set; }
        public string BehandelaarBeroepCode { get; set; }
        //Import specific
        //[Index]
        public int DBCIDExtern { get; set; }
        //[Index]
        public int TijdschrijvenIDExtern { get; set; }
    }
    //IDENTIFICATIENUMMER|DBC_IDENTIFICATIENUMMER|CL_ACTIVITEIT_CODE|ACTIVITEITENDATUM|DIRECTE_MINUTEN|INDIRECTE_MINUTEN_REIS|INDIRECTE_MINUTEN_ALG|BEHANDELAAR_IDENTIFICATIENUMMER|CL_BEROEP_CODE
    public sealed class TijdschrijvenMapTestDBCOnderhoud : CsvClassMap<Tijdschrijven>
    {
        public TijdschrijvenMapTestDBCOnderhoud()
        {
            Map(m => m.TijdschrijvenIDExtern).Index(0);
            Map(m => m.DBCIDExtern).Index(1);
            Map(m => m.ActiviteitCode).Index(2);
            Map(m => m.Datum).Index(3).ConvertUsing(row => (row.GetField(3).Any() ? DateTime.ParseExact(row.GetField(3), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null));
            Map(m => m.DirecteMinuten).Index(4);
            Map(m => m.IndirecteMinutenReis).Index(5);
            Map(m => m.IndirecteMinutenAlgemeen).Index(6);
            Map(m => m.BehandelaarID).Index(7);
            Map(m => m.BehandelaarBeroepCode).Index(8);
        }
    }
}