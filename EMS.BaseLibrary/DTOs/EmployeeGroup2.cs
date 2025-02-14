using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.BaseLibrary.DTOs
{
    public class EmployeeGroup2
    {
        [Required]
        public string JobName { get; set; } = string.Empty;
        [Required, Range(1,9999, ErrorMessage = "You must select branch")]
        public int BranchId { get; set; }
        [Required, Range(1, 9999, ErrorMessage = "You must select town")]
        public int TownId { get; set; }
        [Required]
        public string? Other { get; set; }
    }
}
