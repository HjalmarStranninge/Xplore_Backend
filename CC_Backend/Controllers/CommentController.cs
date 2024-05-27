using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models.DTOs;
using CC_Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CC_Backend.Repositories.StampsRepo;
using CC_Backend.Repositories.CommentRepo;


[ApiController]
[Route("comment")]
public class CommentController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommentRepo _commentRepo;
    private readonly IStampsRepo _stampsRepo;

    public CommentController(UserManager<ApplicationUser> userManager, ICommentRepo commentRepo, IStampsRepo stampsRepo)
    {
        _userManager = userManager;
        _commentRepo = commentRepo;
        _stampsRepo = stampsRepo;
    }

    // Add new comment to a friends collected stamp in feed
    [HttpPost("addcomment")]
    [Authorize]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var stampCollected = await _stampsRepo.GetStampCollectedAsync(dto.StampCollectedId);
        if (stampCollected == null)
        {
            return BadRequest("The specified stamp collected does not exist.");
        }

        var now = DateTime.Now;
        var createdAt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

        var comment = new Comment
        {
            StampCollectedId = dto.StampCollectedId,
            Content = dto.Content,
            StampCollected = stampCollected,
            UserId = userId,
            CreatedAt = createdAt,
        };

        await _commentRepo.AddCommentAsync(comment);

        return Ok(true);
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