using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using System.Globalization;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Hoofdberoep : Codelijst.CodeTable
    {
        public int HoofdberoepID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), Column(TypeName = "varchar"), StringLength(20)]
        new public String Code { get; set; }
        public String beschrijving { get; set; }
        //[Index("IX_Updatekey", 2, IsUnique = true)]
        public int branche_indicatie { get; set; }
    }
    public sealed class HoofdberoepMap : CsvClassMap<Hoofdberoep>
    {
        public HoofdberoepMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.beschrijving).Index(3);
            Map(m => m.branche_indicatie).Index(4);
        }
    }
}