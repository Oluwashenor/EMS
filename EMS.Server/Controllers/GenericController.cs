using EMS.ServerLibrary.Repositories.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T>(IGenericRepositoryInterface<T> genericRepositoryInterface) :
        Controller where T : class
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await genericRepositoryInterface.GetAll());

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid Request sent");
            return Ok(await genericRepositoryInterface.DeleteById(id));
        }

        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid Request sent");
            return Ok(await genericRepositoryInterface.GetById(id));
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(T model)
        {
            if (model is null) return BadRequest("bad Request made");
            return Ok(await genericRepositoryInterface.Insert(model));
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(T model)
        {
            if (model is null) return BadRequest("Bad Request made");
            return Ok(await genericRepositoryInterface.Update(model));
        }
    }
}
