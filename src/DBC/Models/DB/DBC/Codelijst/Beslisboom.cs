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
    public class Beslisboom 
    {
        [Key, Column(Order = 0)]
        public int knoopNummer { get; set; }
        public String kenmerkendeFactorCode { get; set; }
        public String parameter1 { get; set; }
        public String parameter2 { get; set; }
        public String op { get; set; }
        public int? waarde1 { get; set; }
        public int? waarde2 { get; set; }
        public int? knoopDoelTrue { get; set; }
        public int onthoudenDoelTrue { get; set; }
        public int? knoopDoelFalse { get; set; }
        public int onthoudenDoelFalse { get; set; }
        [Key, Column(Order = 1)]
        public DateTime Begindate { get; set; }
        public DateTime Enddate { get; set; }

        public new static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Beslisboom>(b =>
            {
                b.Key(p => new { p.knoopNummer, p.Begindate });
            });
        }
    }
    //knoop_nummer|kenmerkende_factor_code|parameter_1|parameter_2|operator|waarde_1|waarde_2|knoop_doel_true|onthouden_doel_true|knoop_doel_false|onthouden_doel_false
    public sealed class BeslisboomMap : CsvClassMap<Beslisboom>
    {
        public BeslisboomMap()
        {
            Map(m => m.knoopNummer).Index(0);
            Map(m => m.kenmerkendeFactorCode).Index(1);
            Map(m => m.parameter1).Index(2);
            Map(m => m.parameter2).Index(3);
            Map(m => m.op).Index(4);
            Map(m => m.waarde1).Index(5);
            Map(m => m.waarde2).Index(6);
            Map(m => m.knoopDoelTrue).Index(7);
            Map(m => m.onthoudenDoelTrue).Index(8);
            Map(m => m.knoopDoelFalse).Index(9);
            Map(m => m.onthoudenDoelFalse).Index(10);
            Map(m => m.Begindate).Index(11).ConvertUsing(row => (row.GetField(11).Any() ? DateTime.ParseExact(row.GetField(11), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null)); ;
            Map(m => m.Enddate).Index(12).ConvertUsing(row => (row.GetField(12).Any() ? DateTime.ParseExact(row.GetField(12), "yyyyMMdd", CultureInfo.InvariantCulture) as DateTime? : null)); ;
        }
    }
}