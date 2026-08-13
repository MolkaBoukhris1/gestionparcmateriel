using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.DTOs;

public class EmployeDto
{
    public int Id { get; set; }
    public string Matricule { get; set; } = string.Empty;
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Service { get; set; }
    public string? Poste { get; set; }
    public string NomComplet => $"{Prenom} {Nom}";
}

public class EmployeCreateDto
{
    [Required][MaxLength(20)]
    public string Matricule { get; set; } = string.Empty;

    [Required][MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required][MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [MaxLength(150)][EmailAddress]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? Service { get; set; }

    [MaxLength(100)]
    public string? Poste { get; set; }
}

public class EmployeUpdateDto
{
    [Required][MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required][MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [MaxLength(150)][EmailAddress]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? Service { get; set; }

    [MaxLength(100)]
    public string? Poste { get; set; }
}
