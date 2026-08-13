using GestionParcMateriel.API.Data;
using GestionParcMateriel.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Repositories;

public class MouvementRepository : IMouvementRepository
{
    private readonly ApplicationDbContext _ctx;
    public MouvementRepository(ApplicationDbContext ctx) => _ctx = ctx;

    public async Task<IEnumerable<MouvementMateriel>> GetByMaterielIdAsync(int materielId)
    {
        return await _ctx.MouvementsMateriels
            .Include(m => m.Materiel)
            .Include(m => m.Employe)
            .Include(m => m.Localisation)
            .Include(m => m.AncienEtat)
            .Include(m => m.NouvelEtat)
            .Where(m => m.MaterielId == materielId)
            .OrderByDescending(m => m.DateMouvement)
            .ToListAsync();
    }

    public async Task<MouvementMateriel> AddAsync(MouvementMateriel mouvement)
    {
        _ctx.MouvementsMateriels.Add(mouvement);
        await _ctx.SaveChangesAsync();
        return mouvement;
    }
}
