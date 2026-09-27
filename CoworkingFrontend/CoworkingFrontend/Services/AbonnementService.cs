using CoworkingFrontend.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoworkingBlazor.Services
{
    public interface IAbonnementService
    {
        Task<List<Abonnement>> GetMyAbonnementsAsync();
        Task<List<Abonnement>> GetAllAbonnementsAsync();
        Task<List<Abonnement>> GetAbonnementsByUserAsync(int userId);
        Task<List<Abonnement>> GetAbonnementsByEspaceAsync(int espaceId);
        Task<Abonnement> GetAbonnementAsync(int id);
        Task<Abonnement> CreateAbonnementAsync(Abonnement abonnement);
        Task<bool> UpdateAbonnementAsync(int id, Abonnement abonnement);
        Task<bool> DeleteAbonnementAsync(int id);
    }

    public class AbonnementService : IAbonnementService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public AbonnementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Get current user's abonnements
        /// </summary>
        public async Task<List<Abonnement>> GetMyAbonnementsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Abonnement>>("api/Abonnement/my-abonnements", _jsonOptions)
                    ?? new List<Abonnement>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting my abonnements: {ex.Message}");
                return new List<Abonnement>();
            }
        }

        /// <summary>
        /// Get all abonnements (Admin only)
        /// </summary>
        public async Task<List<Abonnement>> GetAllAbonnementsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Abonnement>>("api/Abonnement", _jsonOptions)
                    ?? new List<Abonnement>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all abonnements: {ex.Message}");
                return new List<Abonnement>();
            }
        }

        /// <summary>
        /// Get abonnements by user ID (Admin only)
        /// </summary>
        public async Task<List<Abonnement>> GetAbonnementsByUserAsync(int userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Abonnement>>($"api/Abonnement/user/{userId}", _jsonOptions)
                    ?? new List<Abonnement>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting abonnements by user: {ex.Message}");
                return new List<Abonnement>();
            }
        }

        /// <summary>
        /// Get abonnements by espace ID (Admin only)
        /// </summary>
        public async Task<List<Abonnement>> GetAbonnementsByEspaceAsync(int espaceId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Abonnement>>($"api/Abonnement/espace/{espaceId}", _jsonOptions)
                    ?? new List<Abonnement>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting abonnements by espace: {ex.Message}");
                return new List<Abonnement>();
            }
        }

        /// <summary>
        /// Get single abonnement by ID (Admin only)
        /// </summary>
        public async Task<Abonnement> GetAbonnementAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Abonnement>($"api/Abonnement/{id}", _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting abonnement: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Create new abonnement with automatic approval/rejection
        /// Returns the created abonnement with final status (Confirmé or Rejetée)
        /// </summary>
        public async Task<Abonnement> CreateAbonnementAsync(Abonnement abonnement)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Abonnement", abonnement, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Abonnement>(_jsonOptions);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error creating abonnement: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating abonnement: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Update abonnement
        /// </summary>
        public async Task<bool> UpdateAbonnementAsync(int id, Abonnement abonnement)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Abonnement/{id}", abonnement, _jsonOptions);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error updating abonnement: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating abonnement: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete abonnement
        /// </summary>
        public async Task<bool> DeleteAbonnementAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Abonnement/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting abonnement: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting abonnement: {ex.Message}");
                return false;
            }
        }
    }
}