﻿using MassTransit;
using Windetta.Common.Messages;

namespace Windetta.Contracts.Events;

public interface IBalancesHeld : CorrelatedBy<Guid>, IEvent
{

}