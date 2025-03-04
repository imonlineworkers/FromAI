using AS400WebInterface.Application.DTOs;
using AS400WebInterface.Application.Interfaces;
using AS400WebInterface.Application.ViewModels;
using AS400WebInterface.Domain.Common;
using AS400WebInterface.Domain.Entities;
using AutoMapper;

namespace AS400WebInterface.Infrastructure.Services
{
    public class UserService(IBaseHttpClient httpClient, IMapper mapper) : IUser
    {
        private readonly IBaseHttpClient _httpClient = httpClient;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseResponse<UserDto>?> AddUserAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return await _httpClient.PostAsync<UserDto>("users/add", user, cancellationToken);
        }

        public async Task<BaseResponse<UserDto>?> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _httpClient.DeleteAsync<UserDto>("users/delete", userId, cancellationToken);
        }

        public async Task<List<UserViewModel>> GetUserAsync(CancellationToken cancellationToken)
        {
            var users = await _httpClient.GetDataAsync<User>("users", cancellationToken);
            return _mapper.Map<List<UserViewModel>>(users);
        }

        public async Task<UserViewModel> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _httpClient.GetDataByIdAsync<UserViewModel>($"users/edit/{userId}", cancellationToken);
            return user ?? new UserViewModel();
        }

        public async Task<BaseResponse<UserDto>?> UpdateUserAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return await _httpClient.PutAsync<UserDto>("users/update", user, cancellationToken);
        }
    }
}
