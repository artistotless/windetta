﻿using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IMatchCompleted : CorrelatedBy<Guid>, IEvent
{
    public IEnumerable<Guid> Winners { get; set; }
}