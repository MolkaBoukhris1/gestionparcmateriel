using GestionParcMateriel.API.Models;

namespace GestionParcMateriel.API.Repositories;

public interface IMaterielRepository
{
    /// <summary>Retourne tous les matériels actifs avec leurs relations chargées.</summary>
    Task<IEnumerable<Materiel>> GetAllAsync(string? search = null, int? etatId = null, int? typeId = null, bool includeInactif = false);

    /// <summary>Retourne un matériel par son Id avec toutes les relations.</summary>
    Task<Materiel?> GetByIdAsync(int id);

    /// <summary>Ajoute un nouveau matériel.</summary>
    Task<Materiel> AddAsync(Materiel materiel);

    /// <summary>Met à jour un matériel existant.</summary>
    Task<Materiel> UpdateAsync(Materiel materiel);

    /// <summary>Suppression logique : met Actif = false.</summary>
    Task<bool> SoftDeleteAsync(int id);

    /// <summary>Vérifie si un numéro de série est déjà utilisé (hors Id exclu).</summary>
    Task<bool> ExistsByNumeroSerieAsync(string numeroSerie, int? excludeId = null);

    /// <summary>Vérifie si un code-barres est déjà utilisé (hors Id exclu).</summary>
    Task<bool> ExistsByCodeBarreAsync(string codeBarre, int? excludeId = null);
}
