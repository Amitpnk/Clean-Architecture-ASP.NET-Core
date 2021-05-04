using CA.Domain.Entities;
using CA.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CA.Test.Unit.Persistance.Context
{
    public class ApplicationDbContextTest
    {
        [Test]
        public void CanInsertCardIntoDatabasee()
        {
            using var context = new ApplicationDbContext();
            var card = new Card();
            context.Cards.Add(card);
            Assert.AreEqual(EntityState.Added, context.Entry(card).State);
        }

        [Test]
        public void CanInsertGroupIntoDatabasee()
        {
            using var context = new ApplicationDbContext();
            var groups = new Group();
            context.Groups.Add(groups);
            Assert.AreEqual(EntityState.Added, context.Entry(groups).State);
        }
    }
}
