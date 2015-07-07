using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGZDBC.Models.DBCModel.Registraties
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public int DISNumber { get; set; }
        public int BranchIndicator { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
