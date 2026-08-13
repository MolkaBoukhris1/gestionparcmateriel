using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionParcMateriel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeController : ControllerBase
{
    private readonly IEmployeService _service;
    public EmployeController(IEmployeService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeDto>>> GetAll([FromQuery] string? search = null) =>
        Ok(await _service.GetAllAsync(search));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto == null ? NotFound(new { message = $"Employé introuvable (Id={id})." }) : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeDto>> Create([FromBody] EmployeCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _service.CreateAsync(dto);
        if (error != null) return Conflict(new { message = error });
        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeDto>> Update(int id, [FromBody] EmployeUpdateDto dto)
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
