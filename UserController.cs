using AS400IntegrationLayer.Application.Features.Users.Commands;
using AS400IntegrationLayer.Application.Features.Users.Queries;
using AS400IntegrationLayer.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AS400IntegrationLayer.API.Controllers
{
    [Route("api/users")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        [HttpPost("add-user")]
        public async Task<IActionResult> Create([FromBody] UserViewModel model)
        {
            var result = await _mediator.Send(new CreateUserCommand(model));
            return Ok(result);
        }

        [HttpDelete("delete-user")]
        public async Task<IActionResult> Delete([FromBody] Guid userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(userId));
            return Ok(result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(userId));
            return Ok(result);
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }

        [HttpPost("edit-user")]
        public async Task<IActionResult> Update([FromBody] Guid userId, [FromBody] UserViewModel model)
        {
            var result = await _mediator.Send(new UpdateUserCommand(userId, model));
            return Ok(result);
        }
    }
}
