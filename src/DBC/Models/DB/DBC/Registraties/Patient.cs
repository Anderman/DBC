using System;
using CsvHelper.Configuration;

namespace DBC.Models.DB.DBC.Registraties
{
    public class Patient
    {
        public int PatientID { get; set; }
        public virtual Oganization Organization { get; set; }
        public string Naam_1 { get; set; }
        public string Naam_Voorvoegsel_1 { get; set; }
        public string Naamcode_1 { get; set; }
        public string Naam_2 { get; set; }
        public string Naam_Voorvoegsel_2 { get; set; }
        public string Naamcode_2 { get; set; }
        public string Voorletters { get; set; }
        public string Postcode { get; set; }
        public string Huisnummer { get; set; }
        public string HuisnummerToevoeging { get; set; }
        public string Landcode { get; set; }
        public DateTime Geboortedatum { get; set; }
        public string Geslacht { get; set; }
        public string Burgerservicenummer { get; set; }
        public DateTime? EersteInschrijfdatum { get; set; }
        public DateTime? LaatsteUitschrijfdatum { get; set; }
        public string Geboortemaand { get; set; }
        //public ISO3166_1 GeboortelandPatiënt { get; set; }
        //public ISO3166_1 GeboortelandVader { get; set; }
        //public ISO3166_1 GeboortelandMoeder { get; set; }
        //public Leefsituatie Leefsituatie { get; set; }
        //public Opleidingsniveau Opleidingsniveau { get; set; }
        //Import specific
        //[Index]
        public int PatientIDExtern { get; set; }
        //[Index]
        public int LocationCodeExtern { get; set; }
        //[Index]
        public int ZorginstellingExtern { get; set; }

    }
    //IDENTIFICATIENUMMER|NAAM_1|NAAMCODE_1|VOORLETTERS|HUISNUMMER|LANDCODE|GEBOORTEDATUM|POSTCODENUMMER|GESLACHT|CL_INSTELLING_CODE|LOCATIE_CODE
    public sealed class PatientMapTestSetDBCOnderhoud : CsvClassMap<Patient>
    {
        public PatientMapTestSetDBCOnderhoud()
        {
            Map(m => m.PatientIDExtern).Index(0);
            Map(m => m.Naam_1).Index(1);
            Map(m => m.Naamcode_1).Index(2);
            Map(m => m.Voorletters).Index(3);
            Map(m => m.Huisnummer).Index(4);
            Map(m => m.Landcode).Index(5);
            Map(m => m.Geboortedatum).Index(6).TypeConverterOption("yyyyMMdd");
            Map(m => m.Postcode).Index(7);
            Map(m => m.Geslacht).Index(8);
            Map(m => m.ZorginstellingExtern).Index(9);
            Map(m => m.LocationCodeExtern).Index(10);
        }
    }

}