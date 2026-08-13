using GestionParcMateriel.API.Data;
using GestionParcMateriel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Repositories;

public class MaterielRepository : IMaterielRepository
{
    private readonly ApplicationDbContext _context;

    public MaterielRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Materiel>> GetAllAsync(
        string? search = null,
        int? etatId = null,
        int? typeId = null,
        bool includeInactif = false)
    {
        var query = _context.Materiels
            .Include(m => m.TypeMateriel)
            .Include(m => m.Etat)
            .Include(m => m.Localisation)
            .Include(m => m.Employe)
            .AsQueryable();

        // Filtre actif / inactif
        if (!includeInactif)
            query = query.Where(m => m.Actif);

        // Filtre par état
        if (etatId.HasValue)
            query = query.Where(m => m.EtatId == etatId.Value);

        // Filtre par type
        if (typeId.HasValue)
            query = query.Where(m => m.TypeMaterielId == typeId.Value);

        // Recherche textuelle sur libellé, code-barres, marque, modèle, numéro de série
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(m =>
                m.Libelle.ToLower().Contains(term) ||
                m.CodeBarre.ToLower().Contains(term) ||
                (m.Marque != null && m.Marque.ToLower().Contains(term)) ||
                (m.Modele != null && m.Modele.ToLower().Contains(term)) ||
                (m.NumeroSerie != null && m.NumeroSerie.ToLower().Contains(term)));
        }

        return await query
            .OrderBy(m => m.Libelle)
            .ToListAsync();
    }

    public async Task<Materiel?> GetByIdAsync(int id)
    {
        return await _context.Materiels
            .Include(m => m.TypeMateriel)
            .Include(m => m.Etat)
            .Include(m => m.Localisation)
            .Include(m => m.Employe)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Materiel> AddAsync(Materiel materiel)
    {
        materiel.DateCreation = DateTime.UtcNow;
        materiel.DateModification = DateTime.UtcNow;
        _context.Materiels.Add(materiel);
        await _context.SaveChangesAsync();
        return materiel;
    }

    public async Task<Materiel> UpdateAsync(Materiel materiel)
    {
        materiel.DateModification = DateTime.UtcNow;
        _context.Materiels.Update(materiel);
        await _context.SaveChangesAsync();
        return materiel;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var materiel = await _context.Materiels.FindAsync(id);
        if (materiel == null) return false;

        materiel.Actif = false;
        materiel.DateModification = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? excludeId = null)
    {
        var query = _context.Materiels
            .Where(m => m.NumeroSerie != null && m.NumeroSerie == numeroSerie);

        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> ExistsByCodeBarreAsync(string codeBarre, int? excludeId = null)
    {
        var query = _context.Materiels
            .Where(m => m.CodeBarre == codeBarre);

        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}
