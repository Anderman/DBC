using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.Data.Entity;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Diagnose : Codelijst.CodeTable
    {
        public int DiagnoseID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), Column(TypeName = "varchar"), StringLength(20)]
        new public String Code { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String groepcode { get; set; }
        public String element { get; set; }
        public String beschrijving { get; set; }
        public int? zvz_subscore { get; set; }
        public int? aanspraak_type { get; set; }
        public int hierarchieniveau { get; set; }
        public int selecteerbaar { get; set; }
        public int sorteervolgorde { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String Diagnose_as { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String refcode_icd9cm { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String refcode_icd10 { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String prestatieniveau { get; set; }
        public String prestatiecode_naamgeving_ggz { get; set; }
        public String prestatiecode_naamgeving_fz { get; set; }
        public String prestatiecodedeel_ggz { get; set; }
        public String prestatiecodedeel_fz { get; set; }
        public int? mutatie { get; set; }
        //[Index("IX_Updatekey", 2, IsUnique = true)]
        public int branche_indicatie { get; set; }
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Diagnose>(b =>
            {
                b.Property(c => c.groepcode).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.Diagnose_as).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.refcode_icd9cm).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.refcode_icd10).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.prestatieniveau).ColumnType("varchar").MaxLength(20);
                b.Index(p => p.Code).Unique(true);
            });
        }
    }
    public sealed class DiagnoseMap : CsvClassMap<Diagnose>
    {
        public DiagnoseMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.groepcode).Index(3);
            Map(m => m.element).Index(4);
            Map(m => m.beschrijving).Index(5);
            Map(m => m.zvz_subscore).Index(6);
            Map(m => m.aanspraak_type).Index(7);
            Map(m => m.hierarchieniveau).Index(8);
            Map(m => m.selecteerbaar).Index(9);
            Map(m => m.sorteervolgorde).Index(10);
            Map(m => m.Diagnose_as).Index(11);
            Map(m => m.refcode_icd9cm).Index(12);
            Map(m => m.refcode_icd10).Index(13);
            Map(m => m.prestatieniveau).Index(14);
            Map(m => m.prestatiecode_naamgeving_ggz).Index(15);
            Map(m => m.prestatiecode_naamgeving_fz).Index(16);
            Map(m => m.prestatiecodedeel_ggz).Index(17);
            Map(m => m.prestatiecodedeel_fz).Index(18);
            Map(m => m.mutatie).Index(19);
            Map(m => m.branche_indicatie).Index(20);
        }
    }
}