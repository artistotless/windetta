using Moq;

namespace Windetta.Common.Testing;

public abstract class MockInitializator<TMockable> where TMockable : class
{
    protected bool isSetup { get; private set; }

    private Mock<TMockable> _mock;
    public Mock<TMockable> Mock
    {
        get
        {
            if (!isSetup)
                Setup(_mock);

            isSetup = true;
            return _mock;
        }
        private set
        {
            _mock = value;
        }
    }

    protected MockInitializator()
    {
        Mock = new Mock<TMockable>();
    }

    protected abstract void Setup(Mock<TMockable> mock);

    public static implicit operator Mock<TMockable>(MockInitializator<TMockable> wrapper)
        => wrapper.Mock;
}
