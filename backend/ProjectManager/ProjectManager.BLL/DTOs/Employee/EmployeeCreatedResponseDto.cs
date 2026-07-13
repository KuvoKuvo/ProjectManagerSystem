using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.BLL.DTOs.Employee
{
    /// <summary>
    /// Special response containing mapped employee details and the one-time plain text temporary password.
    /// </summary>
    public class EmployeeCreatedResponseDto
    {
        public EmployeeDto Employee { get; set; } = null!;
        public string TemporaryPassword { get; set; } = string.Empty;
    }
}
