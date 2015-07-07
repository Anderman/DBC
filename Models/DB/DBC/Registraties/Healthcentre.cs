using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Oganization : Department
    {
        public int OganizationID { get; set; }
        public string AGBCode { get; set; }
    }
}
