using CoworkingGes.Enum;
using System.Text.Json.Serialization;

namespace CoworkingGes.DTO
{
    public class ReservationDTO
    {
        [JsonIgnore]
        public int UtilisateurId { get; set; }
        public int EspaceId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Statut Statut { get; set; } = Statut.EnAttente; // Valeur par défaut
        public string? Notes { get; set; }
    }
}