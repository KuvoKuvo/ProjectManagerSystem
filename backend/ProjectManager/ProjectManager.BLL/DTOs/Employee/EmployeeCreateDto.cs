using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Employee
{
    /// <summary>
    /// Data Transfer Object for creating or updating an employee record.
    /// </summary>
    public class EmployeeCreateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = "Employee";
    }
}
