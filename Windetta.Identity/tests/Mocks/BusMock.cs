using MassTransit;
using Windetta.Common.Testing;

namespace Windetta.IdentityTests.Mocks;

internal class BusMock : MockInitializator<IBus>
{
    protected override void Setup(Mock<IBus> mock)
    {
        mock.Setup(x => x.Publish<It.IsAnyType>(
            It.IsAny<object>(), CancellationToken.None))
           .Returns(Task.CompletedTask);
    }
}
