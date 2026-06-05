using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SiwesConnect.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SiwesConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthApiController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // POST: api/AuthApi/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                UserType = model.UserType,
                CompanyID = model.CompanyID
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, user.UserType!);
                return Ok(new { message = "Registration successful" });
            }

            return BadRequest(result.Errors);
        }

        // POST: api/AuthApi/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email!);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            // Verify the password without creating a cookie session
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password!);
            if (!passwordValid)
                return Unauthorized(new { message = "Invalid email or password" });

            // Get the user's role
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            // Generate the JWT token
            var token = GenerateJwtToken(user, role!);

            return Ok(new
            {
                token = token,
                role = role,
                email = user.Email,
                fullName = user.FullName,
                userId = user.Id
            });
        }

        // POST: api/AuthApi/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // JWT is stateless — logout is handled on the frontend
            // by deleting the token from localStorage
            return Ok(new { message = "Logged out successfully" });
        }

        // Private helper — builds and signs the JWT token
        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]!);

            // Claims are pieces of information embedded inside the token
            // The frontend can read these without calling the server
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}