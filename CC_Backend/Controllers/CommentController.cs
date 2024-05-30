using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models.DTOs;
using CC_Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.CommentRepo;
using CC_Backend.Services;
using FluentValidation;


[ApiController]
[Route("comment")]
public class CommentController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommentRepo _commentRepo;
    private readonly IStampsRepo _stampsRepo;
    private readonly ICommentService _commentService;
    private readonly IValidator<CommentCreateDTO> _commentValidator;

    public CommentController(UserManager<ApplicationUser> userManager, ICommentRepo commentRepo, IStampsRepo stampsRepo, ICommentService commentService, IValidator<CommentCreateDTO> commentValidator)
    {
        _userManager = userManager;
        _commentRepo = commentRepo;
        _stampsRepo = stampsRepo;
        _commentService = commentService;
        _commentValidator = commentValidator;
    }

    // Add new comment to a friends collected stamp in feed
    [HttpPost("addcomment")]
    [Authorize]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var validationResult = await _commentValidator.ValidateAsync(dto);
        if(!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            await _commentService.AddCommentAsync(dto, userId);
            return Ok("Comment added successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occured: {ex.Message}");
        }
    }

    // Update an already posted comment
    [HttpPut("updatecomment")]
    [Authorize]
    public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var comment = await _commentRepo.GetCommentByIdAsync(userId, dto.StampCollectedId);
        if (comment == null)
        {
            return NotFound();
        }

        comment.Content = dto.Content;
        await _commentRepo.UpdateCommentAsync(comment);

        return Ok(true);
    }

    // Delete a comment
    [HttpDelete("deletecomment")]
    [Authorize]
    public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var comment = await _commentRepo.GetCommentByIdAsync(userId, dto.StampCollectedId);
        if (comment == null)
        {
            return NotFound();
        }

        await _commentRepo.DeleteCommentAsync(comment.CommentId);

        return Ok(true);
    }
}