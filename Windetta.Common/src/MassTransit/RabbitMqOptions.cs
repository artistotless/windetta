using RawRabbit.Configuration;

namespace Windetta.Common.MassTransit;

public class RabbitMqOptions : RawRabbitConfiguration
{
    public string Namespace { get; set; }
    public int Retries { get; set; }
    public int RetryInterval { get; set; }
}