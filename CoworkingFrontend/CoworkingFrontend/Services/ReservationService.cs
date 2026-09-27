using CoworkingFrontend.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoworkingBlazor.Services
{
    public interface IReservationService
    {
        Task<List<Reservation>> GetMyReservationsAsync();
        Task<List<Reservation>> GetAllReservationsAsync();
        Task<Reservation?> GetReservationAsync(int id);
        Task<List<Reservation>> GetReservationsByUserAsync(int userId);
        Task<int> GetAvailableCapacityAsync(int espaceId, DateTime debut, DateTime fin);
        Task<bool> CreateReservationAsync(Reservation reservation);
        Task<bool> UpdateReservationAsync(int id, Reservation reservation);
        Task<bool> DeleteReservationAsync(int id);
    }

    public class ReservationService : IReservationService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public ReservationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // =============================
        // GET MY RESERVATIONS
        // =============================
        public async Task<List<Reservation>> GetMyReservationsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Reservation>>(
                    "api/Reservation/my-reservations", _jsonOptions
                ) ?? new List<Reservation>();
            }
            catch
            {
                return new List<Reservation>();
            }
        }

        // =============================
        // GET ALL (ADMIN ONLY)
        // =============================
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Reservation>>("api/Reservation", _jsonOptions)
                    ?? new List<Reservation>();
            }
            catch
            {
                return new List<Reservation>();
            }
        }

        // =============================
        // GET BY ID (ADMIN ONLY)
        // =============================
        public async Task<Reservation?> GetReservationAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Reservation>($"api/Reservation/{id}", _jsonOptions);
            }
            catch
            {
                return null;
            }
        }

        // =============================
        // GET BY USER (ADMIN ONLY)
        // =============================
        public async Task<List<Reservation>> GetReservationsByUserAsync(int userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Reservation>>(
                    $"api/Reservation/user/{userId}", _jsonOptions
                ) ?? new List<Reservation>();
            }
            catch
            {
                return new List<Reservation>();
            }
        }

        // =============================
        // CHECK AVAILABLE CAPACITY
        // =============================
        public async Task<int> GetAvailableCapacityAsync(int espaceId, DateTime debut, DateTime fin)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<AvailableCapacityResponse>(
                    $"api/Reservation/espace/{espaceId}/available-capacity?debut={debut:o}&fin={fin:o}", _jsonOptions
                );

                return result?.AvailableCapacity ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private class AvailableCapacityResponse
        {
            public int AvailableCapacity { get; set; }
        }

        // =============================
        // CREATE (DTO)
        // =============================
        public async Task<bool> CreateReservationAsync(Reservation reservation)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Reservation", reservation, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // =============================
        // UPDATE (Reservation)
        // =============================
        public async Task<bool> UpdateReservationAsync(int id, Reservation reservation)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/Reservation/{id}", reservation, _jsonOptions);
                return response.IsSuccessStatusCode; // NoContent OK
            }
            catch
            {
                return false;
            }
        }

        // =============================
        // DELETE
        // =============================
        public async Task<bool> DeleteReservationAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Reservation/{id}");
                return response.IsSuccessStatusCode; // NoContent OK
            }
            catch
            {
                return false;
            }
        }
    }
}
