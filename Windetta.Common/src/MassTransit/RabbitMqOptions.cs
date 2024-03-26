namespace Windetta.Common.MassTransit;

public class RabbitMqOptions
{
    public int Port { get; set; }
    public string VirtualHost { get; set; }
    public List<string> Hostnames { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}