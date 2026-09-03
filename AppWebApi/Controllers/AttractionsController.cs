using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

using Models;
using Services;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AttractionsController : Controller
    {
        readonly IAttractionService _service = null;
        readonly ILogger<AttractionsController> _logger = null;

        public AttractionsController(IAttractionService service, ILogger<AttractionsController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

