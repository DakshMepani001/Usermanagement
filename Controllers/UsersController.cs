using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Repositories;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    // GET: api/users
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<UserResponse>> GetAll()
    {
        // Avoid unnecessary materialization so large user lists are streamed at serialization time.
        var users = _repository.GetAll().Select(ToResponse);
        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserResponse> GetById(Guid id)
    {
        var user = _repository.GetById(id);
        if (user is null)
            return NotFound(new ProblemDetails { Title = "User not found", Detail = $"No user exists with id '{id}'.", Status = StatusCodes.Status404NotFound });

        return Ok(ToResponse(user));
    }

    // POST: api/users
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<UserResponse> Create([FromBody] UserUpsertRequest request)
    {
        if (request is null)
            return BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "Request body is required.", Status = StatusCodes.Status400BadRequest });

        // Normalize values so stored data is consistent (no leading/trailing spaces).
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone?.Trim()
        };

        _repository.Create(user);
        var response = ToResponse(user);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    // PUT: api/users/{id}
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<UserResponse> Update(Guid id, [FromBody] UserUpsertRequest request)
    {
        if (request is null)
            return BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "Request body is required.", Status = StatusCodes.Status400BadRequest });

        var existing = _repository.GetById(id);
        if (existing is null)
            return NotFound(new ProblemDetails { Title = "User not found", Detail = $"No user exists with id '{id}'.", Status = StatusCodes.Status404NotFound });

        var updated = new User
        {
            // Id is forced to match route Id.
            Id = id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone?.Trim()
        };

        _repository.Update(id, updated);
        return Ok(ToResponse(updated));
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        var existing = _repository.GetById(id);
        if (existing is null)
            return NotFound(new ProblemDetails { Title = "User not found", Detail = $"No user exists with id '{id}'.", Status = StatusCodes.Status404NotFound });

        _repository.Delete(id);
        return NoContent();
    }

    private static UserResponse ToResponse(User user)
        => new(user.Id, user.FirstName, user.LastName, user.Email, user.Phone);
}

