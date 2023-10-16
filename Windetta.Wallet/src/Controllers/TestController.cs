using Autofac.Core;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Types;
using Windetta.Contracts.Commands;
using Windetta.Wallet.Application.Services;
using Windetta.Wallet.Infrastructure.Services;

namespace Windetta.Wallet.Controllers;

[Route("[controller]")]
public class TestController : ControllerBase
{
    public static Guid UserId = Guid.NewGuid();
    private readonly ITonService _service;
    private readonly IUserWalletService _walletService;
    private readonly IBus _bus;

    public TestController(IUserWalletService walletService, IBus bus, ITonService service)
    {
        _walletService = walletService;
        _bus = bus;
        _service = service;
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        var walletData = await _service.GenerateWalletData();

        return Ok(walletData);
    }


    [HttpGet]
    [Route("get/{userId:guid}")]
    public async Task<IActionResult> Get(Guid userId)
    {
        var casted = (_walletService as UserWalletService)!;

        var keys = await casted.GetSecretKeyAsync(userId);
        var info = await _walletService.GetWalletInfoAsync(userId);

        return Ok(new { info = info, keys = keys });
    }

    public async Task<IActionResult> Withdraw()
    {
        var command = new
        {
            //UserId = TestController.UserId,
            UserId = new Guid("fc3e8235-a615-493b-a7b5-7ed4c5b5f6d9"),
            Destination = new TonAddress("EQCNkSLURL98zKoKQeEoMSCb7uMO5JFWF5CEaJ-f1baspjA2"),
            Nanotons = 500000000
        };

        await _bus.Publish<IWithdrawTon>(command);

        return Ok(command);
    }
}
