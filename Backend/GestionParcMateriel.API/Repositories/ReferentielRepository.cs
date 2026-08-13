using GestionParcMateriel.API.Data;
using GestionParcMateriel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Repositories;

// ── TypeMateriel ──────────────────────────────────────────────────────────────
public class TypeMaterielRepository : ITypeMaterielRepository
{
    private readonly ApplicationDbContext _ctx;
    public TypeMaterielRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<TypeMateriel>> GetAllAsync() =>
        await _ctx.TypesMateriels.OrderBy(t => t.Libelle).ToListAsync();

    public async Task<TypeMateriel?> GetByIdAsync(int id) =>
        await _ctx.TypesMateriels.FindAsync(id);

    public async Task<TypeMateriel> AddAsync(TypeMateriel entity)
    {
        _ctx.TypesMateriels.Add(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<TypeMateriel?> UpdateAsync(TypeMateriel entity)
    {
        _ctx.TypesMateriels.Update(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _ctx.TypesMateriels.FindAsync(id);
        if (entity == null) return false;
        _ctx.TypesMateriels.Remove(entity);
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByLibelleAsync(string libelle, int? excludeId = null)
    {
        var q = _ctx.TypesMateriels.Where(t => t.Libelle == libelle);
        if (excludeId.HasValue) q = q.Where(t => t.Id != excludeId.Value);
        return await q.AnyAsync();
    }
}

// ── EtatMateriel ──────────────────────────────────────────────────────────────
public class EtatMaterielRepository : IEtatMaterielRepository
{
    private readonly ApplicationDbContext _ctx;
    public EtatMaterielRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<EtatMateriel>> GetAllAsync() =>
        await _ctx.EtatsMateriels.OrderBy(e => e.Id).ToListAsync();

    public async Task<EtatMateriel?> GetByIdAsync(int id) =>
        await _ctx.EtatsMateriels.FindAsync(id);

    public async Task<EtatMateriel> AddAsync(EtatMateriel entity)
    {
        _ctx.EtatsMateriels.Add(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<EtatMateriel?> UpdateAsync(EtatMateriel entity)
    {
        _ctx.EtatsMateriels.Update(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _ctx.EtatsMateriels.FindAsync(id);
        if (entity == null) return false;
        _ctx.EtatsMateriels.Remove(entity);
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByLibelleAsync(string libelle, int? excludeId = null)
    {
        var q = _ctx.EtatsMateriels.Where(e => e.Libelle == libelle);
        if (excludeId.HasValue) q = q.Where(e => e.Id != excludeId.Value);
        return await q.AnyAsync();
    }
}

// ── Localisation ──────────────────────────────────────────────────────────────
public class LocalisationRepository : ILocalisationRepository
{
    private readonly ApplicationDbContext _ctx;
    public LocalisationRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Localisation>> GetAllAsync() =>
        await _ctx.Localisations.OrderBy(l => l.Nom).ToListAsync();

    public async Task<Localisation?> GetByIdAsync(int id) =>
        await _ctx.Localisations.FindAsync(id);

    public async Task<Localisation> AddAsync(Localisation entity)
    {
        _ctx.Localisations.Add(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<Localisation?> UpdateAsync(Localisation entity)
    {
        _ctx.Localisations.Update(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _ctx.Localisations.FindAsync(id);
        if (entity == null) return false;
        _ctx.Localisations.Remove(entity);
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNomAsync(string nom, int? excludeId = null)
    {
        var q = _ctx.Localisations.Where(l => l.Nom == nom);
        if (excludeId.HasValue) q = q.Where(l => l.Id != excludeId.Value);
        return await q.AnyAsync();
    }
}

// ── Employe ───────────────────────────────────────────────────────────────────
public class EmployeRepository : IEmployeRepository
{
    private readonly ApplicationDbContext _ctx;
    public EmployeRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<Employe>> GetAllAsync(string? search = null)
    {
        var q = _ctx.Employes.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            q = q.Where(e =>
                e.Nom.ToLower().Contains(term) ||
                e.Prenom.ToLower().Contains(term) ||
                e.Matricule.ToLower().Contains(term) ||
                (e.Service != null && e.Service.ToLower().Contains(term)));
        }
        return await q.OrderBy(e => e.Nom).ThenBy(e => e.Prenom).ToListAsync();
    }

    public async Task<Employe?> GetByIdAsync(int id) =>
        await _ctx.Employes.FindAsync(id);

    public async Task<Employe> AddAsync(Employe entity)
    {
        _ctx.Employes.Add(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<Employe?> UpdateAsync(Employe entity)
    {
        _ctx.Employes.Update(entity);
        await _ctx.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _ctx.Employes.FindAsync(id);
        if (entity == null) return false;
        _ctx.Employes.Remove(entity);
        await _ctx.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByMatriculeAsync(string matricule, int? excludeId = null)
    {
        var q = _ctx.Employes.Where(e => e.Matricule == matricule);
        if (excludeId.HasValue) q = q.Where(e => e.Id != excludeId.Value);
        return await q.AnyAsync();
    }
}
