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

using AS400IntegrationLayer.Application.Features.Users.Commands;
using AS400IntegrationLayer.Application.Features.Users.Queries;
using AS400IntegrationLayer.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AS400IntegrationLayer.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserViewModel model)
        {
            var result = await _mediator.Send(new CreateUserCommand(model));
            return CreatedAtAction(nameof(GetById), new { userId = result.Data?.UserName }, result);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(userId));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(userId));
            return result.Equals(null) ? Ok(result) : NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UserViewModel model)
        {
            var result = await _mediator.Send(new UpdateUserCommand(userId, model));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
    }
}
