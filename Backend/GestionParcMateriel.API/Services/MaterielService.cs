using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Models;
using GestionParcMateriel.API.Repositories;

namespace GestionParcMateriel.API.Services;

public class MaterielService : IMaterielService
{
    private readonly IMaterielRepository _repository;

    public MaterielService(IMaterielRepository repository)
    {
        _repository = repository;
    }

    // ── Lecture ───────────────────────────────────────────────────────────────

    public async Task<IEnumerable<MaterielDto>> GetAllAsync(
        string? search = null, int? etatId = null, int? typeId = null, bool includeInactif = false)
    {
        var materiels = await _repository.GetAllAsync(search, etatId, typeId, includeInactif);
        return materiels.Select(MapToDto);
    }

    public async Task<MaterielDto?> GetByIdAsync(int id)
    {
        var materiel = await _repository.GetByIdAsync(id);
        return materiel == null ? null : MapToDto(materiel);
    }

    // ── Création ──────────────────────────────────────────────────────────────

    public async Task<(MaterielDto? Result, string? Error)> CreateAsync(MaterielCreateDto dto)
    {
        // Règle métier 7 : unicité code-barres
        if (await _repository.ExistsByCodeBarreAsync(dto.CodeBarre))
            return (null, $"Le code-barres '{dto.CodeBarre}' est déjà utilisé par un autre matériel.");

        // Règle métier 7 : unicité numéro de série si renseigné
        if (!string.IsNullOrWhiteSpace(dto.NumeroSerie) &&
            await _repository.ExistsByNumeroSerieAsync(dto.NumeroSerie))
            return (null, $"Le numéro de série '{dto.NumeroSerie}' est déjà utilisé par un autre matériel.");

        var materiel = new Materiel
        {
            CodeBarre        = dto.CodeBarre.Trim(),
            Libelle          = dto.Libelle.Trim(),
            TypeMaterielId   = dto.TypeMaterielId,
            Marque           = dto.Marque?.Trim(),
            Modele           = dto.Modele?.Trim(),
            NumeroSerie      = string.IsNullOrWhiteSpace(dto.NumeroSerie) ? null : dto.NumeroSerie.Trim(),
            DateAcquisition  = dto.DateAcquisition,
            EtatId           = dto.EtatId,
            LocalisationId   = dto.LocalisationId,
            EmployeId        = dto.EmployeId,
            Commentaires     = dto.Commentaires?.Trim(),
            Actif            = true,
            DateCreation     = DateTime.UtcNow,
            DateModification = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(materiel);

        // Recharger avec les relations pour le DTO de retour
        var withNav = await _repository.GetByIdAsync(created.Id);
        return (withNav == null ? null : MapToDto(withNav), null);
    }

    // ── Modification ──────────────────────────────────────────────────────────

    public async Task<(MaterielDto? Result, string? Error)> UpdateAsync(int id, MaterielUpdateDto dto)
    {
        var materiel = await _repository.GetByIdAsync(id);
        if (materiel == null)
            return (null, $"Matériel introuvable (Id={id}).");

        // Règle métier 7 : unicité numéro de série (hors soi-même)
        if (!string.IsNullOrWhiteSpace(dto.NumeroSerie) &&
            await _repository.ExistsByNumeroSerieAsync(dto.NumeroSerie, excludeId: id))
            return (null, $"Le numéro de série '{dto.NumeroSerie}' est déjà utilisé par un autre matériel.");

        materiel.Libelle          = dto.Libelle.Trim();
        materiel.TypeMaterielId   = dto.TypeMaterielId;
        materiel.Marque           = dto.Marque?.Trim();
        materiel.Modele           = dto.Modele?.Trim();
        materiel.NumeroSerie      = string.IsNullOrWhiteSpace(dto.NumeroSerie) ? null : dto.NumeroSerie.Trim();
        materiel.DateAcquisition  = dto.DateAcquisition;
        materiel.EtatId           = dto.EtatId;
        materiel.LocalisationId   = dto.LocalisationId;
        materiel.EmployeId        = dto.EmployeId;
        materiel.Commentaires     = dto.Commentaires?.Trim();
        materiel.DateModification = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(materiel);

        var withNav = await _repository.GetByIdAsync(updated.Id);
        return (withNav == null ? null : MapToDto(withNav), null);
    }

    // ── Suppression logique ───────────────────────────────────────────────────

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var materiel = await _repository.GetByIdAsync(id);
        if (materiel == null)
            return (false, $"Matériel introuvable (Id={id}).");

        // Règle métier 8 : si le matériel a un historique, on le désactive (soft delete)
        var deleted = await _repository.SoftDeleteAsync(id);
        return deleted
            ? (true, null)
            : (false, "La suppression a échoué.");
    }

    // ── Mapping Entity → DTO ──────────────────────────────────────────────────

    private static MaterielDto MapToDto(Materiel m) => new()
    {
        Id                  = m.Id,
        CodeBarre           = m.CodeBarre,
        Libelle             = m.Libelle,
        TypeMaterielId      = m.TypeMaterielId,
        TypeMaterielLibelle = m.TypeMateriel?.Libelle ?? string.Empty,
        Marque              = m.Marque,
        Modele              = m.Modele,
        NumeroSerie         = m.NumeroSerie,
        DateAcquisition     = m.DateAcquisition,
        EtatId              = m.EtatId,
        EtatLibelle         = m.Etat?.Libelle ?? string.Empty,
        LocalisationId      = m.LocalisationId,
        LocalisationNom     = m.Localisation?.Nom ?? string.Empty,
        EmployeId           = m.EmployeId,
        EmployeNomComplet   = m.Employe != null
                                ? $"{m.Employe.Prenom} {m.Employe.Nom}"
                                : null,
        Actif               = m.Actif,
        Commentaires        = m.Commentaires,
        DateCreation        = m.DateCreation,
        DateModification    = m.DateModification
    };
}
