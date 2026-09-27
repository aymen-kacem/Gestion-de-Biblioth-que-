using CoworkingFrontend.Models;
using System.Net.Http.Json;

namespace CoworkingBlazor.Services
{
    public interface IEspaceService
    {
        Task<List<Espace>> GetAllEspacesAsync();
        Task<Espace?> GetEspaceAsync(int id);
        Task<bool> CreateEspaceAsync(Espace espace);
        Task<bool> UpdateEspaceAsync(int id, Espace espace);
        Task<bool> DeleteEspaceAsync(int id);
    }

    public class EspaceService : IEspaceService
    {
        private readonly HttpClient _httpClient;

        public EspaceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Espace>> GetAllEspacesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Espace>>("api/Espace")
                       ?? new List<Espace>();
            }
            catch
            {
                return new List<Espace>();
            }
        }

        public async Task<Espace?> GetEspaceAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Espace>($"api/Espace/{id}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateEspaceAsync(Espace espace)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Espace", espace);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEspaceAsync(int id, Espace espace)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Espace/{id}", espace);
                return response.IsSuccessStatusCode; // NoContent = OK
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEspaceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Espace/{id}");
                return response.IsSuccessStatusCode; // NoContent = OK
            }
            catch
            {
                return false;
            }
        }
    }
}
