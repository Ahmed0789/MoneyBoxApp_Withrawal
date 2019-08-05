using System;
using Xunit;

using Moq;

using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;

namespace Moneybox.App.Test
{
    public class FeaturesTest
    {
        Mock<IAccountRepository> _mockAccountRepository;
        Mock<INotificationService> _mockNotificationService;

        Account _fromAccount;
        Account _toAccount;

        public FeaturesTest()
        {
            this._mockAccountRepository = new Mock<IAccountRepository>();
            this._mockNotificationService = new Mock<INotificationService>();

            var userFrom = new User()
            {
                Id = Guid.NewGuid(),
                Name = "UserFrom",
                Email = "userfrom@moneybox.com"
            };

            var userTo = new User()
            {
                Id = Guid.NewGuid(),
                Name = "UserTo",
                Email = "userto@moneybox.com"
            };

            this._fromAccount = new Account()
            {
                Id = Guid.NewGuid(),
                Balance = 1000m,
                PaidIn = 2000m,
                Withdrawn = 1000m,
                User = userFrom
            };

            this._toAccount = new Account()
            {
                Id = Guid.NewGuid(),
                Balance = 1000m,
                PaidIn = 3000m,
                Withdrawn = 2000m,
                User = userTo
            };

            this._mockAccountRepository.Setup(x => x.GetAccountById(this._fromAccount.Id)).Returns(() => this._fromAccount);
            this._mockAccountRepository.Setup(x => x.GetAccountById(this._toAccount.Id)).Returns(() => this._toAccount);
        }
        [Fact]
        public void Dispose()
        {

        }

        [Fact]
        public void Withdraw_Insufficient_Funds()
        {
            var withdrawAmount = 1001m;

            var withdrawMoneyFeature = new WithdrawMoney(_mockAccountRepository.Object, _mockNotificationService.Object);

            Assert.Throws<InvalidOperationException>(() => withdrawMoneyFeature.Execute(this._fromAccount.Id, withdrawAmount));
        }

        [Fact]
        public void Withdraw_Sufficient_Funds()
        {
            var withdrawAmount = 1000m;

            var withdrawMoneyFeature = new WithdrawMoney(_mockAccountRepository.Object, _mockNotificationService.Object);

            withdrawMoneyFeature.Execute(this._fromAccount.Id, withdrawAmount);
        }

        [Fact]
        public void Transfer_PayIn_Limit()
        {
            var transferAmount = 1001m;

            var transferMoneyFeature = new TransferMoney(_mockAccountRepository.Object, _mockNotificationService.Object);

            Assert.Throws<InvalidOperationException>(() => transferMoneyFeature.Execute(this._fromAccount.Id, this._toAccount.Id, transferAmount));
        }

        [Fact]
        public void Transfer_Successful()
        {
            var transferAmount = 1000m;

            var transferMoneyFeature = new TransferMoney(_mockAccountRepository.Object, _mockNotificationService.Object);

            transferMoneyFeature.Execute(this._fromAccount.Id, this._toAccount.Id, transferAmount);
        }
    }
}
