using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GGZDBC.Models.DBCModel.Codelijst
{
    public class CodeTable
    {
        //[Index("IX_Updatekey", 0, IsUnique = true)]
        public DateTime begindatum { get; set; }
        public DateTime einddatum { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String Code { get; set; }
        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CodeTable>(b =>
            {
                b.Property(c => c.Code).HasColumnType("varchar").MaxLength(20);
                b.Index(p => p.Code).Unique(true);
            });
        }
    }
}