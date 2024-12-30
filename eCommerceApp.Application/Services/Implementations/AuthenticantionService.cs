using AutoMapper;
using eCommerceApp.Application.DTOs;
using eCommerceApp.Application.DTOs.Identity;
using eCommerceApp.Application.Services.Interface;
using eCommerceApp.Application.Services.Interface.Authentication;
using eCommerceApp.Application.Services.Interface.Logging;
using eCommerceApp.Application.Validations;
using eCommerceApp.Domain.Entities.Identity;
using eCommerceApp.Domain.Interfaces.Authentication;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace eCommerceApp.Application.Services.Implementations
{
    public class AuthenticantionService(ITokenManagement tokenManagement, IUserManagement userManagement,
        IRoleManagement roleManagement, IAppLogger<AuthenticantionService> logger,
        IMapper mapper, IValidator<CreateUser> createUserValidator, IValidator<LoginUser> loginUserValidator,
        IValidationService validationService, ICacheService cacheService, IHubContext<SessionHub> signalRHubContext) : IAuthenticationService
    {
        public async Task<ServiceResponse> CreateUser(CreateUser createUser)
        {
            var _validationResult = await validationService.ValidateAsync(createUser, createUserValidator);
            if (!_validationResult.Success) return _validationResult;

            var mapperModel = mapper.Map<AppUser>(createUser);
            mapperModel.UserName = createUser.Email;
            mapperModel.PasswordHash = createUser.Password;

            var result = await userManagement.CreateUser(mapperModel);
            if (!result)
                return new ServiceResponse { Message = "Email Address might be already in use or unknow error occurred" };

            var _user = await userManagement.GetUserByEmail(createUser.Email);
            var user = await userManagement.GetAllUsers();
            bool assignedResult = await roleManagement.AddUserrole(_user!, user!.Count() > 1 ? "User" : "Admin");

            if (!assignedResult)
            {
                int removeUserResult = await userManagement.RemoveUserByEmail(_user!.Email!);
                if (removeUserResult <= 0)
                {
                    logger.LogError(new Exception($"User with Email at {_user.Email} failed to be remove as a result of role assigning issue"), "User could not be assigned Role");
                    return new ServiceResponse { Message = "Error occurred in creating account" };
                }
            }
            return new ServiceResponse { Success = true, Message = "Account created" };
        }

        public async Task<LoginResponse> LoginUser(LoginUser loginUser)
        {
            var _validationResult = await validationService.ValidateAsync(loginUser, loginUserValidator);
            if (!_validationResult.Success)
                return new LoginResponse(Message: _validationResult.Message);

            var mapperModel = mapper.Map<AppUser>(loginUser);
            mapperModel.PasswordHash = loginUser.Password;

            bool loginResult = await userManagement.LoginUser(mapperModel);
            if (!loginResult)
                return new LoginResponse(Message: "Email not found or invalid credentials");

            var _user = await userManagement.GetUserByEmail(loginUser.Email);
            var claims = await userManagement.GetUserClaims(loginUser.Email);

            var sessionId = Guid.NewGuid().ToString();
            string jwtoken = tokenManagement.GenerateToken(claims, sessionId);
            string refreshToken = tokenManagement.GetRefreshToken();

            bool isSessionSaved = await cacheService.CreateNewSessionAsync(_user.Id.ToString(), sessionId);

            if (!isSessionSaved)
                return new LoginResponse(Message: "Failed to create session");

            int saveTokenResult = 0;
            bool userTokenCheck = await tokenManagement.ValidateRefreshToken(refreshToken);
            if (userTokenCheck)
                saveTokenResult = await tokenManagement.UpdateRefreshToken(_user.Id, refreshToken);
            else
                saveTokenResult = await tokenManagement.AddRefreshToken(_user.Id, refreshToken);

            return saveTokenResult <= 0 ? new LoginResponse(Message: "Internal error occurred while authenticating") :
                new LoginResponse(Success: true, Token: jwtoken, RefreshToken: refreshToken, SessionId: sessionId);
        }

        public async Task<LoginResponse> ReviveToken(string refreshToken)
        {
            bool validateTokenResult = await tokenManagement.ValidateRefreshToken(refreshToken);
            if (!validateTokenResult)
                return new LoginResponse(Message: "Invalid token");

            string userId = await tokenManagement.GetUserIdByRefreshToken(refreshToken);
            AppUser? user = await userManagement.GetUserById(userId);
            var claims = await userManagement.GetUserClaims(user!.Email!);

            string newJwtToken = tokenManagement.GenerateToken(claims, null);
            string newRefreshToken = tokenManagement.GetRefreshToken();
            await tokenManagement.UpdateRefreshToken(userId, newRefreshToken);

            return new LoginResponse(Success: true, Token: newJwtToken, RefreshToken: newRefreshToken);
        }
    }
}
