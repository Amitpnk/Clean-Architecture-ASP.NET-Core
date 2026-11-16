using System.Net;
using System.Net.Sockets;
using CleanArch.Domain.Common;

namespace CleanArch.Domain.UnitTests.Common;

public class IpHelperTests
{
    [Fact]
    public void GetIpAddress_ReturnsEmptyOrAnIPv4Address()
    {
        var ip = IpHelper.GetIpAddress();

        Assert.True(ip == string.Empty ||
                    (IPAddress.TryParse(ip, out var parsed) && parsed.AddressFamily == AddressFamily.InterNetwork),
            $"Unexpected value '{ip}'");
    }
}
