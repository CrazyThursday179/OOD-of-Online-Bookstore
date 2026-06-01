using FavouriteBooks.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult FromResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    protected ActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
