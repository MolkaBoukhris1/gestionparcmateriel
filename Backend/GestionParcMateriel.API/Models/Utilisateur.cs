using System.ComponentModel.DataAnnotations;

namespace GestionParcMateriel.API.Models
{
    public class Utilisateur
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string MotDePasseHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Role { get; set; } = "Utilisateur"; // "Admin" ou "Utilisateur"

        [MaxLength(100)]
        public string? NomComplet { get; set; }
    }
}