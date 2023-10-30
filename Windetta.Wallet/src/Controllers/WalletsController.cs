using Microsoft.AspNetCore.Mvc;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Controllers;

[Route("[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IUserWalletService _walletService;
    private readonly IDepositAddressSource _addressSource;

    public WalletsController(IUserWalletService walletService, IDepositAddressSource addressSource)
    {
        _walletService = walletService;
        _addressSource = addressSource;
    }

    [HttpGet]
    [Route("{userId:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var balance = await _walletService.GetBalance(userId);

        return Ok(balance);
    }

    [HttpGet]
    [Route("{userId:guid}/deposit")]
    public async Task<IActionResult> GetDepositAddress(Guid userId)
    {
        var address = await _addressSource.GetAddressAsync();

        return Ok(address);
    }
}
