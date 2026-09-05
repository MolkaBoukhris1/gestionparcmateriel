using GestionParcMateriel.API.Data;
using GestionParcMateriel.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestionParcMateriel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InsightsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAiAnalysisService _aiService;

        public InsightsController(ApplicationDbContext context, IAiAnalysisService aiService)
        {
            _context = context;
            _aiService = aiService;
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
                .Include(m => m.TypeMateriel)
                .Where(m => m.Actif)
                .ToListAsync();

            var tousMouvements = await _context.MouvementsMateriels
                .Include(m => m.Materiel)
                .OrderByDescending(m => m.DateMouvement)
                .ToListAsync();

            // Règle 1 : matériel en panne/maintenance depuis longtemps
            foreach (var m in materiels.Where(m => m.Etat?.Libelle == "Panne" || m.Etat?.Libelle == "En maintenance"))
            {
                var joursDepuis = (maintenant - m.DateModification).Days;
                if (joursDepuis >= 7)
                {
                    insights.Add(new
                    {
                        type = "warning",
                        icone = "⚠️",
                        titre = $"{m.Libelle} ({m.CodeBarre})",
                        message = $"En état \"{m.Etat?.Libelle}\" depuis {joursDepuis} jours. Intervention recommandée."
                    });
                }
            }

            // Règle 2 : matériel affecté longtemps sans mouvement
            foreach (var m in materiels.Where(m => m.Etat?.Libelle == "Affecté"))
            {
                var dernierMouvement = tousMouvements.FirstOrDefault(mv => mv.MaterielId == m.Id);
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
            foreach (var m in materiels.Where(m => m.Etat?.Libelle == "En stock"))
            {
                var aDejaEuMouvement = tousMouvements.Any(mv => mv.MaterielId == m.Id);
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

            // Règle 4 : localisation avec beaucoup de matériel en panne/HS
            var localisationsProblematiques = materiels
                .Where(m => (m.Etat?.Libelle == "Panne" || m.Etat?.Libelle == "HS") && m.Localisation != null)
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

            // Règle 5 : matériel Rebut mais toujours localisé (à archiver)
            var rebuts = materiels.Where(m => m.Etat?.Libelle == "Rebut").ToList();
            if (rebuts.Any())
            {
                insights.Add(new
                {
                    type = "secondary",
                    icone = "🗑️",
                    titre = "Matériel en Rebut",
                    message = $"{rebuts.Count} matériel(s) marqué(s) Rebut sont toujours dans le parc actif. Pensez à les archiver (Actif = false)."
                });
            }

            // Règle 6 : employé avec beaucoup de matériel affecté (charge élevée)
            var employesCharges = materiels
                .Where(m => m.Employe != null)
                .GroupBy(m => new { m.EmployeId, Nom = m.Employe!.Nom, Prenom = m.Employe!.Prenom })
                .Where(g => g.Count() >= 3)
                .Select(g => new { nom = $"{g.Key.Prenom} {g.Key.Nom}", count = g.Count() });

            foreach (var e in employesCharges)
            {
                insights.Add(new
                {
                    type = "info",
                    icone = "👤",
                    titre = e.nom,
                    message = $"{e.count} matériels affectés à cet employé. Vérifier la cohérence des besoins."
                });
            }

            // Règle 7 : matériel sans numéro de série (traçabilité faible)
            var sansNumeroSerie = materiels.Where(m => string.IsNullOrWhiteSpace(m.NumeroSerie)).ToList();
            if (sansNumeroSerie.Count >= 3)
            {
                insights.Add(new
                {
                    type = "secondary",
                    icone = "🔍",
                    titre = "Traçabilité incomplète",
                    message = $"{sansNumeroSerie.Count} matériels n'ont pas de numéro de série renseigné. Recommandé pour un meilleur suivi."
                });
            }

            // Règle 8 : forte activité récente sur un matériel (mouvements rapprochés)
            var groupesMouvements = tousMouvements
                .Where(mv => (maintenant - mv.DateMouvement).Days <= 7)
                .GroupBy(mv => mv.MaterielId)
                .Where(g => g.Count() >= 3);

            foreach (var g in groupesMouvements)
            {
                var mat = materiels.FirstOrDefault(m => m.Id == g.Key);
                if (mat != null)
                {
                    insights.Add(new
                    {
                        type = "warning",
                        icone = "🔄",
                        titre = $"{mat.Libelle} ({mat.CodeBarre})",
                        message = $"{g.Count()} mouvements en 7 jours. Activité inhabituelle, à vérifier."
                    });
                }
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

        [HttpGet("ai-summary")]
        public async Task<IActionResult> GetAiSummary()
        {
            try
            {
                var materiels = await _context.Materiels
                    .Include(m => m.Etat)
                    .Include(m => m.Localisation)
                    .Where(m => m.Actif)
                    .ToListAsync();

                var parEtat = materiels
                    .GroupBy(m => m.Etat?.Libelle ?? "Non défini")
                    .Select(g => $"{g.Key}: {g.Count()}")
                    .ToList();

                var totalMouvements = await _context.MouvementsMateriels.CountAsync();

                var resume = $@"
Total matériel actif : {materiels.Count}
Répartition par état : {string.Join(", ", parEtat)}
Total mouvements enregistrés : {totalMouvements}
";

                var analyse = await _aiService.AnalyserParcAsync(resume);

                return Ok(new { analyse });
            }
            catch (Exception ex)
            {
                // Ne jamais renvoyer un 500 nu : on renvoie le message d'erreur exploitable
                return StatusCode(500, new { message = "Erreur lors de la génération de l'analyse.", detail = ex.Message });
            }
        }
    }
}