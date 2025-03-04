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
            return CreatedAtAction(nameof(GetById), new { userId = result.Data?.Id }, result);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(userId));
            return result.IsSuccess ? NoContent() : NotFound(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(userId));
            return result.IsSuccess ? Ok(result) : NotFound(result);
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

using AS400WebInterface.Application.Interfaces;
using AS400WebInterface.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AS400WebInterface.Web.Controllers
{
    public class UserController(IUser user) : Controller
    {
        private readonly IUser _user = user;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _user.GetUserAsync(cancellationToken: new CancellationToken());
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel model, CancellationToken cancellationToken)
        {
            var response = await _user.AddUserAsync(model, cancellationToken);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model, CancellationToken cancellationToken)
        {
            var response = await _user.UpdateUserAsync(model, cancellationToken);
            return Json(response);
        }

        [HttpGet("User/EditUser/{userId}")]
        public async Task<IActionResult> EditUser(Guid userId, CancellationToken cancellationToken)
        {

            var user = await _user.GetUserByIdAsync(userId, cancellationToken);
            return PartialView("_UserForm", user);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromBody] Guid userId, CancellationToken cancellationToken)
        {
            var response = await _user.DeleteUserAsync(userId, cancellationToken);
            return Json(response);
        }

    }
}

using AS400WebInterface.Application.Interfaces;
using AS400WebInterface.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AS400WebInterface.Web.Controllers
{
    [Route("users")]
    public class UserController(IUser user) : Controller
    {
        private readonly IUser _user = user;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var response = await _user.GetUserAsync(cancellationToken);
            return View(response);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _user.AddUserAsync(model, cancellationToken);
            return Json(response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _user.UpdateUserAsync(model, cancellationToken);
            return Json(response);
        }

        [HttpGet("edit/{userId}")]
        public async Task<IActionResult> EditUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _user.GetUserByIdAsync(userId, cancellationToken);
            return PartialView("_UserForm", user);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] Guid userId, CancellationToken cancellationToken)
        {
            var response = await _user.DeleteUserAsync(userId, cancellationToken);
            return Json(response);
        }
    }
}

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
            return await _httpClient
                .PostAsync<UserDto>("users/add-user", user, cancellationToken);
        }

        public async Task<BaseResponse<UserDto>?> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _httpClient
                .PostAsync<UserDto>($"users/delete-user/", userId, cancellationToken);
        }

        public async Task<List<UserViewModel>> GetUserAsync(CancellationToken cancellationToken)
        {
            var users = await _httpClient.GetDataAsync<User>("users/get-users", cancellationToken);
            return _mapper.Map<List<UserViewModel>>(users);
        }

        public async Task<UserViewModel> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var users = await _httpClient.GetDataByIdAsync<UserViewModel>($"users/{userId}", cancellationToken);
            return users ?? new UserViewModel();
        }

        public async Task<BaseResponse<UserDto>?> UpdateUserAsync(UserViewModel user, CancellationToken cancellationToken)
        {
            return await _httpClient
                .PostAsync<UserDto>($"users/edit-user/{user.UserId}", user, cancellationToken);
        }
    }
}


