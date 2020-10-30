using CA.CrossCuttingConcerns.Exceptions;
using NUnit.Framework;
using System;

namespace CA.Test.Unit.CrossCutting
{
    public class Exceptions
    {
        [TestCase("API exception")]
        public void CheckApiException(string ExceptionMessage)
        {
            Exception ex = new ApiException(ExceptionMessage, new Exception("Inner exception."));
            Exception ex2 = new ApiException(ExceptionMessage);

            Assert.AreEqual(ExceptionMessage, ex.Message.ToString());
            Assert.AreEqual(ExceptionMessage, ex2.Message.ToString());
        }

        [TestCase("Bad request exception")]
        public void CheckBadRequestException(string ExceptionMessage)
        {
            Exception ex = new BadRequestException(ExceptionMessage);

            Assert.AreEqual(ExceptionMessage, ex.Message.ToString());
        }

        [TestCase("card", 1, "Exception message")]
        [TestCase("card", "f09cedac-8c21-4f04-9ded-219bb57a07a4", "Exception message")]
        public void CheckDeleteFailureException(string name, object key, string message)
        {

            string ExceptionMessage = $"Deletion of entity \"{name}\" ({key}) failed. {message}";
            Exception ex = new DeleteFailureException(name, key, message);

            Assert.AreEqual(ExceptionMessage, ex.Message.ToString());
        }

        [TestCase("card", 1)]
        [TestCase("card", "f09cedac-8c21-4f04-9ded-219bb57a07a4")]
        public void CheckNotFoundException(string name, object key)
        {

            string ExceptionMessage = $"Entity \"{name}\" ({key}) was not found.";
            Exception ex = new NotFoundException(name, key);

            Assert.AreEqual(ExceptionMessage, ex.Message.ToString());
        }

        [TestCase("One or more validation failures have occurred.")]
        public void CheckValidationException(string ExceptionMessage)
        {
            var ex1 = new ValidationException();
            var ex2 = new ValidationException(ExceptionMessage);


            Assert.AreEqual(ExceptionMessage, ex1.Message.ToString());
            Assert.AreEqual(ExceptionMessage, ex2.Message.ToString());
        }

    }
}
