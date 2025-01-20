using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Windetta.Common.Constants;
using Windetta.Common.MassTransit;
using Windetta.Contracts;
using Windetta.Contracts.Commands;
using Windetta.Contracts.Events;
using Windetta.Contracts.Responses;

namespace Windetta.Wallet.Infrastructure.Controllers;

[Route("[controller]/[action]")]
public class TestController : Controller
{
    private readonly IPublishEndpoint _publishEdnpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IRequestClient<IBalanceRequested> _getBalanceClient;

    public TestController(IPublishEndpoint publishEdnpoint, ISendEndpointProvider sendEndpointProvider, IRequestClient<IBalanceRequested> getBalanceClient)
    {
        _publishEdnpoint = publishEdnpoint;
        _sendEndpointProvider = sendEndpointProvider;
        _getBalanceClient = getBalanceClient;
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid userId, uint currencyId)
    {
        var response = await _getBalanceClient.GetResponse<UserBalanceResponse>(new
        {
            UserId = userId,
            CurrencyId = currencyId,
        });

        return Ok(response);
    }

    [HttpPost("{id:guid}")]
    public IActionResult Create(Guid id)
    {
        var msg = new
        {
            Id = id,
            CorrelationId = Guid.NewGuid()
        };

        _publishEdnpoint.Publish<IUserCreated>(msg);

        return Ok(msg);
    }

    public record IncreasePost(BalanceOperationData Data, PositiveBalanceOperationType Type);

    [HttpPost]
    public async Task<IActionResult> Increase([FromBody] IncreasePost request)
    {
        if (request is null)
            return BadRequest();

        var command = new
        {
            request.Type,
            Data = new BalanceOperationData[] { request.Data }.ToList(),
            CorrelationId = Guid.NewGuid()
        };

        var uri = MyEndpointNameFormatter.CommandUri<IIncreaseBalance>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<IIncreaseBalance>(command);

        return Ok(command);
    }

    public record DeductPost(Guid UserId, NegativeBalanceOperationType Type, FundsInfo Funds);

    [HttpPost]
    public async Task<IActionResult> Deduct([FromBody] DeductPost request)
    {
        if (request is null)
            return BadRequest();

        var command = new
        {
            request.Type,
            request.Funds,
            request.UserId,
            CorrelationId = Guid.NewGuid()
        };

        var uri = MyEndpointNameFormatter.CommandUri<IDeductBalance>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<IDeductBalance>(command);

        return Ok(command);
    }

    public record HoldPost(IEnumerable<Guid> UsersIds, FundsInfo Funds);

    [HttpPost]
    public async Task<IActionResult> Hold([FromBody] HoldPost request)
    {
        if (request is null)
            return BadRequest();

        var command = new
        {
            request.Funds,
            request.UsersIds,
            CorrelationId = Guid.NewGuid()
        };

        var uri = MyEndpointNameFormatter.CommandUri<IHoldBalances>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<IHoldBalances>(command);

        return Ok(command);
    }

    public record UnHoldPost(IEnumerable<Guid> UsersIds, FundsInfo Funds);

    [HttpPost]
    public async Task<IActionResult> UnHold([FromBody] UnHoldPost request)
    {
        if (request is null)
            return BadRequest();

        var command = new
        {
            request.Funds,
            request.UsersIds,
            CorrelationId = Guid.NewGuid()
        };

        var uri = MyEndpointNameFormatter.CommandUri<IUnHoldBalances>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<IUnHoldBalances>(command);

        return Ok(command);
    }

    public record TransferPost(Guid InitiatorUserId, Guid DestinationUserId, FundsInfo Funds);

    [HttpPost]
    public async Task<IActionResult> Transfer([FromBody] TransferPost request)
    {
        if (request is null)
            return BadRequest();

        var command = new
        {
            request.Funds,
            request.InitiatorUserId,
            request.DestinationUserId,
            CorrelationId = Guid.NewGuid()
        };

        var uri = MyEndpointNameFormatter.CommandUri<ITransferBalance>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<ITransferBalance>(command);

        return Ok(command);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UnDeduct(Guid id)
    {
        var command = new
        {
            CorrelationId = id
        };

        var uri = MyEndpointNameFormatter.CommandUri<IUnDeductBalance>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<IUnDeductBalance>(command);

        return Ok(command);
    }

    public record DeductUnHoldPost(BalanceOperationData Data, NegativeBalanceOperationType Type);

    [HttpPost]
    public async Task<IActionResult> DeductUnHold([FromBody] DeductUnHoldPost request)
    {
        var command = new
        {
            request.Type,
            Data = new BalanceOperationData[] { request.Data }.ToList(),
            CorrelationId = Guid.NewGuid(),
        };

        var uri = MyEndpointNameFormatter.CommandUri<IDeductUnHoldBalance>(Svc.Wallet);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(uri);

        await endpoint.Send<IDeductUnHoldBalance>(command);

        return Ok(command);
    }
}
