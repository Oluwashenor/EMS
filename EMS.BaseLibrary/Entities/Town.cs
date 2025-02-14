﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EMS.BaseLibrary.Entities
{
	public class Town : BaseEntity
	{
        //Relationship : One to many with Employee
        [JsonIgnore]
        public List<Employee>? Employees { get; set; }
		//Many to one relationshop with city
		public City? City { get; set; }
		public int CityId { get; set; }
	}
}
