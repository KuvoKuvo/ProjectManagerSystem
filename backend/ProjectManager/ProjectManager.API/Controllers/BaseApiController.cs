using Microsoft.AspNetCore.Mvc;
using ProjectManager.DAL.Entities;
using System.Security.Claims;

namespace ProjectManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected int CurrUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        protected string CurrUserRole
        {
            get
            {
                if (User.IsInRole(ApplicationRoles.Director)) return ApplicationRoles.Director;
                if (User.IsInRole(ApplicationRoles.ProjectManager)) return ApplicationRoles.ProjectManager;
                return ApplicationRoles.Employee;
            }
        }
    }
}
