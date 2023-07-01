using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;


namespace TestDocComAPI
{
    public class TestUsersController
    {
        private static DbContextOptions<DocComAPIDBContext> dbContextOptions = new DbContextOptionsBuilder<DocComAPIDBContext>().UseInMemoryDatabase("DocComDB").Options;

        DocComAPIDBContext dbContext;
        UsersController usersController;
        DocumentsController documentsController;
        CommentsController commentsController;

        //Test values
        Guid AuthorID1;
        Guid AuthorID2;
        Guid AuthorID3;

        [OneTimeSetUp]
        public void Setup()
        {
            dbContext = new DocComAPIDBContext(dbContextOptions);
            dbContext.Database.EnsureCreated();
            usersController = new UsersController(dbContext);
            documentsController = new DocumentsController(dbContext);
            commentsController = new CommentsController(dbContext);
            SeedDatabase();
        }


        [OneTimeTearDown]
        public void CleanUp()
        {
            dbContext.Database.EnsureDeleted();
        }

        private async void SeedDatabase()
        {
            //Fill database with users
            var addRequest = new addUserRequest
            {
                username = "Testman 1",
                password = "000000",
                email = "Test@tesmail.com"
            };
            await usersController.AddUser(addRequest);
            var addRequest2 = new addUserRequest
            {
                username = "Testman 2",
                password = "000002",
                email = "Test2@tesmail.com"
            };
            await usersController.AddUser(addRequest2);

            var addRequest3 = new addUserRequest
            {
                username = "Testman 3",
                password = "000003",
                email = "Test3@tesmail.com"
            };
            await usersController.AddUser(addRequest3);

            //Retrieve user id's for testing
            var actionresult = await usersController.GetAllUsers();
            var okResult = actionresult as OkObjectResult;
            var users = okResult.Value as List<user>;
            AuthorID1 = users[0].id;
            AuthorID2 = users[1].id;
            AuthorID3 = users[2].id;

            //Add documents for testing
            var addRequestDoc1 = new addDocumentRequest
            {
                author = AuthorID1,
                title = "to be deleted",
                documentType = "pdf"
            };
            await documentsController.Adddocument(addRequestDoc1);
            var addRequestDoc2 = new addDocumentRequest
            {
                author = AuthorID2,
                title = "pdf or word",
                documentType = "pdf"
            };
            await documentsController.Adddocument(addRequestDoc2);

            //Retrieve Document ID's for testing
            var docActionresult = await documentsController.GetAllDocuments();
            var docOkResult = docActionresult as OkObjectResult;
            var docs = docOkResult.Value as List<document>;
            Guid docID1 = docs[0].id;
            Guid docID2 = docs[1].id;

            //add comments for testing
            var addRequestCom1 = new addCommentRequest
            {
                poster = AuthorID1,
                subject = docID1,
                content = "To be deleteded too"
            };
            await commentsController.AddCommment(addRequestCom1);
            var addRequestCom2 = new addCommentRequest
            {
                poster = AuthorID1,
                subject = docID2,
                content = "To be deleteded too"
            };
            await commentsController.AddCommment(addRequestCom2);
            var addRequestCom3 = new addCommentRequest
            {
                poster = AuthorID2,
                subject = docID2,
                content = "something1"
            };
            await commentsController.AddCommment(addRequestCom3);
            var addRequestCom4 = new addCommentRequest
            {
                poster = AuthorID2,
                subject = docID1,
                content = "something"
            };
            await commentsController.AddCommment(addRequestCom4);

        }

        [Test]
        public async Task AddUser_Test()
        {
            var addRequest = new addUserRequest
            {
                username = "Testman 4",
                password = "333",
                email = "Test4@tesmail.com"
            };
            await usersController.AddUser(addRequest);
            var actionResult = await usersController.GetAllUsers();
            var okResult = actionResult as OkObjectResult;
            var users = okResult.Value as List<user>;
            if (users != null && users.Count() == 4 && users[3].email == addRequest.email && users[3].username == addRequest.username)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public async Task Login_Test_Username()
        {
            var login = new UsernameLogin
            {
                username = "Testman 1",
                password = "000000"
            };
            LoginCheck(await usersController.LoginUsername(login));
        }

        [Test]
        public async Task Login_Test_Email()
        {
            var login = new emailLogin
            {
                email = "Test@tesmail.com",
                password = "000000"
            };
            LoginCheck(await usersController.LoginEmail(login));
        }

        private void LoginCheck(IActionResult actionResult)
        {
            var okResult = actionResult as OkObjectResult;
            if (okResult != null && okResult.Value != null)
            {
                var id = okResult.Value.ToString();
                if (id == AuthorID1.ToString())
                {
                    Assert.Pass();
                }
            }
            Assert.Fail();
        }

        [Test]
        public async Task UpdateUser()
        {
            var updateRequest = new updateUserRequest { username = "TestPerson 2", email = "Test2@testmail.com", password = "000002", securityLevel = 0 };
            await usersController.UpdateUser(AuthorID2, updateRequest);

            var actionResult = await usersController.GetUser(AuthorID2);
            var okResult = actionResult as OkObjectResult;
            var user = okResult.Value as user;

            if (user == null)
            {
                Assert.Fail();
            }
            else if (user.username == "TestPerson 2" && user.email == "Test2@testmail.com")
            {
                Assert.Pass();
            }
            else if (user.username == "Testman 2" || user.email == "Test@tesmail.com")
            {
                Assert.Fail();
            }
            else { Assert.Fail(); }
        }

        [Test]
        public async Task RetrieveUser_Test()
        {
            var actionResult = await usersController.GetUser(AuthorID2);
            var okResult = actionResult as OkObjectResult;
            var user = okResult.Value as user;
            if (user == null)
            {
                Assert.Fail();
            }
            else if (user.username == "Testman 2" && user.email == "Test2@tesmail.com")
            {
                Assert.Pass();
            }
        }

        [Test]
        public async Task RetrievUsers_Test()
        {
            var actionResult = await usersController.GetAllUsers();
            var okResult = actionResult as OkObjectResult;
            var users = okResult.Value as List<user>;
            if (users != null && users.Count() == 2)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public async Task RemoveUser_Test_Simple()
        {
            await usersController.DeleteUser(AuthorID3);
            var actionresult = await usersController.GetAllUsers();
            var okResult = actionresult as OkObjectResult;
            var users = okResult.Value as List<user>;
            bool contained = false;
            foreach (user user in users)
            {
                if (user.id == AuthorID3)
                    contained = true;
            }
            if (users.Count() == 2 && !contained)
            {
                Assert.Pass();
            }
            Assert.Fail("Users: " + users.Count());
        }

        [Test]
        public async Task RemoveUser_Test_complex()
        {

            await usersController.DeleteUser(AuthorID1);
            //Check if author1 has been removed
            var actionresult = await usersController.GetAllUsers();
            var okResult = actionresult as OkObjectResult;
            var users = okResult.Value as List<user>;
            bool contained = false;
            foreach (user user in users)
            {
                if (user.id == AuthorID1)
                {
                    contained = true;
                }
            }
            if (users.Count() != 3 || contained)
            {
                Assert.Fail();
            }

            //check if docs of author1 have been removed
            var docactionresult = await documentsController.GetAllDocuments();
            var docokResult = docactionresult as OkObjectResult;
            var documents = docokResult.Value as List<document>;
            foreach (document doc in documents)
            {
                if (doc.author == AuthorID1) contained = true;
            }
            if (documents.Count() != 1 || contained)
            {
                Assert.Fail();
            }

            //check if comments of author1 have been removed
            var comactionresult = await commentsController.GetAllComments();
            var comokResult = comactionresult as OkObjectResult;
            var comments = comokResult.Value as List<comment>;
            foreach (comment com in comments)
            {
                if (com.poster == AuthorID1) contained = true;
            }
            if (comments.Count() != 1 || contained)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

    }
}