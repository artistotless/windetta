namespace Windetta.Common.MassTransit;

public class RabbitMqOptions
{
    public string Namespace { get; set; }
    public int Retries { get; set; }
    public int RetryInterval { get; set; }
    public string VirtualHost { get; set; }
    public List<string> Hostnames { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}