using CoworkingGes.Models;
using System.Text.Json.Serialization;

namespace CoworkingGes.DTO
{
    public class RessourceDTO
    {
        [JsonIgnore]
        public int UtilisateurId { get; set; }

        public int EspaceId { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int QuantiteDisponible { get; set; }
    }

}
