﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.BaseLibrary.Entities
{
	public class GeneralDepartment : BaseEntity
	{
		//One to many relationships with department
		public List<Department>? Departments { get; set; }
	}
}
