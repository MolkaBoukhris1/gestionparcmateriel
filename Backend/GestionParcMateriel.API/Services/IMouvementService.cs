using GestionParcMateriel.API.DTOs;

namespace GestionParcMateriel.API.Services;

public interface IMouvementService
{
    /// <summary>Affecte un matériel en stock à un employé. Règles : état = En stock.</summary>
    Task<(MouvementMaterielDto? Result, string? Error)> AffecterAsync(int materielId, AffecterMaterielDto dto);

    /// <summary>Retourne un matériel affecté au stock. Règles : état = Affecté.</summary>
    Task<(MouvementMaterielDto? Result, string? Error)> RetournerAsync(int materielId, RetourMaterielDto dto);

    /// <summary>Transfère un matériel vers une nouvelle localisation.</summary>
    Task<(MouvementMaterielDto? Result, string? Error)> TransfererAsync(int materielId, TransfertMaterielDto dto);

    /// <summary>Retourne l'historique des mouvements d'un matériel.</summary>
    Task<IEnumerable<MouvementMaterielDto>> GetHistoriqueAsync(int materielId);
}
