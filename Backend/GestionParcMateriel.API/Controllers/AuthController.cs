using GestionParcMateriel.API.Data;
using GestionParcMateriel.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionParcMateriel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var hash = ApplicationDbContext.HashPassword(dto.MotDePasse);

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.MotDePasseHash == hash);

            if (utilisateur == null)
                return Unauthorized(new { message = "Email ou mot de passe incorrect." });

            var jwtKey = _config["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment_ChangeInProduction";
            var jwtIssuer = _config["Jwt:Issuer"] ?? "GestionParcMateriel";
            var jwtAudience = _config["Jwt:Audience"] ?? "GestionParcMaterielClient";

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, utilisateur.Email),
                new Claim(ClaimTypes.Role, utilisateur.Role),
                new Claim(ClaimTypes.Name, utilisateur.NomComplet ?? utilisateur.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return Ok(new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = utilisateur.Email,
                Role = utilisateur.Role,
                NomComplet = utilisateur.NomComplet
            });
        }
    }
}