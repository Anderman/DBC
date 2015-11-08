using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Data.Entity;

namespace DBC.Models.DB.DBC.Codelijst
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
        public new static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ActiviteitAndTarief>(b =>
            {
                b.Property(c => c.declaratiecode).HasColumnType("char").HasMaxLength(6);
                b.Property(c => c.declaratiecode_kleur).HasColumnType("char").HasMaxLength(5);


                b.Index(p => p.Code).Unique(true);
            });
        }
    }
    public sealed class ActiviteitAndTariefMap : CsvHelper.Configuration.CsvClassMap<ActiviteitAndTarief>
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