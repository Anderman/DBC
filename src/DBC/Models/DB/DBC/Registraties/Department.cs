using System.Collections.Generic;

namespace DBC.Models.DB.DBC.Registraties
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public int DISNumber { get; set; }
        public int BranchIndicator { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
