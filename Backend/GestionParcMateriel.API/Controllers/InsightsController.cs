using GestionParcMateriel.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsightsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InsightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetInsights()
        {
            var insights = new List<object>();
            var maintenant = DateTime.UtcNow;

            var materiels = await _context.Materiels
                .Include(m => m.Etat)
                .Include(m => m.Localisation)
                .Include(m => m.Employe)
                .Where(m => m.Actif)
                .ToListAsync();

            // Règle 1 : matériel en panne/maintenance depuis longtemps
            foreach (var m in materiels.Where(m => m.Etat!.Libelle == "Panne" || m.Etat!.Libelle == "En maintenance"))
            {
                var joursDepuis = (maintenant - m.DateModification).Days;
                if (joursDepuis >= 7)
                {
                    insights.Add(new
                    {
                        type = "warning",
                        icone = "⚠️",
                        titre = $"{m.Libelle} ({m.CodeBarre})",
                        message = $"En état \"{m.Etat!.Libelle}\" depuis {joursDepuis} jours. Intervention recommandée."
                    });
                }
            }

            // Règle 2 : matériel affecté longtemps sans mouvement
            var affectesAnciens = materiels.Where(m => m.Etat!.Libelle == "Affecté");
            foreach (var m in affectesAnciens)
            {
                var dernierMouvement = await _context.MouvementsMateriels
                    .Where(mv => mv.MaterielId == m.Id)
                    .OrderByDescending(mv => mv.DateMouvement)
                    .FirstOrDefaultAsync();

                if (dernierMouvement != null)
                {
                    var joursDepuis = (maintenant - dernierMouvement.DateMouvement).Days;
                    if (joursDepuis >= 180)
                    {
                        insights.Add(new
                        {
                            type = "info",
                            icone = "📌",
                            titre = $"{m.Libelle} ({m.CodeBarre})",
                            message = $"Affecté à {m.Employe?.Nom} {m.Employe?.Prenom} depuis {joursDepuis} jours sans mouvement. Vérification suggérée."
                        });
                    }
                }
            }

            // Règle 3 : matériel jamais mouvementé depuis création (stock dormant)
            foreach (var m in materiels.Where(m => m.Etat!.Libelle == "En stock"))
            {
                var aDejaEuMouvement = await _context.MouvementsMateriels
                    .AnyAsync(mv => mv.MaterielId == m.Id);

                var joursDepuisCreation = (maintenant - m.DateCreation).Days;

                if (!aDejaEuMouvement && joursDepuisCreation >= 30)
                {
                    insights.Add(new
                    {
                        type = "secondary",
                        icone = "📦",
                        titre = $"{m.Libelle} ({m.CodeBarre})",
                        message = $"En stock depuis {joursDepuisCreation} jours, jamais affecté. Stock potentiellement dormant."
                    });
                }
            }

            // Règle 4 : localisation avec beaucoup de matériel en panne
            var localisationsProblematiques = materiels
                .Where(m => m.Etat!.Libelle == "Panne" || m.Etat!.Libelle == "HS")
                .GroupBy(m => m.Localisation!.Nom)
                .Where(g => g.Count() >= 2)
                .Select(g => new { localisation = g.Key, count = g.Count() });

            foreach (var l in localisationsProblematiques)
            {
                insights.Add(new
                {
                    type = "danger",
                    icone = "🔴",
                    titre = l.localisation,
                    message = $"{l.count} matériels en panne/HS dans cette localisation. Zone à surveiller."
                });
            }

            if (insights.Count == 0)
            {
                insights.Add(new
                {
                    type = "success",
                    icone = "✅",
                    titre = "Tout va bien",
                    message = "Aucune alerte détectée. Le parc matériel est en bon état."
                });
            }

            return Ok(insights);
        }
    }
}