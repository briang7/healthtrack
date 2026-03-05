namespace HealthTrack.Api.Controllers;

using HealthTrack.Application.Common.Models;
using HealthTrack.Application.Features.Auth.Commands.Login;
using HealthTrack.Application.Features.Auth.Commands.RefreshToken;
using HealthTrack.Application.Features.Auth.Commands.Register;
using HealthTrack.Application.Features.Auth.DTOs;
using Microsoft.AspNetCore.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/auth")]
[EnableRateLimiting("auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Login with email and password.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Refresh an expired access token using a refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }
}
