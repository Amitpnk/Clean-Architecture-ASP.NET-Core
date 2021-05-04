using CA.Domain.Entities;
using NUnit.Framework;
using System;

namespace CA.Test.Unit.Domain.Entities
{
    public class GroupTest
    {
        private readonly Group _groups;
        private readonly Guid Id = Guid.NewGuid();
        private const string Name = "Test";
        private const bool IsActive = true;
        private const string Description = "Test Description";

        public GroupTest()
        {
            _groups = new Group();
        }

        [Test]
        public void TestSetAndGetId()
        {
            _groups.Id = Id;
            Assert.That(_groups.Id, Is.EqualTo(Id));
        }

        [Test]
        public void TestSetAndGetName()
        {
            _groups.Name = Name;
            Assert.That(_groups.Name, Is.EqualTo(Name));
        }

        [Test]
        public void TestSetAndGetDescription()
        {
            _groups.Description = Description;
            Assert.That(_groups.Description, Is.EqualTo(Description));
        }

        [Test]
        public void TestSetAndGetIsActive()
        {
            _groups.IsActive = IsActive;
            Assert.That(_groups.IsActive, Is.EqualTo(IsActive));
        }
    }
}
