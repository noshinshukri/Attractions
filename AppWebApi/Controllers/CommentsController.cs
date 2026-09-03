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
    public class CommentsController : Controller
    {
        readonly ICommentService _service = null;
        readonly ILogger<CommentsController> _logger = null;

        public CommentsController(ICommentService service, ILogger<CommentsController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}

