using Microsoft.AspNetCore.Mvc;
using Windetta.Wallet.Application.Services;

namespace Windetta.Wallet.Controllers;

[Route("[controller]")]
public class WalletsController : ControllerBase
{
    private readonly IUserWalletService _walletService;

    public WalletsController(IUserWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpGet]
    [Route("{userId:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid userId)
    {
        var balance = await _walletService.GetBalance(userId);

        return Ok(balance);
    }
}
