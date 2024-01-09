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
    [Route("{userId:guid}/balances/{currencyId:int}")]
    public async Task<IActionResult> GetBalance([FromRoute] Guid userId, [FromRoute] int currencyId)
    {
        var balance = await _walletService.GetBalance(userId, currencyId);

        return Ok(balance);
    }
}
