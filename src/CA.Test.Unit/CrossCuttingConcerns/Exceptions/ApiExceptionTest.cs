using CA.CrossCuttingConcerns.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CA.Test.Unit.CrossCuttingConcerns.Exceptions
{
    public class ApiExceptionTest
    {
        [Test]
        public void CheckApiException()
        {

            Exception ex = new ApiException("Message", new Exception("Inner exception."));
            Exception ex2 = new ApiException("Message");

            string exceptionToString = ex.ToString();

            Assert.AreEqual(exceptionToString, ex.ToString());
            Assert.AreEqual(exceptionToString, ex2.ToString());


        }
    }
}
