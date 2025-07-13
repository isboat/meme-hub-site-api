using Meme.Hub.Site.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Meme.Hub.Site.Api.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        protected string GetRequestUserId()
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("userid", StringComparison.OrdinalIgnoreCase));
            return userClaim == null ? throw new InvalidUserIdException() : userClaim.Value;
        }
    }
}
