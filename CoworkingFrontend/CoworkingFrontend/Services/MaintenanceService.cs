using System.Net.Http.Json;
using CoworkingFrontend.Models;

namespace CoworkingBlazor.Services
{
    public interface IMaintenanceService
    {
        Task<List<Maintenance>> GetAllMaintenanceAsync();
        Task<Maintenance?> GetMaintenanceAsync(int id);
        Task<List<Maintenance>> GetMaintenanceByEspaceAsync(int espaceId);
        Task<List<Maintenance>> GetMyMaintenancesAsync();
        Task<bool> CreateMaintenanceAsync(Maintenance maintenance);
        Task<bool> UpdateMaintenanceAsync(int id, Maintenance maintenance);
        Task<bool> DeleteMaintenanceAsync(int id);
    }

    public class MaintenanceService : IMaintenanceService
    {
        private readonly HttpClient _httpClient;

        public MaintenanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Maintenance>> GetAllMaintenanceAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Maintenance>>("api/Maintenance")
                    ?? new List<Maintenance>();
            }
            catch
            {
                return new List<Maintenance>();
            }
        }

        public async Task<Maintenance?> GetMaintenanceAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Maintenance>($"api/Maintenance/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Maintenance>> GetMaintenanceByEspaceAsync(int espaceId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Maintenance>>($"api/Maintenance/espace/{espaceId}")
                    ?? new List<Maintenance>();
            }
            catch
            {
                return new List<Maintenance>();
            }
        }

        public async Task<List<Maintenance>> GetMyMaintenancesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Maintenance>>("api/Maintenance/technicien/my-maintenances")
                    ?? new List<Maintenance>();
            }
            catch
            {
                return new List<Maintenance>();
            }
        }

        public async Task<bool> CreateMaintenanceAsync(Maintenance maintenance)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Maintenance", maintenance);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateMaintenanceAsync(int id, Maintenance maintenance)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Maintenance/{id}", maintenance);
                return response.IsSuccessStatusCode; // NoContent = OK
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteMaintenanceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Maintenance/{id}");
                return response.IsSuccessStatusCode; // NoContent = OK
            }
            catch
            {
                return false;
            }
        }
    }
}
