﻿using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Windetta.Common.Configuration;
using Windetta.Common.RabbitMQ;

namespace Windetta.Common.MassTransit;

public static class Extensions
{
    public static void AddMassTransit(this IServiceCollection services, Assembly assembly, string serviceName)
    {
        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();

        var rmqQtions = configuration.GetOptions<RabbitMqOptions>("RabbitMq");

        services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new MyEndpointNameFormatter(serviceName));
            x.AddConsumers(assembly);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rmqQtions.Hostnames.First() ?? "localhost", rmqQtions.VirtualHost ?? "/", h =>
                {
                    h.Username(rmqQtions.Username ?? "admin");
                    h.Password(rmqQtions.Password ?? "admin");
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }
}