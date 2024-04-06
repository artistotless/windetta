using LSPM.Models;
using Windetta.Common.Types;

namespace Windetta.Main.Core.Matches;

/// <summary>
/// Manages player tickets
/// </summary>
/// <remarks>Task: #TvSCrRzC</remarks>
public interface ITickets : IScopedService
{
    /// <summary>
    /// Sets the tickets
    /// </summary>
    /// <param name="tickets">Collection of tickets</param>
    public Task SetRangeAsync(IEnumerable<Ticket> tickets);

    /// <summary>
    /// Sets the ticket for certain player
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Ticket string</returns>
    public Task<string> GetAsync(TicketKey key);

    /// <summary>
    /// Unsets the ticket for certain player
    /// </summary>
    /// <param name="key">Key</param>
    public Task UnsetAsync(TicketKey key);
}