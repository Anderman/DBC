using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;

namespace GGZDBC.Models.DBCModel.Testset
{
    public class DBCTestset 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None )]
        public int DBCIDExtern3 { get; set; }
        public int ExpectedProductgroup { get; set; }
    }
    //knoop_nummer|kenmerkende_factor_code|parameter_1|parameter_2|operator|waarde_1|waarde_2|knoop_doel_true|onthouden_doel_true|knoop_doel_false|onthouden_doel_false
    public sealed class DBCTestsetMap : CsvClassMap<DBCTestset>
    {
        public DBCTestsetMap()
        {
            Map(m => m.DBCIDExtern3).Index(0);
            Map(m => m.ExpectedProductgroup).Index(1);
        }
    }
}