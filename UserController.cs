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

