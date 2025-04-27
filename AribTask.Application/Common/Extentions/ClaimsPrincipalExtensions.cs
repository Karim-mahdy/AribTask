using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AribTask.Application.Common.Extentions
{

    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue("uid")!;

        public static string GetRoleId(this ClaimsPrincipal principal) =>
            principal.FindFirstValue(ClaimTypes.Role)!;

    }
}
