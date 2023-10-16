using MassTransit;

namespace Windetta.IdentityTests.Mocks;

internal class BusMockFactory
{
    public static Mock<IBus> Create()
    {
        var factoryMock = new Mock<IBus>();
        factoryMock.Setup(x => x.Publish<It.IsAnyType>(It.IsAny<object>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        return factoryMock;
    }
}
