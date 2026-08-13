using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Models;
using GestionParcMateriel.API.Repositories;

namespace GestionParcMateriel.API.Services;

// ── Interfaces ────────────────────────────────────────────────────────────────

public interface ITypeMaterielService
{
    Task<IEnumerable<TypeMaterielDto>> GetAllAsync();
    Task<TypeMaterielDto?> GetByIdAsync(int id);
    Task<(TypeMaterielDto? Result, string? Error)> CreateAsync(TypeMaterielCreateDto dto);
    Task<(TypeMaterielDto? Result, string? Error)> UpdateAsync(int id, TypeMaterielCreateDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface IEtatMaterielService
{
    Task<IEnumerable<EtatMaterielDto>> GetAllAsync();
    Task<EtatMaterielDto?> GetByIdAsync(int id);
    Task<(EtatMaterielDto? Result, string? Error)> CreateAsync(EtatMaterielCreateDto dto);
    Task<(EtatMaterielDto? Result, string? Error)> UpdateAsync(int id, EtatMaterielCreateDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface ILocalisationService
{
    Task<IEnumerable<LocalisationDto>> GetAllAsync();
    Task<LocalisationDto?> GetByIdAsync(int id);
    Task<(LocalisationDto? Result, string? Error)> CreateAsync(LocalisationCreateDto dto);
    Task<(LocalisationDto? Result, string? Error)> UpdateAsync(int id, LocalisationCreateDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

public interface IEmployeService
{
    Task<IEnumerable<EmployeDto>> GetAllAsync(string? search = null);
    Task<EmployeDto?> GetByIdAsync(int id);
    Task<(EmployeDto? Result, string? Error)> CreateAsync(EmployeCreateDto dto);
    Task<(EmployeDto? Result, string? Error)> UpdateAsync(int id, EmployeUpdateDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}

// ── Implémentations ───────────────────────────────────────────────────────────

public class TypeMaterielService : ITypeMaterielService
{
    private readonly ITypeMaterielRepository _repo;
    public TypeMaterielService(ITypeMaterielRepository repo) => _repo = repo;

    public async Task<IEnumerable<TypeMaterielDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(t => new TypeMaterielDto { Id = t.Id, Libelle = t.Libelle });

    public async Task<TypeMaterielDto?> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? null : new TypeMaterielDto { Id = e.Id, Libelle = e.Libelle };
    }

    public async Task<(TypeMaterielDto? Result, string? Error)> CreateAsync(TypeMaterielCreateDto dto)
    {
        if (await _repo.ExistsByLibelleAsync(dto.Libelle))
            return (null, $"Le type '{dto.Libelle}' existe déjà.");
        var created = await _repo.AddAsync(new TypeMateriel { Libelle = dto.Libelle.Trim() });
        return (new TypeMaterielDto { Id = created.Id, Libelle = created.Libelle }, null);
    }

    public async Task<(TypeMaterielDto? Result, string? Error)> UpdateAsync(int id, TypeMaterielCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return (null, $"Type introuvable (Id={id}).");
        if (await _repo.ExistsByLibelleAsync(dto.Libelle, id))
            return (null, $"Le type '{dto.Libelle}' existe déjà.");
        entity.Libelle = dto.Libelle.Trim();
        await _repo.UpdateAsync(entity);
        return (new TypeMaterielDto { Id = entity.Id, Libelle = entity.Libelle }, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        return ok ? (true, null) : (false, $"Type introuvable (Id={id}).");
    }
}

public class EtatMaterielService : IEtatMaterielService
{
    private readonly IEtatMaterielRepository _repo;
    public EtatMaterielService(IEtatMaterielRepository repo) => _repo = repo;

    public async Task<IEnumerable<EtatMaterielDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(e => new EtatMaterielDto { Id = e.Id, Libelle = e.Libelle });

    public async Task<EtatMaterielDto?> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? null : new EtatMaterielDto { Id = e.Id, Libelle = e.Libelle };
    }

    public async Task<(EtatMaterielDto? Result, string? Error)> CreateAsync(EtatMaterielCreateDto dto)
    {
        if (await _repo.ExistsByLibelleAsync(dto.Libelle))
            return (null, $"L'état '{dto.Libelle}' existe déjà.");
        var created = await _repo.AddAsync(new EtatMateriel { Libelle = dto.Libelle.Trim() });
        return (new EtatMaterielDto { Id = created.Id, Libelle = created.Libelle }, null);
    }

    public async Task<(EtatMaterielDto? Result, string? Error)> UpdateAsync(int id, EtatMaterielCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return (null, $"État introuvable (Id={id}).");
        if (await _repo.ExistsByLibelleAsync(dto.Libelle, id))
            return (null, $"L'état '{dto.Libelle}' existe déjà.");
        entity.Libelle = dto.Libelle.Trim();
        await _repo.UpdateAsync(entity);
        return (new EtatMaterielDto { Id = entity.Id, Libelle = entity.Libelle }, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        return ok ? (true, null) : (false, $"État introuvable (Id={id}).");
    }
}

public class LocalisationService : ILocalisationService
{
    private readonly ILocalisationRepository _repo;
    public LocalisationService(ILocalisationRepository repo) => _repo = repo;

    public async Task<IEnumerable<LocalisationDto>> GetAllAsync() =>
        (await _repo.GetAllAsync()).Select(l => new LocalisationDto { Id = l.Id, Nom = l.Nom });

    public async Task<LocalisationDto?> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? null : new LocalisationDto { Id = e.Id, Nom = e.Nom };
    }

    public async Task<(LocalisationDto? Result, string? Error)> CreateAsync(LocalisationCreateDto dto)
    {
        if (await _repo.ExistsByNomAsync(dto.Nom))
            return (null, $"La localisation '{dto.Nom}' existe déjà.");
        var created = await _repo.AddAsync(new Localisation { Nom = dto.Nom.Trim() });
        return (new LocalisationDto { Id = created.Id, Nom = created.Nom }, null);
    }

    public async Task<(LocalisationDto? Result, string? Error)> UpdateAsync(int id, LocalisationCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return (null, $"Localisation introuvable (Id={id}).");
        if (await _repo.ExistsByNomAsync(dto.Nom, id))
            return (null, $"La localisation '{dto.Nom}' existe déjà.");
        entity.Nom = dto.Nom.Trim();
        await _repo.UpdateAsync(entity);
        return (new LocalisationDto { Id = entity.Id, Nom = entity.Nom }, null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        return ok ? (true, null) : (false, $"Localisation introuvable (Id={id}).");
    }
}

public class EmployeService : IEmployeService
{
    private readonly IEmployeRepository _repo;
    public EmployeService(IEmployeRepository repo) => _repo = repo;

    public async Task<IEnumerable<EmployeDto>> GetAllAsync(string? search = null) =>
        (await _repo.GetAllAsync(search)).Select(Map);

    public async Task<EmployeDto?> GetByIdAsync(int id)
    {
        var e = await _repo.GetByIdAsync(id);
        return e == null ? null : Map(e);
    }

    public async Task<(EmployeDto? Result, string? Error)> CreateAsync(EmployeCreateDto dto)
    {
        if (await _repo.ExistsByMatriculeAsync(dto.Matricule))
            return (null, $"Le matricule '{dto.Matricule}' est déjà utilisé.");
        var entity = new Employe
        {
            Matricule = dto.Matricule.Trim(),
            Nom       = dto.Nom.Trim(),
            Prenom    = dto.Prenom.Trim(),
            Email     = dto.Email?.Trim(),
            Service   = dto.Service?.Trim(),
            Poste     = dto.Poste?.Trim()
        };
        var created = await _repo.AddAsync(entity);
        return (Map(created), null);
    }

    public async Task<(EmployeDto? Result, string? Error)> UpdateAsync(int id, EmployeUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return (null, $"Employé introuvable (Id={id}).");
        entity.Nom     = dto.Nom.Trim();
        entity.Prenom  = dto.Prenom.Trim();
        entity.Email   = dto.Email?.Trim();
        entity.Service = dto.Service?.Trim();
        entity.Poste   = dto.Poste?.Trim();
        await _repo.UpdateAsync(entity);
        return (Map(entity), null);
    }

    public async Task<(bool Success, string? Error)> DeleteAsync(int id)
    {
        var ok = await _repo.DeleteAsync(id);
        return ok ? (true, null) : (false, $"Employé introuvable (Id={id}).");
    }

    private static EmployeDto Map(Employe e) => new()
    {
        Id        = e.Id,
        Matricule = e.Matricule,
        Nom       = e.Nom,
        Prenom    = e.Prenom,
        Email     = e.Email,
        Service   = e.Service,
        Poste     = e.Poste
    };
}
