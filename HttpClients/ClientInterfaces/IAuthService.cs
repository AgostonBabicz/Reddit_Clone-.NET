using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.DTOs;

namespace HttpClients.ClientInterfaces;

public interface IAuthService
{
    public Task LoginAsync(string username, string password);
    public Task LogoutAsync();  
    public Task RegisterAsync(UserCreationDto userCreationDto);
    public Task<ClaimsPrincipal> GetAuthAsync();

    public Action<ClaimsPrincipal> OnAuthStateChanged { get; set; }
}