using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDocComAPI
{
    internal class testDocumentsController
    {
        private static DbContextOptions<DocComAPIDBContext> dbContextOptions = new DbContextOptionsBuilder<DocComAPIDBContext>().UseInMemoryDatabase("DocComDB").Options;

        DocComAPIDBContext dbContext;
        UsersController usersController;
        DocumentsController documentsController;
        CommentsController commentsController;

        //Test values
        Guid AuthorID1;
        Guid AuthorID2;
        Guid DocID1;
        Guid DocID2;
        Guid DocID3;

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

            //Retrieve 2 user id's for testing
            dbContext.SaveChanges();
            var actionresult = await usersController.GetAllUsers();
            var okResult = actionresult as OkObjectResult;
            var users = okResult.Value as List<user>;
            AuthorID1 = users[0].id;
            AuthorID2 = users[1].id;

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
            var addRequestDoc3 = new addDocumentRequest
            {
                author = AuthorID2,
                title = "word or txt",
                documentType = "pdf"
            };
            await documentsController.Adddocument(addRequestDoc3);
            dbContext.SaveChanges();

            //Retrieve Document ID's for testing
            var docActionresult = await documentsController.GetAllDocuments();
            var docOkResult = docActionresult as OkObjectResult;
            var docs = docOkResult.Value as List<document>;
            DocID1 = docs[0].id;
            DocID2 = docs[1].id;
            DocID3 = docs[2].id;

            //add comments for testing
            var addRequestCom1 = new addCommentRequest
            {
                poster = AuthorID1,
                subject = DocID1,
                content = "To be deleteded too"
            };
            await commentsController.AddCommment(addRequestCom1);
            var addRequestCom2 = new addCommentRequest
            {
                poster = AuthorID1,
                subject = DocID2,
                content = "Something"
            };
            await commentsController.AddCommment(addRequestCom2);
            var addRequestCom3 = new addCommentRequest
            {
                poster = AuthorID2,
                subject = DocID2,
                content = "something1"
            };
            await commentsController.AddCommment(addRequestCom3);
            var addRequestCom4 = new addCommentRequest
            {
                poster = AuthorID2,
                subject = DocID1,
                content = "To be deleteded too"
            };
            await commentsController.AddCommment(addRequestCom4);
            dbContext.SaveChanges();
        }

        [Test]
        public async Task AddDocument_Test()
        {
            var addRequestDoc = new addDocumentRequest
            {
                author = AuthorID2,
                title = "pdf or txt",
                documentType = "pdf"
            };
            await documentsController.Adddocument(addRequestDoc);
            dbContext.SaveChanges();
            var docactionresult = await documentsController.GetAllDocuments();
            var docokResult = docactionresult as OkObjectResult;
            var documents = docokResult.Value as List<document>;
            if (documents.Count() == 4)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public async Task UpdateDocument_Test()
        {
            var updateRequestDoc = new updateDocumentRequest
            {
                title = "txt or word",
                documentStatus = 1,
                documentSecurityLevel = 0
            };
            await documentsController.UpdateDocument(DocID3, updateRequestDoc);
            var docactionresult = await documentsController.GetDocument(DocID3);
            var docokResult = docactionresult as OkObjectResult;
            var document = docokResult.Value as document;
            if (document.title == "txt or word" && document.documentStatus == 1)
            {
                Assert.Pass();
            }
            Assert.Fail();
        }

        [Test]
        public async Task DeleteDocument_Simple_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task DeleteDocument_Complex_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetDocument_Raw_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetAllDocument_Raw_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetDocument_Formatted_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetAllDocument_Formatted_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetDocument_Page_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetAllDocument_FromAuthor_Formatted_Test()
        {
            Assert.Ignore();
        }

        [Test]
        public async Task GetAllDocument_FromAuthor_Raw_Test()
        {
            Assert.Ignore();
        }

    }
}
