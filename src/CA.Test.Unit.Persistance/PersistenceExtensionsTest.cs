//using CA.Persistance;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CA.Test.Unit.Persistance
//{

//    public class PersistenceExtensionsTest
//    {
//        private readonly IConfigurationRoot _configRoot;
//        public readonly IConfiguration _configuration;
//        private readonly IServiceCollection _serviceCollection;

//        public PersistenceExtensionsTest(IServiceCollection serviceCollection, IConfiguration configuration, IConfigurationRoot configRoot)
//        {
//            _configRoot = configRoot;
//            _serviceCollection = serviceCollection;
//            _configuration = configuration;
//        }

//        [Test]
//        public void CanInsertCardIntoDatabasee()
//        {
//            PersistenceExtensions.AddPersistence(_serviceCollection, _configuration, _configRoot);
//            Assert.IsTrue(true);
//        }

//    }
//}
