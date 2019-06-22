using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var response = new string[] { "value1", "value2" };
            return await Task.FromResult(response);
        }
    }
}
