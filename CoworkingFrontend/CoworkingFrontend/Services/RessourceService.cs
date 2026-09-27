using CoworkingFrontend.Models;
using System.Net.Http.Json;

namespace CoworkingBlazor.Services
{
    public interface IRessourceService
    {
        Task<List<Ressource>> GetAllRessourcesAsync();
        Task<Ressource?> GetRessourceAsync(int id);
        Task<List<Ressource>> GetRessourcesByEspaceAsync(int espaceId);
        Task<bool> CreateRessourceAsync(Ressource ressource);
        Task<bool> UpdateRessourceAsync(int id, Ressource ressource);
        Task<bool> DeleteRessourceAsync(int id);
    }

    public class RessourceService : IRessourceService
    {
        private readonly HttpClient _httpClient;

        public RessourceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =============================
        // GET ALL (Admin only)
        // =============================
        public async Task<List<Ressource>> GetAllRessourcesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Ressource>>("api/Ressource")
                    ?? new List<Ressource>();
            }
            catch
            {
                return new List<Ressource>();
            }
        }

        // =============================
        // GET BY ID (Admin only)
        // =============================
        public async Task<Ressource?> GetRessourceAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Ressource>($"api/Ressource/{id}");
            }
            catch
            {
                return null;
            }
        }

        // =============================
        // GET BY ESPACE (Admin only)
        // =============================
        public async Task<List<Ressource>> GetRessourcesByEspaceAsync(int espaceId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Ressource>>($"api/Ressource/espace/{espaceId}")
                    ?? new List<Ressource>();
            }
            catch
            {
                return new List<Ressource>();
            }
        }

        // =============================
        // CREATE (RessourceDTO)
        // =============================
        public async Task<bool> CreateRessourceAsync(Ressource ressource)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Ressource", ressource);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // =============================
        // UPDATE
        // =============================
        public async Task<bool> UpdateRessourceAsync(int id, Ressource ressource)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Ressource/{id}", ressource);
                return response.IsSuccessStatusCode; // NoContent
            }
            catch
            {
                return false;
            }
        }

        // =============================
        // DELETE
        // =============================
        public async Task<bool> DeleteRessourceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Ressource/{id}");
                return response.IsSuccessStatusCode; // NoContent
            }
            catch
            {
                return false;
            }
        }
    }
}
