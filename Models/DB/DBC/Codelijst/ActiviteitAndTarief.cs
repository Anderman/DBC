using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Web;

namespace GGZDBC.Models.DBCModel.Afleiding
{
    public class ActiviteitAndTarief : Codelijst.CodeTable
    {
        new public String Code { get; set; }
        public int ActiviteitAndTariefID { get; set; }
        public String tarief_omschrijving { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), 
        [Column(TypeName = "char"), StringLength(6)]
        public String declaratiecode { get; set; }
        [Column(TypeName = "char"), StringLength(5)]
        public String declaratiecode_kleur { get; set; }
        public int tarief_basis { get; set; }
        public int tarief_max { get; set; }
        public int tarief_nhc { get; set; }
        public int? mutatie { get; set; }
    }
    public sealed class ActiviteitAndTariefMap : CsvClassMap<ActiviteitAndTarief>
    {
        public ActiviteitAndTariefMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.tarief_omschrijving).Index(3);
            Map(m => m.declaratiecode).Index(4);
            Map(m => m.declaratiecode_kleur).Index(5);
            Map(m => m.tarief_basis).Index(6);
            Map(m => m.tarief_max).Index(7);
            Map(m => m.tarief_nhc).Index(8);
            Map(m => m.mutatie).Index(9);
        }
    }

}