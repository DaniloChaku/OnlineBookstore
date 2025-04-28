using Microsoft.AspNetCore.Mvc;

namespace OnlineBookstore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public abstract class BaseApiController : ControllerBase
{
}
