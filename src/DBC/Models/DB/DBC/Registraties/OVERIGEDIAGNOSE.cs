using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class OverigeDiagnose
    {
        public int OverigeDiagnoseID { get; set; }
        public virtual DBCs DBC { get; set; }
        public string DiagnoseCode { get; set; }
        public DateTime Datum { get; set; }
        public string DiagnoseAS2Trekkenvan { get; set; }
        //Import specific
        //[Index]
        public int DBCIDExtern { get; set; }
    }
    //DBC_IDENTIFICATIENUMMER|CL_DIAGNOSE_CODE|DIAGNOSE_DATUM_DIAGNOSE|DIAGNOSE_AS2_TREKKENVAN

    public sealed class OverigeDiagnoseMapTestDBCOnderhoud : CsvClassMap<OverigeDiagnose>
    {
        public OverigeDiagnoseMapTestDBCOnderhoud()
        {
            Map(m => m.DBCIDExtern).Index(0);
            Map(m => m.DiagnoseCode).Index(1);
            Map(m => m.Datum).Index(2).ConvertUsing(row => (row.GetField(2).Any() ? DateTime.ParseExact(row.GetField(2), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null));
            Map(m => m.DiagnoseAS2Trekkenvan).ConvertUsing(row => row.GetField(3) == "NULL" || row.GetField(3) == "" ? null : row.GetField(3));
        }
    }
}