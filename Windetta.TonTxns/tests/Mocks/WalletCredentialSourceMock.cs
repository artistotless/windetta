﻿using AutoFixture;
using Moq;
using Windetta.Common.Testing;
using Windetta.TonTxns.Infrastructure.Models;
using Windetta.TonTxns.Infrastructure.Services;

namespace Windetta.TonTxnsTests.Mocks;

internal class WalletCredentialSourceMock : MockInitializator<IWalletCredentialSource>
{
    protected override void Setup(Mock<IWalletCredentialSource> mock)
    {
        mock.Setup(x => x.GetCredential())
            .ReturnsAsync(new Fixture().Create<TonWalletCredential>());
    }
}