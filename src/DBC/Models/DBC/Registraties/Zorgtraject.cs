using System;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using DBC.Models.DB.DBC.Codelijst;

namespace DBC.Models.DB.DBC.Registraties
{
    public class Zorgtraject
    {
        public int ZorgtrajectID { get; set; }
        public virtual Patient Patient { get; set; }
        public int InstellingVolgnr { get; set; }
        public virtual Department Zorgafdeling { get; set; }
        public DateTime? Begindatum { get; set; }
        public DateTime? Einddatum { get; set; }
        public virtual Diagnose PrimaireDiagnose { get; set; }
        public DateTime? PrimaireDiagnoseDatum { get; set; }
        public string PrimaireDiagnoseTrekkenVan { get; set; }
        //DIS related
        //public virtual COD016 SoortVerwijzer { get; set; }
        //public virtual Diagnose PrimaireDiagnoseCode { get; set; }
        public string VerwijzendeInstelling { get; set; }
        public string StatusVlag { get; set; }
        public string SBGLocatiecode { get; set; }
        //public virtual Zorgdomein Zorgdomeincode { get; set; }
        public string Verwijsdatum { get; set; }
        //Import specific
        //[Index]
        public int ZorgtrajectIDExtern { get; set; }
        //[Index]
        public int PatientIDExtern { get; set; }
        public string PrimaireDiagnoseCodeExtern { get; set; }

    }
    //IDENTIFICATIENUMMER|BEGINDATUM|EINDDATUM|PATIENT_IDENTIFICATIENUMMER|CL_DIAGNOSE_CODE|PRIMAIREDIAGNOSE_DATUM|AS2_TREKKENVAN
    public sealed class ZorgtrajectMapTestSetDBCOnderhoud : CsvClassMap<Zorgtraject>
    {
        public ZorgtrajectMapTestSetDBCOnderhoud()
        {
            Map(m => m.ZorgtrajectIDExtern).Index(0);
            Map(m => m.Begindatum).ConvertUsing(row => (row.GetField(1).Any() ? DateTime.ParseExact(row.GetField(1), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null)); ;
            Map(m => m.Einddatum).ConvertUsing(row => (row.GetField(2).Any() ? DateTime.ParseExact(row.GetField(2), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null)); ;
            Map(m => m.PatientIDExtern).Index(3);
            Map(m => m.PrimaireDiagnoseCodeExtern).Index(4);
            Map(m => m.PrimaireDiagnoseDatum).ConvertUsing(row => (row.GetField(5).Any() ? DateTime.ParseExact(row.GetField(5), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null)); ;
            Map(m => m.PrimaireDiagnoseTrekkenVan).Index(6);
        }
    }
}