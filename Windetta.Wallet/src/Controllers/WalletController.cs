using Microsoft.AspNetCore.Mvc;

namespace Windetta.Wallet.Controllers;

[Route("[controller]")]
public class WalletController : Controller
{
    [HttpGet]
    [Route("balance")]
    public IActionResult GetBalance()
    {
        return View();
    }

    [HttpGet]
    [Route("deposit")]
    public IActionResult GetDepositAddress()
    {
        return View();
    }

    [HttpPost]
    [Route("withdraw")]
    public IActionResult Withdraw()
    {
        return View();
    }
}
