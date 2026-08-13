using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionParcMateriel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LocalisationController : ControllerBase
{
    private readonly ILocalisationService _service;
    public LocalisationController(ILocalisationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocalisationDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LocalisationDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto == null ? NotFound(new { message = $"Localisation introuvable (Id={id})." }) : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<LocalisationDto>> Create([FromBody] LocalisationCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _service.CreateAsync(dto);
        if (error != null) return Conflict(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<LocalisationDto>> Update(int id, [FromBody] LocalisationCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _service.UpdateAsync(id, dto);
        if (error != null) return error.Contains("introuvable") ? NotFound(new { message = error }) : Conflict(new { message = error });
        return Ok(result);
    }

   [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var (success, error) = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound(new { message = error });
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return Conflict(new { message = "Impossible de supprimer cette localisation : elle est encore utilisée par au moins un matériel." });
        }
    }
}
