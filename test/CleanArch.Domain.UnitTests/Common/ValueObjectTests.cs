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

    [Fact]
    public void SameValues_AreEqual_AndShareHashCode()
    {
        var a = new Address("Main St", "Pune");
        var b = new Address("Main St", "Pune");

        Assert.True(a.Equals(b));
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void DifferentValues_AreNotEqual()
    {
        var a = new Address("Main St", "Pune");
        var b = new Address("Main St", "Goa");

        Assert.False(a.Equals(b));
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void NullComponents_AreComparedSafely()
    {
        var withNull = new Address(null, "Pune");
        var alsoNull = new Address(null, "Pune");
        var withValue = new Address("Main St", "Pune");

        Assert.True(withNull.Equals(alsoNull));
        Assert.False(withNull.Equals(withValue));
        Assert.False(withValue.Equals(withNull));
        Assert.Equal(withNull.GetHashCode(), alsoNull.GetHashCode());
    }

    [Fact]
    public void NullOrDifferentType_IsNotEqual()
    {
        var a = new Address("Main St", "Pune");

        Assert.False(a.Equals(null));
        Assert.False(a.Equals(new OtherAddress("Main St", "Pune")));
        Assert.False(a == null);
        Assert.False(null == a);
        Assert.True((Address)null == null);
    }
}
