using System.Text.Json.Serialization;

namespace CoworkingGes.Models
{
    public class Ressource
    {
        public int Id { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int QuantiteDisponible { get; set; }
        [JsonIgnore]
        public int UtilisateurId { get; set; }
        [JsonIgnore]
        public Utlisateur? Utilisateur { get; set; }

        public int EspaceId { get; set; }
        [JsonIgnore]
        public Espace? Espace { get; set; }
    }
}
