using GestionParcMateriel.API.Models;

namespace GestionParcMateriel.API.Repositories;

public interface IMouvementRepository
{
    Task<IEnumerable<MouvementMateriel>> GetByMaterielIdAsync(int materielId);
    Task<MouvementMateriel> AddAsync(MouvementMateriel mouvement);
}
