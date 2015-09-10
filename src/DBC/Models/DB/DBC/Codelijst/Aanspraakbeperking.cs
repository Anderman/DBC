//code|omschrijving|aanvullende_informatie|begindatum|einddatum|mutatie
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using Microsoft.Data.Entity;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Aanspraakbeperking : Codelijst.CodeTable
    {
        public int AanspraakbeperkingID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true)]
        [StringLength(3)]
        [Column(TypeName = "char")]
        new public string Code { get; set; }
        public string omschrijving { get; set; }
        public string aanvullende_informatie { get; set; }
        public int? mutatie { get; set; }
        public new static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Aanspraakbeperking>(b =>
            {
                b.Property(c => c.Code).HasColumnType("char").MaxLength(3);
                b.Index(p => p.Code).Unique(true);
            });
        }
    }
    public sealed class AanspraakbeperkingMap : CsvHelper.Configuration.CsvClassMap<Aanspraakbeperking>
    {
        public AanspraakbeperkingMap()
        {
            Map(m => m.Code).Index(0);
            Map(m => m.omschrijving).Index(1);
            Map(m => m.aanvullende_informatie).Index(2);
            Map(m => m.begindatum).Index(3).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(4).TypeConverterOption("yyyyMMdd");
            Map(m => m.mutatie).Index(5);
        }
    }
}