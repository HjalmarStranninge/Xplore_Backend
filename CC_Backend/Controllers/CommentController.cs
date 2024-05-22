using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CC_Backend.Models.Viewmodels;
using CC_Backend.Models.DTOs;
using CC_Backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommentRepo _commentRepository;

    public CommentController(UserManager<ApplicationUser> userManager, ICommentRepo commentRepository)
    {
        _userManager = userManager;
        _commentRepository = commentRepository;
    }

    // POST: Comment
    [HttpPost]
    [Authorize]
    [Route("/comment/addcomment")]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDTO commentDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var stampCollected = await _commentRepository.GetStampCollectedAsync(commentDto.StampCollectedId);
        if (stampCollected == null)
        {
            return BadRequest("The specified stamp collected does not exist.");
        }


        var comment = new Comment
        {
            StampCollectedId = commentDto.StampCollectedId,
            Content = commentDto.Content,
            StampCollected = stampCollected,
            UserId = userId

        };

        await _commentRepository.AddCommentAsync(comment);



        return Ok(true);
    }




    // PUT: Comment/{id}
    [HttpPut]
    [Authorize]
    [Route("/comment/updatecomment")]
    public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDTO updatedCommentDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var comment = await _commentRepository.GetCommentByIdAsync(userId, updatedCommentDto.StampCollectedId);
        if (comment == null)
        {
            return NotFound();
        }



        comment.Content = updatedCommentDto.Content;
        await _commentRepository.UpdateCommentAsync(comment);

        return Ok(true);
    }

    // DELETE: Comment/{id}
    [HttpDelete]
    [Authorize]
    [Route("/comment/deletecomment")]
    public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentDTO dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var comment = await _commentRepository.GetCommentByIdAsync(userId, dto.StampCollectedId);
        if (comment == null)
        {
            return NotFound();
        }



        await _commentRepository.DeleteCommentAsync(comment.CommentId);

        return Ok(true);
    }
}