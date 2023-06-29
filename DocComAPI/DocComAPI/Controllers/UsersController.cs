using DocComAPI.Data;
using DocComAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DocComAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : Controller
    {

        private readonly DocComAPIDBContext dbContext;

        public UsersController(DocComAPIDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Get all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok( await dbContext.Users.ToListAsync());
        }

        //Get single user
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetUser([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                return Ok(user);
            }

            return NotFound();
        }

        //Add one user
        [HttpPost]
        public async Task<IActionResult> AddUser(addUserRequest request) {

            var user = new user()
            {
                id = Guid.NewGuid(),
                username = request.username,
                email = request.email,
                password = request.password,
                securityLevel = 0
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok(user);

        }

        //Update single user
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, updateUserRequest request)
        {
            var user = await dbContext.Users.FindAsync(id);
            if(user != null)
            {
                user.email = request.email;
                user.password = request.password;
                user.securityLevel = request.securityLevel; 
                user.username = request.username;

                await dbContext.SaveChangesAsync();
                return Ok(user);
            }

            return NotFound();

        }


        //delete single user, the related comments the user made themselves and the documents made by the user with the related comments to those documents.
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {

                List<comment> comments = await dbContext.Comments.Where(comment => comment.poster == id).ToListAsync();
                foreach (comment comment in comments)
                {
                    dbContext.Comments.Remove(comment);
                }

                List<document> documents = await dbContext.Documents.Where(document => document.author == id).ToListAsync();
                foreach (document document in documents)
                {
                    List<comment> docComments = await dbContext.Comments.Where(comment => comment.subject == document.id).ToListAsync();
                    foreach(comment comment in docComments)
                    {
                        dbContext.Comments.Remove(comment);
                    }
                    dbContext.Documents.Remove(document);
                }

                dbContext.Remove(user);
                await dbContext.SaveChangesAsync();
                return Ok(user);
            }

            return NotFound();
        }

    }
}
