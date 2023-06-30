using DocComAPI.Data;
using DocComAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DocComAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DocumentsController : Controller
    {

        private readonly DocComAPIDBContext dbContext;

        public DocumentsController(DocComAPIDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Get all formatted documents
        [HttpGet]
        public async Task<IActionResult> GetAllFormattedDocuments()
        {
            var list = await dbContext.Documents.ToListAsync();
            List<viewDocument> result = await ConvertDocuments(list);

            return Ok(result);
        }

        //Get all documents
        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            return Ok(await dbContext.Documents.ToListAsync());
        }

        //Get single formatted document 
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetFormattedDocument([FromRoute] Guid id)
        {
            var document = await dbContext.Documents.FindAsync(id);
            if (document != null)
            {
                viewDocument documentView = await convertDocument(document);
                return Ok(documentView);
            }

            return NotFound();
        }

        //Get single document
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetDocument([FromRoute] Guid id)
        {
            var document = await dbContext.Documents.FindAsync(id);
            if (document != null)
            {
                return Ok(document);
            }

            return NotFound();
        }

        private async Task<Boolean> CheckAuthorQuery(Guid authorId)
        {
            var author = await dbContext.Users.FindAsync(authorId);
            if (author == null)
            {
                return false;
            }
            List<document> list = await dbContext.Documents.Where(document => document.author == authorId).ToListAsync();
            if (list.IsNullOrEmpty())
            {
                return false;
            }
            return true;
        }


        //Get all formatted documents from one author
        [HttpGet]
        [Route("{author:guid}")]
        public async Task<IActionResult> GetAllFormattedDocumentsFromAuthor([FromRoute] Guid authorId)
        {
            var authorCheck = await CheckAuthorQuery(authorId);

            if (authorCheck)
            {
                List<document> list = await dbContext.Documents.Where(document => document.author == authorId).ToListAsync();
                List<viewDocument> result = await ConvertDocuments(list);

                return Ok(result);
            }
            return BadRequest();
        }

        //Get all documents from one author
        [HttpGet]
        [Route("{author:guid}")]
        public async Task<IActionResult> GetAllDocumentsFromAuthor([FromRoute] Guid authorId)
        {
            var authorCheck = await CheckAuthorQuery(authorId);

            if (authorCheck)
            {
                List<document> list = await dbContext.Documents.Where(document => document.author == authorId).ToListAsync();
                return Ok(list);
            }
            return BadRequest();
        }

        //Add one document
        [HttpPost]
        public async Task<IActionResult> Adddocument(addDocumentRequest request)
        {

            var author = await dbContext.Users.FindAsync(request.author);
            if (author == null)
            {
                BadRequest("Error: Author does not exist");
            }

            var document = new document()
            {
                id = Guid.NewGuid(),
                title = request.title,
                date = DateTime.Now,
                documentType = request.documentType,
                documentSecurityLevel = 0,
                author = request.author,
                documentLocation = "NA",
                documentStatus = 0
            };

            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            return Ok(document);

        }

        //Update single document
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateDocument([FromRoute] Guid id, updateDocumentRequest request)
        {
            var document = await dbContext.Documents.FindAsync(id);
            if (document != null)
            {
                document.title = request.title;
                document.documentStatus = request.documentStatus;
                document.documentSecurityLevel = request.documentSecurityLevel;

                await dbContext.SaveChangesAsync();
                return Ok(document);
            }

            return NotFound();

        }


        //delete single document
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteDocument([FromRoute] Guid id)
        {
            var document = await dbContext.Documents.FindAsync(id);
            if (document != null)
            {
                List<comment> comments = await dbContext.Comments.Where(comment => comment.subject == id).ToListAsync();
                foreach (comment comment in comments)
                {
                    dbContext.Comments.Remove(comment);
                }
                dbContext.Documents.Remove(document);
                await dbContext.SaveChangesAsync();
                return Ok(document);
            }
            return NotFound();
        }


        //Get Full document page
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetDocumentPage([FromRoute] Guid id)
        {
            var document = await dbContext.Documents.FindAsync(id);
            if (document != null)
            {
                var author = await dbContext.Users.FindAsync(document.author);
                if (author == null)
                {
                    return BadRequest();
                }
                List<comment> commentList = await dbContext.Comments.Where(comment => comment.subject == document.id).ToListAsync();
                List<commentView> result = new List<commentView>();
                foreach (var comment in commentList)
                {
                    var poster = await dbContext.Users.FindAsync(comment.poster);
                    if (poster == null)
                    {
                        continue;
                    }
                    commentView additionView = new commentView
                    {
                        subjectTitle = document.title,
                        posterUsername = poster.username,
                        content = comment.content,
                        commentStatus = comment.commentStatus,
                        date = comment.date
                    };
                    result.Add(additionView);
                }

                var resultPage = new page
                {
                    id = id,
                    title = document.title,
                    documentStatus = document.documentStatus,
                    author = document.author,
                    date = document.date,
                    authorUsername = author.username,
                    documentSecurityLevel = document.documentSecurityLevel,
                    documentType = document.documentType,
                    documentLocation = document.documentLocation,
                    comments = result
                };

                return Ok(resultPage);
            }

            return NotFound();
        }

        [NonAction]
        public async Task<List<viewDocument>> ConvertDocuments(List<document> documents)
        {
            var result = new List<viewDocument>();
            foreach (var document in documents)
            {
                viewDocument addition = new viewDocument();
                var author = await dbContext.Users.FindAsync(document.author);
                if (author == null)
                {
                    addition.author = "Error 404: Author not found";
                }
                else
                {
                    addition.author = author.username;
                }
                addition.date = document.date;
                addition.title = document.title;
                addition.documentStatus = document.documentStatus;
                addition.documentType = document.documentType;
                result.Add(addition);
            }
            return result;
        }

        [NonAction]
        public async Task<viewDocument> convertDocument(document document)
        {
            var result = new viewDocument();
            var author = await dbContext.Users.FindAsync(document.author);
            if (author == null)
            {
                result.author = "Error 404: Author not found";
            }
            else
            {
                result.author = author.username;
            }
            result.date = document.date;
            result.title = document.title;
            result.documentStatus = document.documentStatus;
            result.documentType = document.documentType;


            return result;
        }




    }






}
