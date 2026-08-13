using GestionParcMateriel.API.DTOs;

namespace GestionParcMateriel.API.Services;

public interface IMaterielService
{
    Task<IEnumerable<MaterielDto>> GetAllAsync(string? search = null, int? etatId = null, int? typeId = null, bool includeInactif = false);
    Task<MaterielDto?> GetByIdAsync(int id);
    Task<(MaterielDto? Result, string? Error)> CreateAsync(MaterielCreateDto dto);
    Task<(MaterielDto? Result, string? Error)> UpdateAsync(int id, MaterielUpdateDto dto);
    Task<(bool Success, string? Error)> DeleteAsync(int id);
}
