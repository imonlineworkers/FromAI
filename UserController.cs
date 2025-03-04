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
