using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestionParcMateriel.API.Controllers;

/// <summary>Gestion du parc matériel : CRUD + opérations métier (affecter, retour, transfert, historique).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MaterielController : ControllerBase
{
    private readonly IMaterielService  _service;
    private readonly IMouvementService _mouvementService;

    public MaterielController(IMaterielService service, IMouvementService mouvementService)
    {
        _service          = service;
        _mouvementService = mouvementService;
    }

    // ── GET /api/materiels ────────────────────────────────────────────────────
    /// <summary>Retourne la liste des matériels, avec filtres optionnels.</summary>
    /// <param name="search">Recherche textuelle (libellé, code-barres, marque, modèle, N° série)</param>
    /// <param name="etatId">Filtre par état</param>
    /// <param name="typeId">Filtre par type de matériel</param>
    /// <param name="includeInactif">Inclure les matériels désactivés (false par défaut)</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaterielDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MaterielDto>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int? etatId = null,
        [FromQuery] int? typeId = null,
        [FromQuery] bool includeInactif = false)
    {
        var result = await _service.GetAllAsync(search, etatId, typeId, includeInactif);
        return Ok(result);
    }

    // ── GET /api/materiels/{id} ───────────────────────────────────────────────
    /// <summary>Retourne le détail d'un matériel par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaterielDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MaterielDto>> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        if (dto == null)
            return NotFound(new { message = $"Matériel introuvable (Id={id})." });

        return Ok(dto);
    }

    // ── POST /api/materiels ───────────────────────────────────────────────────
    /// <summary>Crée un nouveau matériel dans le parc.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(MaterielDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MaterielDto>> Create([FromBody] MaterielCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (result, error) = await _service.CreateAsync(dto);

        if (error != null)
            return Conflict(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
    }

    // ── PUT /api/materiels/{id} ───────────────────────────────────────────────
    /// <summary>Modifie un matériel existant.</summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MaterielDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MaterielDto>> Update(int id, [FromBody] MaterielUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (result, error) = await _service.UpdateAsync(id, dto);

        if (error != null)
        {
            if (error.Contains("introuvable"))
                return NotFound(new { message = error });

            return Conflict(new { message = error });
        }

        return Ok(result);
    }

    // ── DELETE /api/materiels/{id} ────────────────────────────────────────────
    /// <summary>
    /// Suppression logique : désactive le matériel (Actif = false).
    /// Règle métier : un matériel avec historique n'est jamais supprimé physiquement.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, error) = await _service.DeleteAsync(id);

        if (!success)
            return NotFound(new { message = error });

        return NoContent();
    }

    // ── POST /api/materiels/{id}/affecter ─────────────────────────────────────
    /// <summary>
    /// Affecte un matériel (en stock) à un employé.
    /// Règle métier : état doit être "En stock", crée un mouvement Affectation.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/affecter")]
    [ProducesResponseType(typeof(MouvementMaterielDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MouvementMaterielDto>> Affecter(int id, [FromBody] AffecterMaterielDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _mouvementService.AffecterAsync(id, dto);
        if (error != null)
            return error.Contains("introuvable") ? NotFound(new { message = error }) : Conflict(new { message = error });
        return Ok(result);
    }

    // ── POST /api/materiels/{id}/retour ───────────────────────────────────────
    /// <summary>
    /// Retourne un matériel affecté au stock.
    /// Règle métier : état doit être "Affecté", libère l'employé, crée un mouvement Retour.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/retour")]
    [ProducesResponseType(typeof(MouvementMaterielDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MouvementMaterielDto>> Retour(int id, [FromBody] RetourMaterielDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _mouvementService.RetournerAsync(id, dto);
        if (error != null)
            return error.Contains("introuvable") ? NotFound(new { message = error }) : Conflict(new { message = error });
        return Ok(result);
    }

    // ── POST /api/materiels/{id}/transfert ────────────────────────────────────
    /// <summary>
    /// Transfère un matériel vers une nouvelle localisation.
    /// Règle métier : crée un mouvement Transfert, l'état ne change pas.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("{id:int}/transfert")]
    [ProducesResponseType(typeof(MouvementMaterielDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MouvementMaterielDto>> Transfert(int id, [FromBody] TransfertMaterielDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (result, error) = await _mouvementService.TransfererAsync(id, dto);
        if (error != null)
            return error.Contains("introuvable") ? NotFound(new { message = error }) : Conflict(new { message = error });
        return Ok(result);
    }

    // ── GET /api/materiels/{id}/historique ────────────────────────────────────
    /// <summary>Retourne l'historique des mouvements d'un matériel (date, type, employé, localisation).</summary>
    [HttpGet("{id:int}/historique")]
    [ProducesResponseType(typeof(IEnumerable<MouvementMaterielDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MouvementMaterielDto>>> Historique(int id)
    {
        // Vérifier que le matériel existe
        var materiel = await _service.GetByIdAsync(id);
        if (materiel == null)
            return NotFound(new { message = $"Matériel introuvable (Id={id})." });

        var historique = await _mouvementService.GetHistoriqueAsync(id);
        return Ok(historique);
    }
}