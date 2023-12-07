using Windetta.Main.Core.Services;

namespace Windetta.MainTests.Mocks;

public class MinUserBalanceJoinFilter : JoinFilterConfigurable
{
    private readonly IWalletService _walletService;

    public MinUserBalanceJoinFilter(IWalletService walletService)
    {
        _walletService = walletService;
    }

    public override async ValueTask<(bool isValid, string? error)> ExecuteAsync(Guid userId, CancellationToken token)
    {
        var currencyId = GetRequirementValue<int>("currencyId");
        var requiredBalance = GetRequirementValue<ulong>("minBalance");
        var actualBalance = await _walletService.GetBalance(userId, currencyId);

        if (actualBalance.AvailableAmount >= requiredBalance)
            return (true, null);
        else
            return (false, $"funds not enough to join. Minimum required balance - {requiredBalance}");
    }

    protected override RequirementsDefinition? SetupRequirements(RequirementsDefinitionBuilder builder)
    {
        return builder
            .Add<int>("currencyId")
            .Add<ulong>("minBalance")
            .Build();
    }
}