using Moq;

namespace Windetta.Common.Testing;

public abstract class MockInitializator<TMockable> where TMockable : class
{
    public Mock<TMockable> Mock { get; private set; }

    protected MockInitializator()
    {
        Mock = new Mock<TMockable>();

        Setup(Mock);
    }

    protected abstract void Setup(Mock<TMockable> mock);

    public static implicit operator Mock<TMockable>(MockInitializator<TMockable> wrapper)
        => wrapper.Mock;
}
