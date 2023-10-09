using Microsoft.AspNetCore.Mvc;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Controllers;

[Route("[controller]")]
public class WalletController : Controller
{
    private readonly IUserWalletService _walletService;

    public WalletController(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    [Route("{userId:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var info = await _walletService.GetWalletInfoAsync(userId);

        return View(info.Balance);
    }

    [HttpGet]
    [Route("{userId:guid}/deposit")]
    public async Task<IActionResult> GetDepositAddress(Guid userId)
    {
        var info = await _walletService.GetWalletInfoAsync(userId);

        return View(info.Address);
    }
}
