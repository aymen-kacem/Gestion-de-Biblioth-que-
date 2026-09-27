using System.Text.Json.Serialization;

namespace CoworkingGes.DTO
{
    public class EspaceDTO
    {
        [JsonIgnore]
        public int UtilisateurId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacite { get; set; }
        public string Localisation { get; set; } = string.Empty;
    }
}
