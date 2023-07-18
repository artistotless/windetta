namespace Windetta.Common.RabbitMQ;

public class CorrelationContext : ICorrelationContext
{
    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid ResourceId { get; }
    public string TraceId { get; }
    public string SpanContext { get; }
    public string ConnectionId { get; }
    public string Name { get; }
    public string Origin { get; }
    public string Resource { get; }
    public string Culture { get; }
    public int Retries { get; set; }
    public DateTime CreatedAt { get; }

    public static ICorrelationContext Empty
        => new CorrelationContext();
}