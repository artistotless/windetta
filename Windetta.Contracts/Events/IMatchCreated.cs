﻿using MassTransit;
using Windetta.Contracts.Base;

namespace Windetta.Contracts.Events;

public interface IMatchCreated : CorrelatedBy<Guid>, IEvent
{
    public IReadOnlyDictionary<Guid, string> Tickets { get; set; }
}