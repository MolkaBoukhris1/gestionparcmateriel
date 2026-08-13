using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.Models;

public class TypeMateriel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Libelle { get; set; } = string.Empty;

    // Navigation
    public ICollection<Materiel> Materiels { get; set; } = new List<Materiel>();
}
