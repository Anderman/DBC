using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using DBC.Models.DB.DBC.Codelijst;

namespace DBC.Models.DB.DBC.Registraties
{
    public partial class DBCs
    {
        public int DBCID { get; set; }
        public virtual Zorgtraject Zorgtraject { get; set; }
        public DateTime Begindatum { get; set; }
        public DateTime Einddatum { get; set; }
        public int BehandelaarID { get; set; }
        public virtual Zorgtype Zorgtype { get; set; }
        public string CirquitCode { get; set; }
        public string RedensluitenCode { get; set; }
        public virtual ICollection<Tijdschrijven> Tijdschrijven { get; set; }
        public virtual ICollection<Verblijfsdag> Verblijfsdagen { get; set; }
        public virtual ICollection<Dagbesteding> Dagbestedingen { get; set; }
        //DIS Specific
        public int BehandelaarDiagnoseID { get; set; }
        public string BehandelaarDiagnoseBeroep { get; set; }
        public int BehandelaarBehandelingID { get; set; }
        public string BehandelaarBehandelingBeroep { get; set; }
        public virtual Productgroep Productgroep { get; set; }
        public virtual Prestatiecode Prestatiecode { get; set; }
        //Import specific
        //[Index]
        public int DBCIDExtern { get; set; }
        //[Index]
        public int ZorgtrajectIDExtern { get; set; }
        public string ZorgtypeCodeExtern { get; set; }
        public string PrestatieCodeExtern { get; set; }
    }

    public sealed class DBCMapTestDBCOnderhoud : CsvClassMap<DBCs>
    {
        public DBCMapTestDBCOnderhoud()
        {
            Map(m => m.DBCIDExtern).Index(0);
            Map(m => m.ZorgtrajectIDExtern).Index(1);
            Map(m => m.Begindatum).ConvertUsing(row => (row.GetField(2).Any() ? DateTime.ParseExact(row.GetField(2), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null));
            Map(m => m.Einddatum).ConvertUsing(row => (row.GetField(3).Any() ? DateTime.ParseExact(row.GetField(3), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null));
            Map(m => m.ZorgtypeCodeExtern).Index(4);
            Map(m => m.CirquitCode).Index(5);
            Map(m => m.RedensluitenCode).Index(6);
            Map(m => m.BehandelaarID).Index(7);
        }
    }

}