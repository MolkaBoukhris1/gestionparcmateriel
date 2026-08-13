using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionParcMateriel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EtatMaterielController : ControllerBase
{
    private readonly IEtatMaterielService _service;
    public EtatMaterielController(IEtatMaterielService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EtatMaterielDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EtatMaterielDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto == null ? NotFound(new { message = $"État introuvable (Id={id})." }) : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<EtatMaterielDto>> Create([FromBody] EtatMaterielCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _service.CreateAsync(dto);
        if (error != null) return Conflict(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EtatMaterielDto>> Update(int id, [FromBody] EtatMaterielCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _service.UpdateAsync(id, dto);
        if (error != null) return error.Contains("introuvable") ? NotFound(new { message = error }) : Conflict(new { message = error });
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _service.DeleteAsync(id);
        return success ? NoContent() : NotFound(new { message = error });
    }
}
