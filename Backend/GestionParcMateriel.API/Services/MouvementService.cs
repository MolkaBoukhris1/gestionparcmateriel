using GestionParcMateriel.API.DTOs;
using GestionParcMateriel.API.Models;
using GestionParcMateriel.API.Repositories;

namespace GestionParcMateriel.API.Services;

public class MouvementService : IMouvementService
{
    // IDs des états seed (correspondant à ApplicationDbContext.SeedData)
    private const int ETAT_EN_STOCK   = 1;
    private const int ETAT_AFFECTE    = 2;

    private readonly IMaterielRepository   _materielRepo;
    private readonly IMouvementRepository  _mouvementRepo;
    private readonly IEtatMaterielRepository _etatRepo;

    public MouvementService(
        IMaterielRepository materielRepo,
        IMouvementRepository mouvementRepo,
        IEtatMaterielRepository etatRepo)
    {
        _materielRepo  = materielRepo;
        _mouvementRepo = mouvementRepo;
        _etatRepo      = etatRepo;
    }

    // ── Affecter ──────────────────────────────────────────────────────────────
    public async Task<(MouvementMaterielDto? Result, string? Error)> AffecterAsync(
        int materielId, AffecterMaterielDto dto)
    {
        var materiel = await _materielRepo.GetByIdAsync(materielId);
        if (materiel == null)
            return (null, $"Matériel introuvable (Id={materielId}).");

        if (!materiel.Actif)
            return (null, "Ce matériel est désactivé, impossible de l'affecter.");

        // Règle métier 2 : seul un matériel "En stock" peut être affecté
        if (materiel.EtatId != ETAT_EN_STOCK)
            return (null, $"Le matériel doit être 'En stock' pour être affecté. État actuel : {materiel.Etat?.Libelle}.");

        // Règle métier 3 : ne peut pas être affecté à un autre sans retour préalable
        if (materiel.EmployeId.HasValue)
            return (null, "Ce matériel est déjà affecté. Effectuez d'abord un retour ou un transfert.");

        int ancienEtat = materiel.EtatId;

        // Mise à jour du matériel
        materiel.EmployeId       = dto.EmployeId;
        materiel.LocalisationId  = dto.LocalisationId;
        materiel.EtatId          = ETAT_AFFECTE;
        await _materielRepo.UpdateAsync(materiel);

        // Règle métier 4 : création automatique d'une ligne historique
        var mouvement = await _mouvementRepo.AddAsync(new MouvementMateriel
        {
            MaterielId     = materielId,
            DateMouvement  = DateTime.UtcNow,
            TypeMouvement  = TypeMouvement.Affectation,
            EmployeId      = dto.EmployeId,
            LocalisationId = dto.LocalisationId,
            AncienEtatId   = ancienEtat,
            NouvelEtatId   = ETAT_AFFECTE,
            Commentaires   = dto.Commentaires
        });

        return (await MapMouvementAsync(mouvement), null);
    }

    // ── Retour ────────────────────────────────────────────────────────────────
    public async Task<(MouvementMaterielDto? Result, string? Error)> RetournerAsync(
        int materielId, RetourMaterielDto dto)
    {
        var materiel = await _materielRepo.GetByIdAsync(materielId);
        if (materiel == null)
            return (null, $"Matériel introuvable (Id={materielId}).");

        // Règle métier 5 : doit être "Affecté"
        if (materiel.EtatId != ETAT_AFFECTE)
            return (null, $"Le matériel doit être 'Affecté' pour effectuer un retour. État actuel : {materiel.Etat?.Libelle}.");

        int ancienEtat      = materiel.EtatId;
        int? ancienEmploye  = materiel.EmployeId;

        // Libération de l'affectation
        materiel.EmployeId      = null;
        materiel.LocalisationId = dto.LocalisationId;
        materiel.EtatId         = ETAT_EN_STOCK;
        await _materielRepo.UpdateAsync(materiel);

        // Règle métier 5 : création mouvement Retour
        var mouvement = await _mouvementRepo.AddAsync(new MouvementMateriel
        {
            MaterielId     = materielId,
            DateMouvement  = DateTime.UtcNow,
            TypeMouvement  = TypeMouvement.Retour,
            EmployeId      = ancienEmploye,
            LocalisationId = dto.LocalisationId,
            AncienEtatId   = ancienEtat,
            NouvelEtatId   = ETAT_EN_STOCK,
            Commentaires   = dto.Commentaires
        });

        return (await MapMouvementAsync(mouvement), null);
    }

    // ── Transfert ─────────────────────────────────────────────────────────────
    public async Task<(MouvementMaterielDto? Result, string? Error)> TransfererAsync(
        int materielId, TransfertMaterielDto dto)
    {
        var materiel = await _materielRepo.GetByIdAsync(materielId);
        if (materiel == null)
            return (null, $"Matériel introuvable (Id={materielId}).");

        if (!materiel.Actif)
            return (null, "Ce matériel est désactivé, impossible de le transférer.");

        if (materiel.LocalisationId == dto.NouvelleLocalisationId)
            return (null, "La nouvelle localisation est identique à la localisation actuelle.");

        int ancienEtat = materiel.EtatId;

        // Règle métier 6 : modification de la localisation
        materiel.LocalisationId  = dto.NouvelleLocalisationId;
        await _materielRepo.UpdateAsync(materiel);

        // Règle métier 6 : création mouvement Transfert
        var mouvement = await _mouvementRepo.AddAsync(new MouvementMateriel
        {
            MaterielId     = materielId,
            DateMouvement  = DateTime.UtcNow,
            TypeMouvement  = TypeMouvement.Transfert,
            EmployeId      = materiel.EmployeId,
            LocalisationId = dto.NouvelleLocalisationId,
            AncienEtatId   = ancienEtat,
            NouvelEtatId   = ancienEtat,   // l'état ne change pas lors d'un transfert
            Commentaires   = dto.Commentaires
        });

        return (await MapMouvementAsync(mouvement), null);
    }

    // ── Historique ────────────────────────────────────────────────────────────
    public async Task<IEnumerable<MouvementMaterielDto>> GetHistoriqueAsync(int materielId)
    {
        var mouvements = await _mouvementRepo.GetByMaterielIdAsync(materielId);
        return mouvements.Select(MapMouvement);
    }

    // ── Mapping ───────────────────────────────────────────────────────────────
    private async Task<MouvementMaterielDto> MapMouvementAsync(MouvementMateriel m)
    {
        // Recharger depuis le repo pour avoir les navigations
        var mouvements = await _mouvementRepo.GetByMaterielIdAsync(m.MaterielId);
        var full = mouvements.FirstOrDefault(x => x.Id == m.Id);
        return full != null ? MapMouvement(full) : MapMouvement(m);
    }

    private static MouvementMaterielDto MapMouvement(MouvementMateriel m) => new()
    {
        Id                 = m.Id,
        MaterielId         = m.MaterielId,
        MaterielLibelle    = m.Materiel?.Libelle ?? string.Empty,
        DateMouvement      = m.DateMouvement,
        TypeMouvement      = m.TypeMouvement.ToString(),
        EmployeId          = m.EmployeId,
        EmployeNomComplet  = m.Employe != null ? $"{m.Employe.Prenom} {m.Employe.Nom}" : null,
        LocalisationId     = m.LocalisationId,
        LocalisationNom    = m.Localisation?.Nom ?? string.Empty,
        AncienEtatId       = m.AncienEtatId,
        AncienEtatLibelle  = m.AncienEtat?.Libelle ?? string.Empty,
        NouvelEtatId       = m.NouvelEtatId,
        NouvelEtatLibelle  = m.NouvelEtat?.Libelle ?? string.Empty,
        Commentaires       = m.Commentaires
    };
}
