using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Employee
{
    /// <summary>
    /// Data Transfer Object for displaying employee information.
    /// </summary>
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Employee";


        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }
}
