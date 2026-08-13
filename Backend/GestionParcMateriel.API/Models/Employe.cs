using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.Models;

public class Employe
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Matricule { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Prenom { get; set; } = string.Empty;

    [MaxLength(150)]
    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? Service { get; set; }

    [MaxLength(100)]
    public string? Poste { get; set; }

    // Navigation
    public ICollection<Materiel> Materiels { get; set; } = new List<Materiel>();
    public ICollection<MouvementMateriel> Mouvements { get; set; } = new List<MouvementMateriel>();
}
