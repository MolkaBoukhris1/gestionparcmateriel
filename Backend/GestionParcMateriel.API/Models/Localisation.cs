using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.Models;

public class Localisation
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nom { get; set; } = string.Empty;

    // Navigation
    public ICollection<Materiel> Materiels { get; set; } = new List<Materiel>();
    public ICollection<MouvementMateriel> Mouvements { get; set; } = new List<MouvementMateriel>();
}
