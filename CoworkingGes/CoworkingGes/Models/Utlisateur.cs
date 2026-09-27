using CoworkingGes.Enum;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace CoworkingGes.Models
{
    public class Utlisateur : IdentityUser<int>
    {
        public UserRole Role { get; set; }
        public string? Matricule { get; set; } // pour étudiant
        public string? Specialite { get; set; } // pour technicien
        public string? Departement { get; set; } // pour admin
        [JsonIgnore]
        public ICollection<Reservation> Reservations { get; set; }
        [JsonIgnore]
        public ICollection<Abonnement> Abonnements { get; set; }
        [JsonIgnore]
        public ICollection<Maintenance> Maintenances { get; set; }

        [JsonIgnore]
        public ICollection<Ressource> Ressources { get; set; }
        public ICollection<Espace> Espaces { get; set; }

    }
}



