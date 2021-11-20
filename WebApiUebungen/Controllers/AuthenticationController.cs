using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiUebungen.Entities;
using WebApiUebungen.Models;
using WebApiUebungen.Services;

namespace WebApiUebungen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthenticationController(IAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// User sign up.
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The ID of the new user.</returns>
        [HttpPost("signup")]
        public Task<int> SignUp(Credentials credentials, CancellationToken cancellationToken)
        {
            return authService.SignUp(credentials, cancellationToken);
        }

        /// <summary>
        /// User sign in.
        /// </summary>
        /// <param name="credentials"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The JSON Web Token.</returns>
        [HttpPost("signin")]
        public Task<string> SignIn(Credentials credentials, CancellationToken cancellationToken)
        {
            return authService.SignIn(credentials, cancellationToken);
        }
    }
}
