using System.ComponentModel.DataAnnotations;
using System.Linq;
using WasiApiGateway.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WasiApiGateway.Controllers
{
    [ApiController]
    [Route("api-handlers")]
    public class ApiHandlersController : ControllerBase
    {
        private readonly ApiHandlersContainer _container;

        public ApiHandlersController(ApiHandlersContainer container)
        {
            _container = container;
        }

        [HttpPost]
        public void AddApiHandler(
            [FromForm(Name = "path"), Required] string path,
            [FromForm(Name = "method"), Required] string method,
            [FromForm(Name = "wasi_module"), Required] IFormFile wasiModuleFile)
        {
            using var wasiModuleStream = wasiModuleFile.OpenReadStream();
            _container.AddOrUpdateApiHandler(path, method, wasiModuleStream);
        }

        [HttpGet]
        public object GetApiHandlers()
        {
            return new
            {
                ApiHandlers = _container.GetMappedRoutes().Select(r => $"{r.method} {r.path}"),
            };
        }
    }
}