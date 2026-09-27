using CoworkingFrontend.Models;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace CoworkingBlazor.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string email, string password);
        Task LogoutAsync();
        Task<string> GetTokenAsync();
        Task<string> GetRoleAsync();
        Task<string> GetUsernameAsync();
        bool IsAuthenticated { get; }
        Task<List<Utilisateur>> GetAllTechniciens();
        Task<List<Utilisateur>> GetAllUsers();  // ✅ Add this line
        Task<bool> ChangeUserRoleAsync(int userId, UserRole newRole);


        Task InitializeAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
        };

        private string _token = string.Empty;
        private string _role = string.Empty;
        private string _username = string.Empty;

        public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

        public AuthService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }
        public async Task<bool> ChangeUserRoleAsync(int userId, UserRole newRole)
        {
            try
            {
                var dto = new { newRole = newRole.ToString() };
                var response = await _httpClient.PutAsJsonAsync($"api/Account/change-role/{userId}", dto);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


        public async Task InitializeAsync()
        {
            try
            {
                var token = await _js.InvokeAsync<string>("localStorage.getItem", "cw_token");
                var role = await _js.InvokeAsync<string>("localStorage.getItem", "cw_role");
                var username = await _js.InvokeAsync<string>("localStorage.getItem", "cw_username");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _token = token;
                    _role = role ?? string.Empty;
                    _username = username ?? string.Empty;
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                }
            }
            catch
            {
                // ignore
            }
        }
        public async Task<List<Utilisateur>> GetAllUsers()
        {
            try
            {
                var list = await _httpClient.GetFromJsonAsync<List<UserDto>>("api/Account/all-users", _jsonOptions);
                if (list != null && list.Count > 0)
                {
                    var mapped = list.Select(d => new Utilisateur
                    {
                        Id = d.id,
                        UserName = d.userName,
                        Email = d.email,
                        Role = Enum.TryParse<UserRole>(d.role, true, out var r) ? r : UserRole.Etudiant
                    }).ToList();

                    return mapped;
                }

                return new List<Utilisateur>();
            }
            catch
            {
                return new List<Utilisateur>();
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Account/login",
                    new { username, password });

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    _token = root.GetProperty("token").GetString();
                    _role = root.GetProperty("role").GetString();
                    _username = username;

                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                    // persist
                    try
                    {
                        await _js.InvokeVoidAsync("localStorage.setItem", "cw_token", _token);
                        await _js.InvokeVoidAsync("localStorage.setItem", "cw_role", _role ?? string.Empty);
                        await _js.InvokeVoidAsync("localStorage.setItem", "cw_username", _username ?? string.Empty);
                    }
                    catch
                    {
                        // ignore storage errors
                    }

                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Account/register",
                    new { username, emailAddress = email, password });
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }


        public async Task LogoutAsync()
        {
            _token = string.Empty;
            _role = string.Empty;
            _username = string.Empty;
            _httpClient.DefaultRequestHeaders.Authorization = null;
            try
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", "cw_token");
                await _js.InvokeVoidAsync("localStorage.removeItem", "cw_role");
                await _js.InvokeVoidAsync("localStorage.removeItem", "cw_username");
            }
            catch
            {
                // ignore
            }
        }

        public Task<string> GetTokenAsync() => Task.FromResult(_token);
        public Task<string> GetRoleAsync() => Task.FromResult(_role);
        public Task<string> GetUsernameAsync() => Task.FromResult(_username);

        // DTO used to deserialize backend user objects (role as string)
        private class UserDto
        {
            public int id { get; set; }
            public string userName { get; set; }
            public string email { get; set; }
            public string role { get; set; }
        }

        public async Task<List<Utilisateur>> GetAllTechniciens()
        {
            try
            {
                var endpoints = new[] {"api/Account/techniciens"};
                foreach (var ep in endpoints)
                {
                    try
                    {
                        var list = await _httpClient.GetFromJsonAsync<List<UserDto>>(ep, _jsonOptions);
                        if (list != null && list.Count > 0)
                        {
                            var mapped = list.Select(d => new Utilisateur
                            {
                                Id = d.id,
                                UserName = d.userName,
                                Email = d.email,
                                Role = Enum.TryParse<UserRole>(d.role, true, out var r) ? r : UserRole.Etudiant
                            }).ToList();

                            var technicians = mapped.Where(u => u.Role == UserRole.Technicien).ToList();
                            if (technicians.Count > 0) return technicians;

                            // if role parsing didn't produce techniciens, return mapped list
                            return mapped;
                        }
                    }
                    catch
                    {
                        // ignore and try next endpoint
                    }
                }

                return new List<Utilisateur>();
            }
            catch
            {
                return new List<Utilisateur>();
            }
        }

       
    }
}