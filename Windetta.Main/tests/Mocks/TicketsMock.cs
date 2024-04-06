using AutoFixture;
using LSPM.Models;
using Windetta.Common.Testing;
using Windetta.Main.Core.Matches;

namespace Windetta.MainTests.Mocks;

public class TicketsMock : MockInitializator<ITickets>
{
    protected override void Setup(Mock<ITickets> mock)
    {
        mock
            .Setup(x => x.GetAsync(It.IsAny<TicketKey>()))
            .ReturnsAsync(new Fixture().Create<string>());
        mock
            .Setup(x => x.UnsetAsync(It.IsAny<TicketKey>()));
        mock
            .Setup(x => x.SetRangeAsync(It.IsAny<IEnumerable<Ticket>>()));
    }
}