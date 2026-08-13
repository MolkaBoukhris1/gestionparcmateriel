using Microsoft.AspNetCore.Mvc;

namespace GestionParcMateriel.API.Controllers;

/// <summary>
/// Endpoint de vérification de santé de l'API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "healthy",
            Application = "Gestion Parc Matériel API",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow
        });
    }
}
