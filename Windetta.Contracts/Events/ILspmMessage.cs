namespace Windetta.Contracts.Events;

/// <summary>
/// Message intended for LSPM
/// </summary>
public interface ILspmMessage
{
    /// <summary>
    /// IP address of the LSPM service instance
    /// </summary>
    public string LspmIp { get; set; }
}