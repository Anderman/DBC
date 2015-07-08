//begindatum|einddatum|code|groepcode|element|beschrijving|aanspraak_type|hierarchieniveau|selecteerbaar|sorteervolgorde|soort|mag_direct|mag_indirect|mag_reistijd|mag_groep|mutatie|branche_indicatie
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Globalization;
using DBC.Models.DB;
using Microsoft.Data.Entity;

namespace GGZDBC.Models.DBCModel.Afleiding
{
    public class Activiteit : Codelijst.CodeTable
    {
        public int ActiviteitID { get; set; }
        //[Index("IX_Updatekey", 1, IsUnique = true), 
        [Column(TypeName = "varchar"), StringLength(20)]
        new public String Code { get; set; }

        [Column(TypeName = "varchar"), StringLength(20)]
        public String groepcode { get; set; }

        public String element { get; set; }
        public String beschrijving { get; set; }

        [Column(TypeName = "char")]
        [StringLength(1)]
        public String aanspraak_type { get; set; }

        public int hierarchieniveau { get; set; }
        public int selecteerbaar { get; set; }
        public int sorteervolgorde { get; set; }
        [Column(TypeName = "varchar"), StringLength(20)]
        public String soort { get; set; }

        [StringLength(1)]
        [Column(TypeName = "char")]
        public String mag_direct { get; set; }

        [StringLength(1)]
        [Column(TypeName = "char")]
        public String mag_indirect { get; set; }

        [StringLength(1)]
        [Column(TypeName = "char")]
        public String mag_reistijd { get; set; }

        [StringLength(1)]
        [Column(TypeName = "char")]
        public String mag_groep { get; set; }
        public int? mutatie { get; set; }

        //[Index("IX_Updatekey", 2, IsUnique = true)]
        public int branche_indicatie { get; set; }

        //Verblijfsdagen met overnachting zijn activiteiten uit codelijst CL_Activiteit waarvoor 
        //de kolom CL_Activiteit_Soort de waarde 'Verblijfsdag' bevat en 
        //de kolom CL_Activiteit_Code de waarde 'Act_8.%.1 t/m 'Act_8.%.5' bevat.  
        //Uitzondering: verblijf zonder overnachting dienen niet in deze calculatie te worden meegenomen.  
        //Verblijf zonder overnachting is een activiteit uit codelijst CL_Activiteit waarvoor 
        //de kolom CL_Activiteit_Soort de waarde 'Verblijfsdag' bevat en 
        //de kolom CL_Activiteit_Code de waarde 'Act_8.%.6' bevat.
        //public IQueryable<string> verblijsdagenMetOvernachting(ApplicationContext context)
        //{
        //    return from a in context.Activiteit where a.soort == "Verblijfsdag" && SqlMethods.Like(a.Code, "Act_8.%.[12345]%") select a.Code;
        //}
        //Tijdschrijf activiteiten zijn activiteiten uit codelijst CL_Activiteit waarvoor 
        //de kolom CL_Activiteit_Soort de waarde 'Tijdschrijven' bevat.  
        //De waarde van de kolom CL_Activiteit_Mag_direct uit codelijst CL_Activiteit dient hier opgeteld te worden.  
        public IQueryable<string> TijdschrijfActiviteiten(ApplicationContext context)
        {
            return from a in context.Activiteit where a.soort == "Tijdschrijven" select a.Code;
        }

        //Dagbestedings activiteiten zijn activiteiten uit codelijst CL_Activiteit waarvoor 
        //de kolom CL_Activiteit_Soort de waarde 'Dagbesteding' bevat en 
        //de kolom CL_Activiteit_Code de waarde 'Act_9%' bevat.  
        //Dagbesteding moet omgerekend worden naar minuten (maal 60 omdat dit in uren wordt geregistreerd).
        //public IQueryable<string> DagbestedingsActiviteiten(ApplicationContext context)
        //{
        //    return from a in context.Activiteit where a.soort == "Dagbesteding" && SqlMethods.Like(a.Code, "Act_9%") select a.Code;
        //}

        public static void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Activiteit>(b =>
            {
                b.Property(c => c.Code).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.groepcode).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.element).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.beschrijving).ColumnType("varchar").MaxLength(20);
                b.Property(c => c.aanspraak_type).ColumnType("char").MaxLength(1);
                b.Property(c => c.hierarchieniveau).ColumnType("char").MaxLength(1);
                b.Property(c => c.selecteerbaar).ColumnType("char").MaxLength(1);
                b.Property(c => c.sorteervolgorde).ColumnType("char").MaxLength(1);
                b.Property(c => c.soort).ColumnType("char").MaxLength(20);
                b.Property(c => c.mag_direct).ColumnType("char").MaxLength(1);
                b.Property(c => c.mag_indirect).ColumnType("varchar").MaxLength(1);
                b.Property(c => c.mag_reistijd).ColumnType("varchar").MaxLength(1);
                b.Property(c => c.mag_groep).ColumnType("varchar").MaxLength(1);
                

                b.Index(p => p.Code).Unique(true);
            });
        }
    }
    public sealed class ActiviteitMap : CsvClassMap<Activiteit>
    {
        public ActiviteitMap()
        {
            Map(m => m.begindatum).Index(0).TypeConverterOption("yyyyMMdd");
            Map(m => m.einddatum).Index(1).TypeConverterOption("yyyyMMdd");
            Map(m => m.Code).Index(2);
            Map(m => m.groepcode).Index(3);
            Map(m => m.element).Index(4);
            Map(m => m.beschrijving).Index(5);
            Map(m => m.aanspraak_type).Index(6);
            Map(m => m.hierarchieniveau).Index(7);
            Map(m => m.selecteerbaar).Index(8);
            Map(m => m.sorteervolgorde).Index(9);
            Map(m => m.soort).Index(10);
            Map(m => m.mag_direct).Index(11);
            Map(m => m.mag_indirect).Index(12);
            Map(m => m.mag_reistijd).Index(13);
            Map(m => m.mag_groep).Index(14);
            Map(m => m.mutatie).Index(15);
            Map(m => m.branche_indicatie).Index(16);
        }
    }

}