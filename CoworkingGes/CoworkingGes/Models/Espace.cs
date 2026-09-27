using System.Text.Json.Serialization;

namespace CoworkingGes.Models
{
    public class Espace
    {
        public int Id { get; set; }
        public string Nom { get; set; } 
        public string Type { get; set; } 
        public int Capacite { get; set; }
        public string Localisation { get; set; } = string.Empty;
        public string Etat { get; set; } = "Disponible";
        [JsonIgnore]
        public int UtilisateurId { get; set; }
        [JsonIgnore]
        public Utlisateur? Utilisateur { get; set; }

        public ICollection<Abonnement>? Abonnements { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Ressource>? Ressources { get; set; }
        public ICollection<Maintenance>? Maintenances { get; set; }
    }
}
