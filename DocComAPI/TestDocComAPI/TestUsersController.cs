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

        [OneTimeSetUp]
        public void Setup()
        {
            dbContext = new DocComAPIDBContext(dbContextOptions);
            dbContext.Database.EnsureCreated();
            usersController = new UsersController(dbContext);
            SeedDatabase();

        }


        [OneTimeTearDown]
        public void CleanUp()
        {
            dbContext.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            //TODO: add base set of testData


            dbContext.SaveChanges();
        }


        [Test]
        public void RemoveUser_Test_Simple()
        {
            //TODO
            Assert.Pass();
        }


        [Test]
        public void RemoveUser_Test_complex()
        {
            //TODO
            Assert.Pass();
        }


        [Test]
        public async Task AddUser_Test()
        {
            var addRequest = new addUserRequest
            {
                username = "Testman 1",
                password = "000000",
                email = "Test@tesmail.com"
            };
            await usersController.AddUser(addRequest);
            var actionResult = await usersController.GetAllUsers();
            var okResult = actionResult as OkObjectResult;
            var users = okResult.Value as List<user>;
            if (users == null)
            {
                Assert.Fail();
            }
            else if (users.Count() == 1)
            {
                Assert.Pass();
            }
            else { Assert.Fail(); }
        }



        [Test]
        public async Task UpdateUser()
        {
            var addRequest = new addUserRequest
            {
                username = "Testman 1",
                password = "000000",
                email = "Test@tesmail.com"
            };
            var actionResult = await usersController.AddUser(addRequest);
            var okResult = actionResult as OkObjectResult;
            var user = okResult.Value as user;


            var updateRequest = new updateUserRequest { username = "TestPerson 1", email = addRequest.email, password = addRequest.password, securityLevel = 0 };
            await usersController.UpdateUser(user.id, updateRequest);

            var actionResult2 = await usersController.GetUser(user.id);
            var okResult2 = actionResult as OkObjectResult;
            var user2 = okResult.Value as user;

            if (user2 == null)
            {
                Assert.Fail();
            }
            else if (user2.username == "TestPerson 1")
            {
                Assert.Pass();
            }
            else if (user2.username == user.username)
            {
                Assert.Fail();
            }
            else { Assert.Fail(); }
        }

        [Test]
        public async Task RetrieveUser_Test()
        {
            var addRequest = new addUserRequest
            {
                username = "Testman 1",
                password = "000000",
                email = "Test@tesmail.com"
            };
            var actionResult = await usersController.AddUser(addRequest);
            var okResult = actionResult as OkObjectResult;
            var user = okResult.Value as user;


            //TODO


        }

        [Test]
        public async Task RetrievUsers_Test()
        {
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
            await usersController.AddUser(addRequest);

            var addRequest3 = new addUserRequest
            {
                username = "Testman 3",
                password = "000003",
                email = "Test3@tesmail.com"
            };
            await usersController.AddUser(addRequest);
            //TODO
        }


    }
}