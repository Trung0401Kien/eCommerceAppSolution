using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;

namespace eCommerceApp.Application.Services.Interface.Authentication
{
    public interface IAuthenticationService
    {
        Task<ServiceResponse> CreateUser(CreateUser createUser);
        Task<LoginResponse> LoginUser(LoginUser loginUser);
        Task<LoginResponse> ReviveToken(string refreshToken);
    }
}
