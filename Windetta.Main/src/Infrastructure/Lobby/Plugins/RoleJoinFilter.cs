﻿using Windetta.Main.Core.Lobbies.Plugins;

namespace Windetta.Main.Infrastructure.Lobby.Plugins;

public class RoleJoinFilter : JoinFilterConfigurable
{
    private readonly IHttpContextAccessor _contextAccessor;

    public RoleJoinFilter(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    protected override RequirementsDefinition SetupRequirements(RequirementsDefinitionBuilder builder)
    {
        return builder
            .Add<string>("role")
            .Build();
    }

    public override ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token)
    {
        var roleValue = GetRequirementValue<string>("role");

        if (_contextAccessor.HttpContext is null)
            return ValueTask.FromResult<(bool, string?)>((false, "HttpContext is not available"));

        if (_contextAccessor.HttpContext.User.IsInRole(roleValue))
        {
            return ValueTask.FromResult<(bool, string?)>((true, null));
        }

        return ValueTask.FromResult<(bool, string?)>((false, $"User is not in Role - {roleValue}"));
    }
}
