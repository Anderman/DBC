using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Migrations.Infrastructure;
using DBC.Models.DB;

namespace DBC.Migrations
{
    [ContextType(typeof(ApplicationContext))]
    partial class a
    {
        public override string Id
        {
            get { return "20150707142034_a"; }
        }
        
        public override string ProductVersion
        {
            get { return "7.0.0-beta5-13549"; }
        }
        
        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("SqlServer:ValueGeneration", "Identity");
            
            builder.Entity("DBC.Models.DB.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .GenerateValueOnAdd();
                    
                    b.Property<int>("AccessFailedCount");
                    
                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();
                    
                    b.Property<string>("Email");
                    
                    b.Property<bool>("EmailConfirmed");
                    
                    b.Property<bool>("LockoutEnabled");
                    
                    b.Property<DateTimeOffset?>("LockoutEnd");
                    
                    b.Property<string>("NormalizedEmail");
                    
                    b.Property<string>("NormalizedUserName");
                    
                    b.Property<string>("PasswordHash");
                    
                    b.Property<string>("PhoneNumber");
                    
                    b.Property<bool>("PhoneNumberConfirmed");
                    
                    b.Property<string>("SecurityStamp");
                    
                    b.Property<bool>("TwoFactorEnabled");
                    
                    b.Property<string>("UserName");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetUsers");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Afleiding.Activiteit", b =>
                {
                    b.Property<int>("ActiviteitID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<string>("aanspraak_type");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<string>("element");
                    
                    b.Property<string>("groepcode");
                    
                    b.Property<int>("hierarchieniveau");
                    
                    b.Property<string>("mag_direct");
                    
                    b.Property<string>("mag_groep");
                    
                    b.Property<string>("mag_indirect");
                    
                    b.Property<string>("mag_reistijd");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<int>("selecteerbaar");
                    
                    b.Property<string>("soort");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Key("ActiviteitID");
                    
                    b.Index("Code", "branche_indicatie");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Afleiding.ActiviteitAndTarief", b =>
                {
                    b.Property<int>("ActiviteitAndTariefID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("declaratiecode");
                    
                    b.Property<string>("declaratiecode_kleur");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<int>("tarief_basis");
                    
                    b.Property<int>("tarief_max");
                    
                    b.Property<int>("tarief_nhc");
                    
                    b.Property<string>("tarief_omschrijving");
                    
                    b.Key("ActiviteitAndTariefID");
                    
                    b.Index("declaratiecode");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Aanspraakbeperking", b =>
                {
                    b.Property<int>("AanspraakbeperkingID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code")
                        .Annotation("MaxLength", 3)
                        .Annotation("Relational:ColumnType", "char");
                    
                    b.Property<string>("aanvullende_informatie");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<string>("omschrijving");
                    
                    b.Key("AanspraakbeperkingID");
                    
                    b.Index("Code");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Beroep", b =>
                {
                    b.Property<int>("BeroepID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<string>("element");
                    
                    b.Property<string>("groepcode");
                    
                    b.Property<int>("hierarchieniveau");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<int>("selecteerbaar");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Key("BeroepID");
                    
                    b.Index("Code", "branche_indicatie");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Beslisboom", b =>
                {
                    b.Property<int>("knoopNummer")
                        .GenerateValueOnAdd();
                    
                    b.Property<DateTime>("Begindate")
                        .GenerateValueOnAdd();
                    
                    b.Property<DateTime>("Enddate");
                    
                    b.Property<string>("kenmerkendeFactorCode");
                    
                    b.Property<int?>("knoopDoelFalse");
                    
                    b.Property<int?>("knoopDoelTrue");
                    
                    b.Property<int>("onthoudenDoelFalse");
                    
                    b.Property<int>("onthoudenDoelTrue");
                    
                    b.Property<string>("op");
                    
                    b.Property<string>("parameter1");
                    
                    b.Property<string>("parameter2");
                    
                    b.Property<int?>("waarde1");
                    
                    b.Property<int?>("waarde2");
                    
                    b.Key("knoopNummer", "Begindate");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Cirquit", b =>
                {
                    b.Property<int>("CirquitID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Key("CirquitID");
                    
                    b.Index("Code", "branche_indicatie");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.DBCs", b =>
                {
                    b.Property<int>("DBCID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<DateTime>("Begindatum");
                    
                    b.Property<string>("BehandelaarBehandelingBeroep");
                    
                    b.Property<int>("BehandelaarBehandelingID");
                    
                    b.Property<string>("BehandelaarDiagnoseBeroep");
                    
                    b.Property<int>("BehandelaarDiagnoseID");
                    
                    b.Property<int>("BehandelaarID");
                    
                    b.Property<string>("CirquitCode");
                    
                    b.Property<int>("DBCIDExtern");
                    
                    b.Property<DateTime>("Einddatum");
                    
                    b.Property<string>("PrestatieCodeExtern");
                    
                    b.Property<int?>("PrestatiecodePrestatiecodeID");
                    
                    b.Property<int?>("ProductgroepProductgroepID");
                    
                    b.Property<string>("RedensluitenCode");
                    
                    b.Property<int>("ZorgtrajectIDExtern");
                    
                    b.Property<int?>("ZorgtrajectZorgtrajectID");
                    
                    b.Property<string>("ZorgtypeCodeExtern");
                    
                    b.Property<int?>("ZorgtypeZorgtypeID");
                    
                    b.Key("DBCID");
                    
                    b.Index("DBCIDExtern");
                    
                    b.Index("ZorgtrajectIDExtern");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Dagbesteding", b =>
                {
                    b.Property<int>("DagbestedingID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("AantalUur");
                    
                    b.Property<string>("ActiviteitCode");
                    
                    b.Property<string>("Activiteitnummer");
                    
                    b.Property<int?>("DBCDBCID");
                    
                    b.Property<int>("DBCIDExtern");
                    
                    b.Property<int>("DagbestedingExtern");
                    
                    b.Property<DateTime>("Datum");
                    
                    b.Property<int>("Postwijk");
                    
                    b.Property<int>("Tarief");
                    
                    b.Property<string>("UitvoerendeInstelling");
                    
                    b.Key("DagbestedingID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Department", b =>
                {
                    b.Property<int>("DepartmentID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("BranchIndicator");
                    
                    b.Property<int>("DISNumber");
                    
                    b.Property<int?>("DepartmentDepartmentID");
                    
                    b.Property<int?>("OganizationOganizationID");
                    
                    b.Key("DepartmentID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Diagnose", b =>
                {
                    b.Property<int>("DiagnoseID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<string>("Diagnose_as");
                    
                    b.Property<int?>("aanspraak_type");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<string>("element");
                    
                    b.Property<string>("groepcode");
                    
                    b.Property<int>("hierarchieniveau");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<string>("prestatiecode_naamgeving_fz");
                    
                    b.Property<string>("prestatiecode_naamgeving_ggz");
                    
                    b.Property<string>("prestatiecodedeel_fz");
                    
                    b.Property<string>("prestatiecodedeel_ggz");
                    
                    b.Property<string>("prestatieniveau");
                    
                    b.Property<string>("refcode_icd10");
                    
                    b.Property<string>("refcode_icd9cm");
                    
                    b.Property<int>("selecteerbaar");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Property<int?>("zvz_subscore");
                    
                    b.Key("DiagnoseID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Hoofdberoep", b =>
                {
                    b.Property<int>("HoofdberoepID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Key("HoofdberoepID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Oganization", b =>
                {
                    b.Property<int>("OganizationID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("AGBCode");
                    
                    b.Property<int>("BranchIndicator");
                    
                    b.Property<int>("DISNumber");
                    
                    b.Property<int>("DepartmentID");
                    
                    b.Key("OganizationID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.OverigeDiagnose", b =>
                {
                    b.Property<int>("OverigeDiagnoseID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int?>("DBCDBCID");
                    
                    b.Property<int>("DBCIDExtern");
                    
                    b.Property<DateTime>("Datum");
                    
                    b.Property<string>("DiagnoseAS2Trekkenvan");
                    
                    b.Property<string>("DiagnoseCode");
                    
                    b.Key("OverigeDiagnoseID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Patient", b =>
                {
                    b.Property<int>("PatientID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Burgerservicenummer");
                    
                    b.Property<DateTime?>("EersteInschrijfdatum");
                    
                    b.Property<DateTime>("Geboortedatum");
                    
                    b.Property<string>("Geboortemaand");
                    
                    b.Property<string>("Geslacht");
                    
                    b.Property<string>("Huisnummer");
                    
                    b.Property<string>("HuisnummerToevoeging");
                    
                    b.Property<DateTime?>("LaatsteUitschrijfdatum");
                    
                    b.Property<string>("Landcode");
                    
                    b.Property<int>("LocationCodeExtern");
                    
                    b.Property<string>("Naam_1");
                    
                    b.Property<string>("Naam_2");
                    
                    b.Property<string>("Naam_Voorvoegsel_1");
                    
                    b.Property<string>("Naam_Voorvoegsel_2");
                    
                    b.Property<string>("Naamcode_1");
                    
                    b.Property<string>("Naamcode_2");
                    
                    b.Property<int?>("OrganizationOganizationID");
                    
                    b.Property<int>("PatientIDExtern");
                    
                    b.Property<string>("Postcode");
                    
                    b.Property<string>("Voorletters");
                    
                    b.Property<int>("ZorginstellingExtern");
                    
                    b.Key("PatientID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Prestatiecode", b =>
                {
                    b.Property<int>("PrestatiecodeID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<string>("agb_code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("cl_declaratiecode");
                    
                    b.Property<string>("cl_diagnose_prestatiecodedeel");
                    
                    b.Property<string>("cl_productgroep_code");
                    
                    b.Property<string>("cl_zorgtype_prestatiecodedeel");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<int?>("zvz");
                    
                    b.Key("PrestatiecodeID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Productgroep", b =>
                {
                    b.Property<int>("ProductgroepID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<string>("categorie");
                    
                    b.Property<string>("code_behandeling");
                    
                    b.Property<string>("code_verblijf");
                    
                    b.Property<string>("diagnose_blinderen");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<int>("hierarchieniveau");
                    
                    b.Property<string>("lekenvertaling");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<string>("omschrijving_behandeling");
                    
                    b.Property<string>("omschrijving_verblijf");
                    
                    b.Property<int>("selecteerbaar");
                    
                    b.Property<string>("setting");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Property<string>("type");
                    
                    b.Key("ProductgroepID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Redensluiten", b =>
                {
                    b.Property<int>("RedensluitenID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Key("RedensluitenID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Tijdschrijven", b =>
                {
                    b.Property<int>("TijdschrijvenID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ActiviteitCode");
                    
                    b.Property<string>("BehandelaarBeroepCode");
                    
                    b.Property<int>("BehandelaarID");
                    
                    b.Property<int?>("DBCDBCID");
                    
                    b.Property<int>("DBCIDExtern");
                    
                    b.Property<DateTime>("Datum");
                    
                    b.Property<int>("DirecteMinuten");
                    
                    b.Property<int>("IndirecteMinutenAlgemeen");
                    
                    b.Property<int>("IndirecteMinutenReis");
                    
                    b.Property<int>("TijdschrijvenIDExtern");
                    
                    b.Key("TijdschrijvenID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Verblijfsdag", b =>
                {
                    b.Property<int>("VerblijfsdagID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("AantalDagen");
                    
                    b.Property<string>("ActiviteitCode");
                    
                    b.Property<string>("Activiteitnummer");
                    
                    b.Property<DateTime>("Begindatum");
                    
                    b.Property<int?>("DBCDBCID");
                    
                    b.Property<int>("DBCIDExtern");
                    
                    b.Property<DateTime>("Einddatum");
                    
                    b.Property<int>("Postwijk");
                    
                    b.Property<int>("Tarief");
                    
                    b.Property<string>("UitvoerendeInstelling");
                    
                    b.Property<int>("VerblijfsdagIDExtern");
                    
                    b.Key("VerblijfsdagID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Zorgtraject", b =>
                {
                    b.Property<int>("ZorgtrajectID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<DateTime?>("Begindatum");
                    
                    b.Property<DateTime?>("Einddatum");
                    
                    b.Property<int>("InstellingVolgnr");
                    
                    b.Property<int>("PatientIDExtern");
                    
                    b.Property<int?>("PatientPatientID");
                    
                    b.Property<string>("PrimaireDiagnoseCodeExtern");
                    
                    b.Property<DateTime?>("PrimaireDiagnoseDatum");
                    
                    b.Property<int?>("PrimaireDiagnoseDiagnoseID");
                    
                    b.Property<string>("PrimaireDiagnoseTrekkenVan");
                    
                    b.Property<string>("SBGLocatiecode");
                    
                    b.Property<string>("StatusVlag");
                    
                    b.Property<string>("Verwijsdatum");
                    
                    b.Property<string>("VerwijzendeInstelling");
                    
                    b.Property<int?>("ZorgafdelingDepartmentID");
                    
                    b.Property<int>("ZorgtrajectIDExtern");
                    
                    b.Key("ZorgtrajectID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Zorgtype", b =>
                {
                    b.Property<int>("ZorgtypeID")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("Code");
                    
                    b.Property<DateTime>("begindatum");
                    
                    b.Property<string>("beschrijving");
                    
                    b.Property<int>("branche_indicatie");
                    
                    b.Property<DateTime>("einddatum");
                    
                    b.Property<string>("element");
                    
                    b.Property<string>("groepcode");
                    
                    b.Property<int>("hierarchieniveau");
                    
                    b.Property<int?>("mutatie");
                    
                    b.Property<string>("prestatiecodedeel");
                    
                    b.Property<int>("selecteerbaar");
                    
                    b.Property<int>("sorteervolgorde");
                    
                    b.Key("ZorgtypeID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Testset.DBCTestset", b =>
                {
                    b.Property<int>("DBCIDExtern3")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<int>("ExpectedProductgroup");
                    
                    b.Key("DBCIDExtern3");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ConcurrencyStamp")
                        .ConcurrencyToken();
                    
                    b.Property<string>("Name");
                    
                    b.Property<string>("NormalizedName");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetRoles");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ClaimType");
                    
                    b.Property<string>("ClaimValue");
                    
                    b.Property<string>("RoleId");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetRoleClaims");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .GenerateValueOnAdd()
                        .StoreGeneratedPattern(StoreGeneratedPattern.Identity);
                    
                    b.Property<string>("ClaimType");
                    
                    b.Property<string>("ClaimValue");
                    
                    b.Property<string>("UserId");
                    
                    b.Key("Id");
                    
                    b.Annotation("Relational:TableName", "AspNetUserClaims");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ProviderKey")
                        .GenerateValueOnAdd();
                    
                    b.Property<string>("ProviderDisplayName");
                    
                    b.Property<string>("UserId");
                    
                    b.Key("LoginProvider", "ProviderKey");
                    
                    b.Annotation("Relational:TableName", "AspNetUserLogins");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");
                    
                    b.Property<string>("RoleId");
                    
                    b.Key("UserId", "RoleId");
                    
                    b.Annotation("Relational:TableName", "AspNetUserRoles");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.DBCs", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Prestatiecode")
                        .InverseCollection()
                        .ForeignKey("PrestatiecodePrestatiecodeID");
                    
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Productgroep")
                        .InverseCollection()
                        .ForeignKey("ProductgroepProductgroepID");
                    
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Zorgtraject")
                        .InverseCollection()
                        .ForeignKey("ZorgtrajectZorgtrajectID");
                    
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Zorgtype")
                        .InverseCollection()
                        .ForeignKey("ZorgtypeZorgtypeID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Dagbesteding", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.DBCs")
                        .InverseCollection()
                        .ForeignKey("DBCDBCID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Department", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Department")
                        .InverseCollection()
                        .ForeignKey("DepartmentDepartmentID");
                    
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Oganization")
                        .InverseCollection()
                        .ForeignKey("OganizationOganizationID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.OverigeDiagnose", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.DBCs")
                        .InverseCollection()
                        .ForeignKey("DBCDBCID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Patient", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Oganization")
                        .InverseCollection()
                        .ForeignKey("OrganizationOganizationID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Tijdschrijven", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.DBCs")
                        .InverseCollection()
                        .ForeignKey("DBCDBCID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Verblijfsdag", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.DBCs")
                        .InverseCollection()
                        .ForeignKey("DBCDBCID");
                });
            
            builder.Entity("GGZDBC.Models.DBCModel.Registraties.Zorgtraject", b =>
                {
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Patient")
                        .InverseCollection()
                        .ForeignKey("PatientPatientID");
                    
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Diagnose")
                        .InverseCollection()
                        .ForeignKey("PrimaireDiagnoseDiagnoseID");
                    
                    b.Reference("GGZDBC.Models.DBCModel.Registraties.Department")
                        .InverseCollection()
                        .ForeignKey("ZorgafdelingDepartmentID");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Reference("DBC.Models.DB.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Reference("DBC.Models.DB.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
            
            builder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Reference("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .InverseCollection()
                        .ForeignKey("RoleId");
                    
                    b.Reference("DBC.Models.DB.ApplicationUser")
                        .InverseCollection()
                        .ForeignKey("UserId");
                });
        }
    }
}
