using GestionParcMateriel.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var materiels = _context.Materiels.Where(m => m.Actif);

            var total = await materiels.CountAsync();

            var parEtat = await materiels
                .Include(m => m.Etat)
                .GroupBy(m => m.Etat!.Libelle)
                .Select(g => new { etat = g.Key, count = g.Count() })
                .ToListAsync();

            var derniersMouvements = await _context.MouvementsMateriels
                .Include(m => m.Materiel)
                .Include(m => m.Employe)
                .Include(m => m.Localisation)
                .OrderByDescending(m => m.DateMouvement)
                .Take(10)
                .Select(m => new
                {
                    m.Id,
                    materiel = m.Materiel!.Libelle,
                    type = m.TypeMouvement.ToString(),
                    employe = m.Employe != null ? m.Employe.Nom + " " + m.Employe.Prenom : null,
                    localisation = m.Localisation!.Nom,
                    date = m.DateMouvement
                })
                .ToListAsync();

            return Ok(new
            {
                totalMateriel = total,
                repartitionParEtat = parEtat,
                derniersMouvements = derniersMouvements
            });
        }
    }
}