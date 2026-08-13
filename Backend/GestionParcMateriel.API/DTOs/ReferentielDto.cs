using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.DTOs;

// ── Génériques (Id + Libelle) ─────────────────────────────────────────────────

public class TypeMaterielDto
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
}

public class TypeMaterielCreateDto
{
    [Required][MaxLength(50)]
    public string Libelle { get; set; } = string.Empty;
}

public class EtatMaterielDto
{
    public int Id { get; set; }
    public string Libelle { get; set; } = string.Empty;
}

public class EtatMaterielCreateDto
{
    [Required][MaxLength(50)]
    public string Libelle { get; set; } = string.Empty;
}

public class LocalisationDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
}

public class LocalisationCreateDto
{
    [Required][MaxLength(100)]
    public string Nom { get; set; } = string.Empty;
}
