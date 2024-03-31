using EMS.BaseLibrary.Entities;
using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(IGenericRepositoryInterface<Department> genericRepositoryInterface) 
        : GenericController<Department>(genericRepositoryInterface)
    {
    }
}
