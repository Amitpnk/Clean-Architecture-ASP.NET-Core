using CA.Domain.Entities;
using NUnit.Framework;
using System;

namespace CA.Test.Unit.Domain.Entities
{
    public class CardTest
    {
        private readonly Card _cards;
        private readonly Guid Id = Guid.NewGuid();
        private const int SerialNumber = 1;
        private const int Chapter = 1;
        private const int Verse = 1;
        private const string Description = "Test Description";
        private const string Synonmys = "Test Synonmys";
        private const string Meaning = "Test Meaning";

        public CardTest()
        {
            _cards = new Card();
        }

        [Test]
        public void TestSetAndGetId()
        {
            _cards.Id = Id;
            Assert.That(_cards.Id, Is.EqualTo(Id));
        }

        [Test]
        public void TestSetAndGetSerialNumber()
        {
            _cards.SerialNumber = SerialNumber;
            Assert.That(_cards.SerialNumber, Is.EqualTo(SerialNumber));
        }

        [Test]
        public void TestSetAndGetChapter()
        {
            _cards.Chapter = Chapter;
            Assert.That(_cards.Chapter, Is.EqualTo(Chapter));
        }

        [Test]
        public void TestSetAndGetVerse()
        {
            _cards.Verse = Verse;
            Assert.That(_cards.Verse, Is.EqualTo(Verse));
        }
        [Test]
        public void TestSetAndGetDescription()
        {
            _cards.Description = Description;
            Assert.That(_cards.Description, Is.EqualTo(Description));
        }

        [Test]
        public void TestSetAndGetSynonmys()
        {
            _cards.Synonmys = Synonmys;
            Assert.That(_cards.Synonmys, Is.EqualTo(Synonmys));
        }

        [Test]
        public void TestSetAndGetMeaning()
        {
            _cards.Meaning = Meaning;
            Assert.That(_cards.Meaning, Is.EqualTo(Meaning));
        }
    }
}
