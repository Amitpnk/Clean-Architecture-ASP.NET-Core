using CleanArch.Domain.Common;

namespace CleanArch.Domain.UnitTests.Common;

public class ValueObjectTests
{
    private sealed class Address(string street, string city) : ValueObject
    {
        public static bool operator ==(Address left, Address right) => EqualOperator(left, right);
        public static bool operator !=(Address left, Address right) => NotEqualOperator(left, right);

        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return street;
            yield return city;
        }
    }

    private sealed class OtherAddress(string street, string city) : ValueObject
    {
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return street;
            yield return city;
        }
    }

    [Test]
    public void SameValues_AreEqual_AndShareHashCode()
    {
        var a = new Address("Main St", "Pune");
        var b = new Address("Main St", "Pune");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a.Equals(b), Is.True);
            Assert.That(a == b, Is.True);
            Assert.That(a != b, Is.False);
            Assert.That(a.GetHashCode(), Is.EqualTo(b.GetHashCode()));
        }
    }

    [Test]
    public void DifferentValues_AreNotEqual()
    {
        var a = new Address("Main St", "Pune");
        var b = new Address("Main St", "Goa");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a.Equals(b), Is.False);
            Assert.That(a == b, Is.False);
            Assert.That(a != b, Is.True);
        }
    }

    [Test]
    public void NullComponents_AreComparedSafely()
    {
        var withNull = new Address(null, "Pune");
        var alsoNull = new Address(null, "Pune");
        var withValue = new Address("Main St", "Pune");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(withNull.Equals(alsoNull), Is.True);
            Assert.That(withNull.Equals(withValue), Is.False);
            Assert.That(withValue.Equals(withNull), Is.False);
            Assert.That(withNull.GetHashCode(), Is.EqualTo(alsoNull.GetHashCode()));
        }
    }

    [Test]
    public void NullOrDifferentType_IsNotEqual()
    {
        var a = new Address("Main St", "Pune");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(a.Equals(null), Is.False);
            Assert.That(a.Equals(new OtherAddress("Main St", "Pune")), Is.False);
            Assert.That(a == null, Is.False);
            Assert.That(null == a, Is.False);
            Assert.That((Address)null == null, Is.True);
        }
    }
}
