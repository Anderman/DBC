using System.Collections.Generic;
using Microsoft.Data.Entity.Relational.Migrations;
using Microsoft.Data.Entity.Relational.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Migrations.Operations;

namespace DBC.Migrations
{
    public partial class a : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.DropIndex(name: "EmailIndex", table: "AspNetUsers");
            migration.DropIndex(name: "UserNameIndex", table: "AspNetUsers");
            migration.DropIndex(name: "RoleNameIndex", table: "AspNetRoles");
            migration.CreateTable(
                name: "Activiteit",
                columns: table => new
                {
                    ActiviteitID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    aanspraak_type = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    element = table.Column(type: "nvarchar(max)", nullable: true),
                    groepcode = table.Column(type: "nvarchar(max)", nullable: true),
                    hierarchieniveau = table.Column(type: "int", nullable: false),
                    mag_direct = table.Column(type: "nvarchar(max)", nullable: true),
                    mag_groep = table.Column(type: "nvarchar(max)", nullable: true),
                    mag_indirect = table.Column(type: "nvarchar(max)", nullable: true),
                    mag_reistijd = table.Column(type: "nvarchar(max)", nullable: true),
                    mutatie = table.Column(type: "int", nullable: true),
                    selecteerbaar = table.Column(type: "int", nullable: false),
                    soort = table.Column(type: "nvarchar(max)", nullable: true),
                    sorteervolgorde = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activiteit", x => x.ActiviteitID);
                });
            migration.CreateTable(
                name: "ActiviteitAndTarief",
                columns: table => new
                {
                    ActiviteitAndTariefID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    declaratiecode = table.Column(type: "nvarchar(max)", nullable: true),
                    declaratiecode_kleur = table.Column(type: "nvarchar(max)", nullable: true),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    tarief_basis = table.Column(type: "int", nullable: false),
                    tarief_max = table.Column(type: "int", nullable: false),
                    tarief_nhc = table.Column(type: "int", nullable: false),
                    tarief_omschrijving = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiviteitAndTarief", x => x.ActiviteitAndTariefID);
                });
            migration.CreateTable(
                name: "Aanspraakbeperking",
                columns: table => new
                {
                    AanspraakbeperkingID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    aanvullende_informatie = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    omschrijving = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aanspraakbeperking", x => x.AanspraakbeperkingID);
                });
            migration.CreateTable(
                name: "Beroep",
                columns: table => new
                {
                    BeroepID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    element = table.Column(type: "nvarchar(max)", nullable: true),
                    groepcode = table.Column(type: "nvarchar(max)", nullable: true),
                    hierarchieniveau = table.Column(type: "int", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    selecteerbaar = table.Column(type: "int", nullable: false),
                    sorteervolgorde = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beroep", x => x.BeroepID);
                });
            migration.CreateTable(
                name: "Beslisboom",
                columns: table => new
                {
                    knoopNummer = table.Column(type: "int", nullable: false),
                    Begindate = table.Column(type: "datetime2", nullable: false),
                    Enddate = table.Column(type: "datetime2", nullable: false),
                    kenmerkendeFactorCode = table.Column(type: "nvarchar(max)", nullable: true),
                    knoopDoelFalse = table.Column(type: "int", nullable: true),
                    knoopDoelTrue = table.Column(type: "int", nullable: true),
                    onthoudenDoelFalse = table.Column(type: "int", nullable: false),
                    onthoudenDoelTrue = table.Column(type: "int", nullable: false),
                    op = table.Column(type: "nvarchar(max)", nullable: true),
                    parameter1 = table.Column(type: "nvarchar(max)", nullable: true),
                    parameter2 = table.Column(type: "nvarchar(max)", nullable: true),
                    waarde1 = table.Column(type: "int", nullable: true),
                    waarde2 = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beslisboom", x => new { x.knoopNummer, x.Begindate });
                });
            migration.CreateTable(
                name: "Cirquit",
                columns: table => new
                {
                    CirquitID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    sorteervolgorde = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cirquit", x => x.CirquitID);
                });
            migration.CreateTable(
                name: "Diagnose",
                columns: table => new
                {
                    DiagnoseID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    Diagnose_as = table.Column(type: "nvarchar(max)", nullable: true),
                    aanspraak_type = table.Column(type: "int", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    element = table.Column(type: "nvarchar(max)", nullable: true),
                    groepcode = table.Column(type: "nvarchar(max)", nullable: true),
                    hierarchieniveau = table.Column(type: "int", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    prestatiecode_naamgeving_fz = table.Column(type: "nvarchar(max)", nullable: true),
                    prestatiecode_naamgeving_ggz = table.Column(type: "nvarchar(max)", nullable: true),
                    prestatiecodedeel_fz = table.Column(type: "nvarchar(max)", nullable: true),
                    prestatiecodedeel_ggz = table.Column(type: "nvarchar(max)", nullable: true),
                    prestatieniveau = table.Column(type: "nvarchar(max)", nullable: true),
                    refcode_icd10 = table.Column(type: "nvarchar(max)", nullable: true),
                    refcode_icd9cm = table.Column(type: "nvarchar(max)", nullable: true),
                    selecteerbaar = table.Column(type: "int", nullable: false),
                    sorteervolgorde = table.Column(type: "int", nullable: false),
                    zvz_subscore = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnose", x => x.DiagnoseID);
                });
            migration.CreateTable(
                name: "Hoofdberoep",
                columns: table => new
                {
                    HoofdberoepID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoofdberoep", x => x.HoofdberoepID);
                });
            migration.CreateTable(
                name: "Oganization",
                columns: table => new
                {
                    OganizationID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    AGBCode = table.Column(type: "nvarchar(max)", nullable: true),
                    BranchIndicator = table.Column(type: "int", nullable: false),
                    DISNumber = table.Column(type: "int", nullable: false),
                    DepartmentID = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oganization", x => x.OganizationID);
                });
            migration.CreateTable(
                name: "Prestatiecode",
                columns: table => new
                {
                    PrestatiecodeID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    agb_code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    cl_declaratiecode = table.Column(type: "nvarchar(max)", nullable: true),
                    cl_diagnose_prestatiecodedeel = table.Column(type: "nvarchar(max)", nullable: true),
                    cl_productgroep_code = table.Column(type: "nvarchar(max)", nullable: true),
                    cl_zorgtype_prestatiecodedeel = table.Column(type: "nvarchar(max)", nullable: true),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    zvz = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prestatiecode", x => x.PrestatiecodeID);
                });
            migration.CreateTable(
                name: "Productgroep",
                columns: table => new
                {
                    ProductgroepID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    categorie = table.Column(type: "nvarchar(max)", nullable: true),
                    code_behandeling = table.Column(type: "nvarchar(max)", nullable: true),
                    code_verblijf = table.Column(type: "nvarchar(max)", nullable: true),
                    diagnose_blinderen = table.Column(type: "nvarchar(max)", nullable: true),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    hierarchieniveau = table.Column(type: "int", nullable: false),
                    lekenvertaling = table.Column(type: "nvarchar(max)", nullable: true),
                    mutatie = table.Column(type: "int", nullable: true),
                    omschrijving_behandeling = table.Column(type: "nvarchar(max)", nullable: true),
                    omschrijving_verblijf = table.Column(type: "nvarchar(max)", nullable: true),
                    selecteerbaar = table.Column(type: "int", nullable: false),
                    setting = table.Column(type: "nvarchar(max)", nullable: true),
                    sorteervolgorde = table.Column(type: "int", nullable: false),
                    @type = table.Column(name: "type", type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productgroep", x => x.ProductgroepID);
                });
            migration.CreateTable(
                name: "Redensluiten",
                columns: table => new
                {
                    RedensluitenID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    sorteervolgorde = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redensluiten", x => x.RedensluitenID);
                });
            migration.CreateTable(
                name: "Zorgtype",
                columns: table => new
                {
                    ZorgtypeID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Code = table.Column(type: "nvarchar(max)", nullable: true),
                    begindatum = table.Column(type: "datetime2", nullable: false),
                    beschrijving = table.Column(type: "nvarchar(max)", nullable: true),
                    branche_indicatie = table.Column(type: "int", nullable: false),
                    einddatum = table.Column(type: "datetime2", nullable: false),
                    element = table.Column(type: "nvarchar(max)", nullable: true),
                    groepcode = table.Column(type: "nvarchar(max)", nullable: true),
                    hierarchieniveau = table.Column(type: "int", nullable: false),
                    mutatie = table.Column(type: "int", nullable: true),
                    prestatiecodedeel = table.Column(type: "nvarchar(max)", nullable: true),
                    selecteerbaar = table.Column(type: "int", nullable: false),
                    sorteervolgorde = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zorgtype", x => x.ZorgtypeID);
                });
            migration.CreateTable(
                name: "DBCTestset",
                columns: table => new
                {
                    DBCIDExtern3 = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ExpectedProductgroup = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBCTestset", x => x.DBCIDExtern3);
                });
            migration.CreateTable(
                name: "Department",
                columns: table => new
                {
                    DepartmentID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    BranchIndicator = table.Column(type: "int", nullable: false),
                    DISNumber = table.Column(type: "int", nullable: false),
                    DepartmentDepartmentID = table.Column(type: "int", nullable: true),
                    OganizationOganizationID = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Department_Department_DepartmentDepartmentID",
                        columns: x => x.DepartmentDepartmentID,
                        referencedTable: "Department",
                        referencedColumn: "DepartmentID");
                    table.ForeignKey(
                        name: "FK_Department_Oganization_OganizationOganizationID",
                        columns: x => x.OganizationOganizationID,
                        referencedTable: "Oganization",
                        referencedColumn: "OganizationID");
                });
            migration.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    PatientID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Burgerservicenummer = table.Column(type: "nvarchar(max)", nullable: true),
                    EersteInschrijfdatum = table.Column(type: "datetime2", nullable: true),
                    Geboortedatum = table.Column(type: "datetime2", nullable: false),
                    Geboortemaand = table.Column(type: "nvarchar(max)", nullable: true),
                    Geslacht = table.Column(type: "nvarchar(max)", nullable: true),
                    Huisnummer = table.Column(type: "nvarchar(max)", nullable: true),
                    HuisnummerToevoeging = table.Column(type: "nvarchar(max)", nullable: true),
                    LaatsteUitschrijfdatum = table.Column(type: "datetime2", nullable: true),
                    Landcode = table.Column(type: "nvarchar(max)", nullable: true),
                    LocationCodeExtern = table.Column(type: "int", nullable: false),
                    Naam_1 = table.Column(type: "nvarchar(max)", nullable: true),
                    Naam_2 = table.Column(type: "nvarchar(max)", nullable: true),
                    Naam_Voorvoegsel_1 = table.Column(type: "nvarchar(max)", nullable: true),
                    Naam_Voorvoegsel_2 = table.Column(type: "nvarchar(max)", nullable: true),
                    Naamcode_1 = table.Column(type: "nvarchar(max)", nullable: true),
                    Naamcode_2 = table.Column(type: "nvarchar(max)", nullable: true),
                    OrganizationOganizationID = table.Column(type: "int", nullable: true),
                    PatientIDExtern = table.Column(type: "int", nullable: false),
                    Postcode = table.Column(type: "nvarchar(max)", nullable: true),
                    Voorletters = table.Column(type: "nvarchar(max)", nullable: true),
                    ZorginstellingExtern = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.PatientID);
                    table.ForeignKey(
                        name: "FK_Patient_Oganization_OrganizationOganizationID",
                        columns: x => x.OrganizationOganizationID,
                        referencedTable: "Oganization",
                        referencedColumn: "OganizationID");
                });
            migration.CreateTable(
                name: "Zorgtraject",
                columns: table => new
                {
                    ZorgtrajectID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Begindatum = table.Column(type: "datetime2", nullable: true),
                    Einddatum = table.Column(type: "datetime2", nullable: true),
                    InstellingVolgnr = table.Column(type: "int", nullable: false),
                    PatientIDExtern = table.Column(type: "int", nullable: false),
                    PatientPatientID = table.Column(type: "int", nullable: true),
                    PrimaireDiagnoseCodeExtern = table.Column(type: "nvarchar(max)", nullable: true),
                    PrimaireDiagnoseDatum = table.Column(type: "datetime2", nullable: true),
                    PrimaireDiagnoseDiagnoseID = table.Column(type: "int", nullable: true),
                    PrimaireDiagnoseTrekkenVan = table.Column(type: "nvarchar(max)", nullable: true),
                    SBGLocatiecode = table.Column(type: "nvarchar(max)", nullable: true),
                    StatusVlag = table.Column(type: "nvarchar(max)", nullable: true),
                    Verwijsdatum = table.Column(type: "nvarchar(max)", nullable: true),
                    VerwijzendeInstelling = table.Column(type: "nvarchar(max)", nullable: true),
                    ZorgafdelingDepartmentID = table.Column(type: "int", nullable: true),
                    ZorgtrajectIDExtern = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zorgtraject", x => x.ZorgtrajectID);
                    table.ForeignKey(
                        name: "FK_Zorgtraject_Patient_PatientPatientID",
                        columns: x => x.PatientPatientID,
                        referencedTable: "Patient",
                        referencedColumn: "PatientID");
                    table.ForeignKey(
                        name: "FK_Zorgtraject_Diagnose_PrimaireDiagnoseDiagnoseID",
                        columns: x => x.PrimaireDiagnoseDiagnoseID,
                        referencedTable: "Diagnose",
                        referencedColumn: "DiagnoseID");
                    table.ForeignKey(
                        name: "FK_Zorgtraject_Department_ZorgafdelingDepartmentID",
                        columns: x => x.ZorgafdelingDepartmentID,
                        referencedTable: "Department",
                        referencedColumn: "DepartmentID");
                });
            migration.CreateTable(
                name: "DBCs",
                columns: table => new
                {
                    DBCID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    Begindatum = table.Column(type: "datetime2", nullable: false),
                    BehandelaarBehandelingBeroep = table.Column(type: "nvarchar(max)", nullable: true),
                    BehandelaarBehandelingID = table.Column(type: "int", nullable: false),
                    BehandelaarDiagnoseBeroep = table.Column(type: "nvarchar(max)", nullable: true),
                    BehandelaarDiagnoseID = table.Column(type: "int", nullable: false),
                    BehandelaarID = table.Column(type: "int", nullable: false),
                    CirquitCode = table.Column(type: "nvarchar(max)", nullable: true),
                    DBCIDExtern = table.Column(type: "int", nullable: false),
                    Einddatum = table.Column(type: "datetime2", nullable: false),
                    PrestatieCodeExtern = table.Column(type: "nvarchar(max)", nullable: true),
                    PrestatiecodePrestatiecodeID = table.Column(type: "int", nullable: true),
                    ProductgroepProductgroepID = table.Column(type: "int", nullable: true),
                    RedensluitenCode = table.Column(type: "nvarchar(max)", nullable: true),
                    ZorgtrajectIDExtern = table.Column(type: "int", nullable: false),
                    ZorgtrajectZorgtrajectID = table.Column(type: "int", nullable: true),
                    ZorgtypeCodeExtern = table.Column(type: "nvarchar(max)", nullable: true),
                    ZorgtypeZorgtypeID = table.Column(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBCs", x => x.DBCID);
                    table.ForeignKey(
                        name: "FK_DBCs_Prestatiecode_PrestatiecodePrestatiecodeID",
                        columns: x => x.PrestatiecodePrestatiecodeID,
                        referencedTable: "Prestatiecode",
                        referencedColumn: "PrestatiecodeID");
                    table.ForeignKey(
                        name: "FK_DBCs_Productgroep_ProductgroepProductgroepID",
                        columns: x => x.ProductgroepProductgroepID,
                        referencedTable: "Productgroep",
                        referencedColumn: "ProductgroepID");
                    table.ForeignKey(
                        name: "FK_DBCs_Zorgtraject_ZorgtrajectZorgtrajectID",
                        columns: x => x.ZorgtrajectZorgtrajectID,
                        referencedTable: "Zorgtraject",
                        referencedColumn: "ZorgtrajectID");
                    table.ForeignKey(
                        name: "FK_DBCs_Zorgtype_ZorgtypeZorgtypeID",
                        columns: x => x.ZorgtypeZorgtypeID,
                        referencedTable: "Zorgtype",
                        referencedColumn: "ZorgtypeID");
                });
            migration.CreateTable(
                name: "Dagbesteding",
                columns: table => new
                {
                    DagbestedingID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    AantalUur = table.Column(type: "int", nullable: false),
                    ActiviteitCode = table.Column(type: "nvarchar(max)", nullable: true),
                    Activiteitnummer = table.Column(type: "nvarchar(max)", nullable: true),
                    DBCDBCID = table.Column(type: "int", nullable: true),
                    DBCIDExtern = table.Column(type: "int", nullable: false),
                    DagbestedingExtern = table.Column(type: "int", nullable: false),
                    Datum = table.Column(type: "datetime2", nullable: false),
                    Postwijk = table.Column(type: "int", nullable: false),
                    Tarief = table.Column(type: "int", nullable: false),
                    UitvoerendeInstelling = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dagbesteding", x => x.DagbestedingID);
                    table.ForeignKey(
                        name: "FK_Dagbesteding_DBCs_DBCDBCID",
                        columns: x => x.DBCDBCID,
                        referencedTable: "DBCs",
                        referencedColumn: "DBCID");
                });
            migration.CreateTable(
                name: "OverigeDiagnose",
                columns: table => new
                {
                    OverigeDiagnoseID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    DBCDBCID = table.Column(type: "int", nullable: true),
                    DBCIDExtern = table.Column(type: "int", nullable: false),
                    Datum = table.Column(type: "datetime2", nullable: false),
                    DiagnoseAS2Trekkenvan = table.Column(type: "nvarchar(max)", nullable: true),
                    DiagnoseCode = table.Column(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverigeDiagnose", x => x.OverigeDiagnoseID);
                    table.ForeignKey(
                        name: "FK_OverigeDiagnose_DBCs_DBCDBCID",
                        columns: x => x.DBCDBCID,
                        referencedTable: "DBCs",
                        referencedColumn: "DBCID");
                });
            migration.CreateTable(
                name: "Tijdschrijven",
                columns: table => new
                {
                    TijdschrijvenID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    ActiviteitCode = table.Column(type: "nvarchar(max)", nullable: true),
                    BehandelaarBeroepCode = table.Column(type: "nvarchar(max)", nullable: true),
                    BehandelaarID = table.Column(type: "int", nullable: false),
                    DBCDBCID = table.Column(type: "int", nullable: true),
                    DBCIDExtern = table.Column(type: "int", nullable: false),
                    Datum = table.Column(type: "datetime2", nullable: false),
                    DirecteMinuten = table.Column(type: "int", nullable: false),
                    IndirecteMinutenAlgemeen = table.Column(type: "int", nullable: false),
                    IndirecteMinutenReis = table.Column(type: "int", nullable: false),
                    TijdschrijvenIDExtern = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tijdschrijven", x => x.TijdschrijvenID);
                    table.ForeignKey(
                        name: "FK_Tijdschrijven_DBCs_DBCDBCID",
                        columns: x => x.DBCDBCID,
                        referencedTable: "DBCs",
                        referencedColumn: "DBCID");
                });
            migration.CreateTable(
                name: "Verblijfsdag",
                columns: table => new
                {
                    VerblijfsdagID = table.Column(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGeneration", "Identity"),
                    AantalDagen = table.Column(type: "int", nullable: false),
                    ActiviteitCode = table.Column(type: "nvarchar(max)", nullable: true),
                    Activiteitnummer = table.Column(type: "nvarchar(max)", nullable: true),
                    Begindatum = table.Column(type: "datetime2", nullable: false),
                    DBCDBCID = table.Column(type: "int", nullable: true),
                    DBCIDExtern = table.Column(type: "int", nullable: false),
                    Einddatum = table.Column(type: "datetime2", nullable: false),
                    Postwijk = table.Column(type: "int", nullable: false),
                    Tarief = table.Column(type: "int", nullable: false),
                    UitvoerendeInstelling = table.Column(type: "nvarchar(max)", nullable: true),
                    VerblijfsdagIDExtern = table.Column(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verblijfsdag", x => x.VerblijfsdagID);
                    table.ForeignKey(
                        name: "FK_Verblijfsdag_DBCs_DBCDBCID",
                        columns: x => x.DBCDBCID,
                        referencedTable: "DBCs",
                        referencedColumn: "DBCID");
                });
            migration.CreateIndex(
                name: "IX_Activiteit_Code_branche_indicatie",
                table: "Activiteit",
                columns: new[] { "Code", "branche_indicatie" });
            migration.CreateIndex(
                name: "IX_ActiviteitAndTarief_declaratiecode",
                table: "ActiviteitAndTarief",
                column: "declaratiecode");
            migration.CreateIndex(
                name: "IX_Aanspraakbeperking_Code",
                table: "Aanspraakbeperking",
                column: "Code");
            migration.CreateIndex(
                name: "IX_Beroep_Code_branche_indicatie",
                table: "Beroep",
                columns: new[] { "Code", "branche_indicatie" });
            migration.CreateIndex(
                name: "IX_Cirquit_Code_branche_indicatie",
                table: "Cirquit",
                columns: new[] { "Code", "branche_indicatie" });
            migration.CreateIndex(
                name: "IX_DBCs_DBCIDExtern",
                table: "DBCs",
                column: "DBCIDExtern");
            migration.CreateIndex(
                name: "IX_DBCs_ZorgtrajectIDExtern",
                table: "DBCs",
                column: "ZorgtrajectIDExtern");
        }
        
        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("Activiteit");
            migration.DropTable("ActiviteitAndTarief");
            migration.DropTable("Aanspraakbeperking");
            migration.DropTable("Beroep");
            migration.DropTable("Beslisboom");
            migration.DropTable("Cirquit");
            migration.DropTable("DBCs");
            migration.DropTable("Dagbesteding");
            migration.DropTable("Department");
            migration.DropTable("Diagnose");
            migration.DropTable("Hoofdberoep");
            migration.DropTable("Oganization");
            migration.DropTable("OverigeDiagnose");
            migration.DropTable("Patient");
            migration.DropTable("Prestatiecode");
            migration.DropTable("Productgroep");
            migration.DropTable("Redensluiten");
            migration.DropTable("Tijdschrijven");
            migration.DropTable("Verblijfsdag");
            migration.DropTable("Zorgtraject");
            migration.DropTable("Zorgtype");
            migration.DropTable("DBCTestset");
            migration.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
            migration.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");
            migration.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
        }
    }
}
