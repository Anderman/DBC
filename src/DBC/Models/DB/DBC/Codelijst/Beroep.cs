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
    public class Beroep : Codelijst.CodeTable
    {
        public int BeroepID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), 
        [Column(TypeName = "varchar"), StringLength(20)]
        new public String Code { get; set; }
        [Column(TypeName = "varchar"), StringLength(10)]
        public String groepcode { get; set; }
        public String element { get; set; }
        public String beschrijving { get; set; }
        public int hierarchieniveau { get; set; }
        public int selecteerbaar { get; set; }
        public int sorteervolgorde { get; set; }
        public int? mutatie { get; set; }
        //[Index("IX_Updatekey", 2, IsUnique = true)]
        public int branche_indicatie { get; set; }
        public new static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Beroep>(b =>
            {
                b.Property(c => c.Code).HasColumnType("varchar").MaxLength(20);
                b.Property(c => c.groepcode).HasColumnType("varchar").MaxLength(10);
                b.Index(p => new { p.Code, p.branche_indicatie }).Unique(true);
            });
        }
    }
    public sealed class BeroepMap : CsvClassMap<Beroep>
    {
        public BeroepMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.groepcode).Index(3);
            Map(m => m.element).Index(4);
            Map(m => m.beschrijving).Index(5);
            Map(m => m.hierarchieniveau).Index(6);
            Map(m => m.selecteerbaar).Index(7);
            Map(m => m.sorteervolgorde).Index(8);
            Map(m => m.mutatie).Index(9);
            Map(m => m.branche_indicatie).Index(10);
        }
    }
}