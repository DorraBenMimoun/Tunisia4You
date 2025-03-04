using Microsoft.AspNetCore.Mvc;
using MiniProjet.Helpers;
using MiniProjet.Models;
using MiniProjet.Repositories;
using BCrypt.Net;
using System.Threading.Tasks;

namespace MiniProjet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthController(UserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            // Hash du mot de passe
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            await _userRepository.CreateAsync(user);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser == null)
            {
                return Unauthorized("Invalid credentials");
            }
            Console.WriteLine($"existingUser : {existingUser}");

            // Vérification du mot de passe
            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, existingUser.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _jwtHelper.GenerateToken(existingUser);
            return Ok(new { token });
        }
    }
}
