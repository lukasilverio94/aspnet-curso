using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TDDExample.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="email">string</param>
        /// <returns>IActionResult</returns>
        public IActionResult RegisterUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required");
            }

            if (!IsValidEmail(email))
            {
                return BadRequest("Invalid email format");
            }

            return Ok($"User with email {email} registered successfully");
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var emailAttribute = new EmailAddressAttribute();
                return emailAttribute.IsValid(email);
            }
            catch
            {

                return false;
            }
        }
    }
}