using GestionParcMateriel.API.Models;

namespace GestionParcMateriel.API.Repositories;

public interface ITypeMaterielRepository
{
    Task<IEnumerable<TypeMateriel>> GetAllAsync();
    Task<TypeMateriel?> GetByIdAsync(int id);
    Task<TypeMateriel> AddAsync(TypeMateriel entity);
    Task<TypeMateriel?> UpdateAsync(TypeMateriel entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByLibelleAsync(string libelle, int? excludeId = null);
}

public interface IEtatMaterielRepository
{
    Task<IEnumerable<EtatMateriel>> GetAllAsync();
    Task<EtatMateriel?> GetByIdAsync(int id);
    Task<EtatMateriel> AddAsync(EtatMateriel entity);
    Task<EtatMateriel?> UpdateAsync(EtatMateriel entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByLibelleAsync(string libelle, int? excludeId = null);
}

public interface ILocalisationRepository
{
    Task<IEnumerable<Localisation>> GetAllAsync();
    Task<Localisation?> GetByIdAsync(int id);
    Task<Localisation> AddAsync(Localisation entity);
    Task<Localisation?> UpdateAsync(Localisation entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNomAsync(string nom, int? excludeId = null);
}

public interface IEmployeRepository
{
    Task<IEnumerable<Employe>> GetAllAsync(string? search = null);
    Task<Employe?> GetByIdAsync(int id);
    Task<Employe> AddAsync(Employe entity);
    Task<Employe?> UpdateAsync(Employe entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByMatriculeAsync(string matricule, int? excludeId = null);
}
