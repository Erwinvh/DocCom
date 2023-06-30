using DocComAPI.Data;
using DocComAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace DocComAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CommentsController : Controller
    {
        private readonly DocComAPIDBContext dbContext;

        public CommentsController(DocComAPIDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Get all comments
        [HttpGet]
        public async Task<IActionResult> GetAllComments()
        {
            return Ok(await dbContext.Comments.ToListAsync());
        }

        //Get single comment
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCommment([FromRoute] Guid id)
        {
            var comment = await dbContext.Comments.FindAsync(id);
            if (comment != null)
            {
                return Ok(comment);
            }

            return NotFound();
        }

        //Get all formatted comments
        [HttpGet]
        public async Task<IActionResult> GetAllFormattedComments()
        {
            List<comment> list = await dbContext.Comments.ToListAsync();
            List<commentView> result = await convertComments(list);
            return Ok(result);
        }

        //Get single formatted comment
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetFormattedCommment([FromRoute] Guid id)
        {
            var comment = await dbContext.Comments.FindAsync(id);
            if (comment != null)
            {
                var poster = await dbContext.Users.FindAsync(comment.poster);
                var subject = await dbContext.Documents.FindAsync(comment.subject);
                if (subject == null|| poster == null)
                {
                    return BadRequest();
                }
                commentView Result = await convertComment(comment);
                return Ok(Result);
            }

            return NotFound();
        }

        //Get all comments of a single document
        [HttpGet]
        [Route("{subject:guid}")]
        public async Task<IActionResult> GetAllCommentsFromDocument([FromRoute] Guid documentId)
        {
            var subject = dbContext.Documents.FindAsync(documentId);
            if (subject == null)
            {
                return BadRequest();
            }
            List<comment> comments = await dbContext.Comments.Where(comment => comment.subject == documentId).ToListAsync();
            return Ok(comments);
        }


        //Get all formatted comments of a single document
        [HttpGet]
        [Route("{subject:guid}")]
        public async Task<IActionResult> GetAllFormattedCommentsfromDocument([FromRoute] Guid documentId)
        {
            var subject = await dbContext.Documents.FindAsync(documentId);
            if (subject == null)
            {
                return BadRequest();
            }
            List<comment> comments = await dbContext.Comments.Where(comment => comment.subject == documentId).ToListAsync();
            List<commentView> result = await convertComments(comments);


            return Ok(result);
        }


        //Get all comments of a single user
        [HttpGet]
        [Route("{poster:guid}")]
        public async Task<IActionResult> GetAllCommentsFromUser([FromRoute] Guid posterId)
        {
            var poster = dbContext.Users.FindAsync(posterId);
            if (poster == null)
            {
                return BadRequest();
            }
            List<comment> comments = await dbContext.Comments.Where(comment => comment.poster == posterId).ToListAsync();

            return Ok(comments);
        }


        //Get all formatted comments of a single user
        [HttpGet]
        [Route("{poster:guid}")]
        public async Task<IActionResult> GetAllFormattedCommentsFromUser([FromRoute] Guid posterId)
        {
            var poster = await dbContext.Users.FindAsync(posterId);
            if (poster == null)
            {
                return BadRequest();
            }
            List<comment> comments = await dbContext.Comments.Where(comment => comment.poster == posterId).ToListAsync();
            List<commentView> result = await convertComments(comments);

            return Ok(result);
        }



        //Add one comment
        [HttpPost]
        public async Task<IActionResult> AddCommment(addCommentRequest request)
        {
            var user = await dbContext.Users.FindAsync(request.poster);
            var document = await dbContext.Documents.FindAsync(request.subject);
            if (user == null)
            {
                return BadRequest("Posting user not found");
            }
            if (document == null)
            {
                return BadRequest("Subject document not found");
            }
            var comment = new comment()
            {
                id = Guid.NewGuid(),
                poster = request.poster,
                subject = request.subject,
                content = request.content,
                commentStatus = 0,
                date = DateTime.Now
            };

            await dbContext.Comments.AddAsync(comment);
            await dbContext.SaveChangesAsync();
            return Ok(comment);

        }

        //Update single comment status
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCommment([FromRoute] Guid id, updateCommentRequest request)
        {
            var comment = await dbContext.Comments.FindAsync(id);
            if (comment != null)
            {
                comment.commentStatus = request.commentStatus;

                await dbContext.SaveChangesAsync();
                return Ok(comment);
            }
            return NotFound();

        }


        //delete single comment
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCommment([FromRoute] Guid id)
        {
            var comment = await dbContext.Documents.FindAsync(id);
            if (comment != null)
            {
                dbContext.Remove(comment);
                await dbContext.SaveChangesAsync();
                return Ok(comment);
            }

            return NotFound();
        }


        [NonAction]
        public async Task<commentView> convertComment(comment comment)
        {
            var result = new commentView
            {
                date = comment.date,
                content = comment.content,
                commentStatus = comment.commentStatus

            };
            var poster = await dbContext.Users.FindAsync(comment.poster);
            if (poster != null)
            {
                result.posterUsername = poster.username;
            }
            else
            {
                result.posterUsername = "Error 404: poster not found";
            }
            var subject = await dbContext.Documents.FindAsync(comment.subject);
            if (subject != null)
            {
                result.subjectTitle = subject.title;
            }
            else
            {
                result.subjectTitle = "Error 404: subject not found";
            }

            return result;
        }


        [NonAction]
        public async Task<List<commentView>> convertComments(List<comment> comments)
        {
            var result = new List<commentView>();
            foreach (var comment in comments)
            {
                commentView additionView = new commentView
                {
                    content = comment.content,
                    commentStatus = comment.commentStatus,
                    date = comment.date
                };
                var subject = await dbContext.Documents.FindAsync(comment.subject);
                if (subject != null)
                {
                    additionView.subjectTitle = subject.title;
                }
                else
                {
                    additionView.subjectTitle = "Error 404: Subject not found";
                }
                var poster = await dbContext.Users.FindAsync(comment.poster);
                if (poster != null)
                {
                    additionView.posterUsername = poster.username;
                }
                else
                {
                    additionView.posterUsername = "Error 404: Poster not found";
                }
                result.Add(additionView);
            }


            return result;
        }


    }
}

