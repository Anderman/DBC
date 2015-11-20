//agb_code|cl_zorgtype_prestatiecodedeel|cl_diagnose_prestatiecodedeel|zvz|cl_productgroep_code|cl_dbc_prestatiecode|cl_declaratiecode|begindatum|einddatum|mutatie

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using Microsoft.Data.Entity;

namespace DBC.Models.DB.DBC.Codelijst
{
    public class Prestatiecode : Codelijst.CodeTable
    {
        public int PrestatiecodeID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), Column(TypeName = "varchar"), StringLength(12)]
        new public String Code { get; set; }
        //[Index("IX_Updatekey", 2, IsUnique = true), Column(TypeName = "varchar"), StringLength(6)]
        public String cl_declaratiecode { get; set; }
        [Column(TypeName = "char"), StringLength(4)]
        public String agb_code { get; set; }
        [Column(TypeName = "char"), StringLength(3)]
        public String cl_zorgtype_prestatiecodedeel { get; set; }
        [Column(TypeName = "varchar"), StringLength(3)]
        public String cl_diagnose_prestatiecodedeel { get; set; }
        public int? zvz { get; set; }
        [Column(TypeName = "varchar"), StringLength(6)]
        public String cl_productgroep_code { get; set; }
        public int? mutatie { get; set; }
        public new static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Prestatiecode>(b =>
            {
                b.Property(c => c.Code).HasColumnType("varchar").HasMaxLength(12);
                b.Property(c => c.cl_declaratiecode).HasColumnType("varchar").HasMaxLength(6);
                b.Property(c => c.agb_code).HasColumnType("char").HasMaxLength(4);
                b.Property(c => c.cl_zorgtype_prestatiecodedeel).HasColumnType("char").HasMaxLength(3);
                b.Property(c => c.cl_diagnose_prestatiecodedeel).HasColumnType("varchar").HasMaxLength(3);
                b.Property(c => c.cl_productgroep_code).HasColumnType("varchar").HasMaxLength(6);
                b.HasIndex(p => new { p.Code, p.cl_declaratiecode }).IsUnique(true);
            });
        }
    }
    public sealed class PrestatiecodeMap : CsvClassMap<Prestatiecode>
    {
        public PrestatiecodeMap()
        {
            Map(m => m.agb_code).Index(0);
            Map(m => m.cl_zorgtype_prestatiecodedeel).Index(1);
            Map(m => m.cl_diagnose_prestatiecodedeel).Index(2);
            Map(m => m.zvz).Index(3);
            Map(m => m.cl_productgroep_code).Index(4);
            Map(m => m.Code).Index(5);
            Map(m => m.cl_declaratiecode).Index(6);
            Map(m => m.begindatum).Index(7).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(8).TypeConverterOption("yyyyMMdd");
            Map(m => m.mutatie).Index(9);
        }
    }
}