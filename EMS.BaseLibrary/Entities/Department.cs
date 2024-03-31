using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.BaseLibrary.Entities
{
	public class Department : BaseEntity
	{
		//Many to one relationship with deneral department
		public GeneralDepartment? GeneralDepartment { get; set; }
		public int GeneralDepartmentId { get; set; }

		//One to many relationship with branch
		public List<Branch>? Branches { get; set; }
	}
}
