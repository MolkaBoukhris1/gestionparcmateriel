namespace GestionParcMateriel.API.DTOs;

/// <summary>DTO de lecture — expose les libellés lisibles au lieu des IDs bruts.</summary>
public class MaterielDto
{
    public int Id { get; set; }
    public string CodeBarre { get; set; } = string.Empty;
    public string Libelle { get; set; } = string.Empty;

    // Type
    public int TypeMaterielId { get; set; }
    public string TypeMaterielLibelle { get; set; } = string.Empty;

    public string? Marque { get; set; }
    public string? Modele { get; set; }
    public string? NumeroSerie { get; set; }
    public DateTime DateAcquisition { get; set; }

    // État
    public int EtatId { get; set; }
    public string EtatLibelle { get; set; } = string.Empty;

    // Localisation
    public int LocalisationId { get; set; }
    public string LocalisationNom { get; set; } = string.Empty;

    // Employé affecté (nullable)
    public int? EmployeId { get; set; }
    public string? EmployeNomComplet { get; set; }

    public bool Actif { get; set; }
    public string? Commentaires { get; set; }
    public DateTime DateCreation { get; set; }
    public DateTime DateModification { get; set; }
}
