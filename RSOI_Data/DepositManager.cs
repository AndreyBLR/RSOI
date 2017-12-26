using System;
using System.Text.RegularExpressions;
using System.Threading;
using RSOI_Data.Entities;
using RSOI_Data.Enums;

namespace RSOI_Data
{
    public class DepositManager
    {
        private Credentials _currentOperator;

        private static DepositManager _depositManager;

        public static DepositManager Instance
        {
            get
            {
                if (_depositManager == null)
                    _depositManager = new DepositManager();

                return _depositManager;
            }
        }

        public static double CalculateDepositIncomeByDailyRate(Deposit deposit)
        {
            var dailyRate = deposit.Rate / 365;

            return (deposit.AvailableAmount + deposit.InterestAccount.Amount) / 100 * dailyRate;
        }

        public static double CalculateDepositIncomeByMinRate(Deposit deposit, int days)
        {
            double result = 0;
            double _minRate = 0.1;

            for (int i = 0; i < days; i++)
            {
                result += (deposit.AvailableAmount + deposit.InterestAccount.Amount + result) / 100 * _minRate;
            }

            return result;
        }

        public void SetCurrentOperator(Credentials currentOperator)
        {
            _currentOperator = currentOperator;
        }

        public Deposit OpenDeposit(Client client, DepositType depositType)
        {
            var generatedNumber = GenerateContractNumber();

            var currentAccount = new Account
            {
                CurrencyType = depositType.CurrencyType,
                Amount = 0,
                Number = generatedNumber,
                AccountType = new AccountType() {Id = (int) AccountTypeIds.Current},
                IsActive = false
            };

            var interestAccount = new Account
            {
                CurrencyType = depositType.CurrencyType,
                Amount = 0,
                Number = generatedNumber,
                AccountType = new AccountType() {Id = (int) AccountTypeIds.Interest},
                IsActive = false
            };

            var contract = new Contract
            {
                Client = client,
                Number = generatedNumber,
                ContractType = new ContractType() {Id = (int) ContractTypeIds.Deposit},
                IsClosed = false,
                EarlyClosing = depositType.WithWithdraw,
                Operator = _currentOperator
            };

            var deposit = new Deposit
            {
                CurrencyType = depositType.CurrencyType,
                OpenDate = DateTime.Now,
                Number = generatedNumber,
                Rate = depositType.Rate,
                AvailableAmount = 0,
                CurrentAccount = currentAccount,
                InterestAccount = interestAccount
            };

            deposit.CloseDate = deposit.OpenDate.AddMonths(depositType.TermInMonth);

            currentAccount.Insert();
            interestAccount.Insert();
            contract.Insert();
            deposit.Insert();

            return deposit;
        }

        public void CloseDeposit(Deposit deposit, bool earlyClose)
        {
            if (earlyClose)
            {
                var days = (DateTime.Now - deposit.OpenDate).Days;

                var recalculatedInterestAmount = CalculateDepositIncomeByMinRate(deposit, days);
                TransactionFromInterestAccountToBankFund(deposit, deposit.InterestAccount.Amount,
                    "Возврат средств процентного счёта в фонд банка в связи с досрочным закрытием депозита");
                TransactionFromBankFundToInterestAccount(deposit, recalculatedInterestAmount,
                    "Начисление средств на процентный счёт депозита по мин. ставке 0.01%");

                if (deposit.WithdrawAmount > 0)
                {
                    TransactionFromСurrentAccountToBankFund(deposit, deposit.WithdrawAmount, "Возврат суммы уже снятых процентов в фонд банка.");
                }

                var forWithdrawFromCurrent = deposit.AvailableAmount - deposit.WithdrawAmount;
                var forWithdrawFromInterest = deposit.InterestAccount.Amount;
                WithdrawFromCurrentAccount(deposit, forWithdrawFromCurrent);
                WithdrawFromInterestAccount(deposit, forWithdrawFromInterest);
            }
            else
            {
                WithdrawFromCurrentAccount(deposit, deposit.AvailableAmount);
                WithdrawFromInterestAccount(deposit, deposit.InterestAccount.Amount);
            }

            var contract = Contract.GetContractByNumber(deposit.Number);
            contract.Operator = _currentOperator;
            contract.IsClosed = true;

            contract.Update();
        }
        
        public void TransferToCurrentAccount(Deposit deposit, double sum, string passportSNumber, string fio)
        {
            var payerInfo = new PayerInfo()
            {
                FIO = fio,
                PassportSNumber = passportSNumber
            };

            TransactionFromCashOfficeToCurrentAccount(deposit, sum, "Внесение средств на депозит", payerInfo);
            TransactionFromСurrentAccountToBankFund(deposit, sum, "Перевод средств на счёт банковского фонда");
        }

        public void TransferToInterestAccounts(Deposit deposit, double sum)
        {
            TransactionFromBankFundToInterestAccount(deposit, sum, "Начисление средств на процентный счёт депозита");
        }

        public void WithdrawFromCurrentAccount(Deposit deposit, double sum)
        {
            TransactionFromBankFundToCurrentAccount(deposit, sum, "Перевод средств с банковского фонда на основной счёт депозита");
            TransactionFromСurrentAccountToCashOffice(deposit, sum, "Списание средств с основного счёта депозита");
        }

        public void WithdrawFromInterestAccount(Deposit deposit, double sum)
        {
            TransactionFromInterestAccountToCashOffice(deposit, sum, "Списание средств с процентного счёта депозита");
        }

        #region  Transactions

        private void TransactionFromCashOfficeToCurrentAccount(Deposit deposit, double sum, string description, PayerInfo payerInfo)
        {
            var cashOfficeAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.CashOffice, deposit.CurrencyType.Id);

            CreateTransaction(cashOfficeAccount, deposit.CurrentAccount, TransactionTypeIds.Income, sum, description, payerInfo);

            cashOfficeAccount.Amount += sum;
            cashOfficeAccount.Update();

            deposit.CurrentAccount.Amount += sum;
            deposit.CurrentAccount.Update();

            deposit.AvailableAmount += sum;
            deposit.Update();
        }

        private void TransactionFromBankFundToInterestAccount(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.BankFund, deposit.CurrencyType.Id);

            CreateTransaction(deposit.InterestAccount, bankFundAccount, TransactionTypeIds.IncomeExpense, sum,description);

            deposit.InterestAccount.Amount += sum;
            deposit.InterestAccount.Update();

            bankFundAccount.Amount -= sum;
            bankFundAccount.Update();
        }

        private void TransactionFromInterestAccountToCashOffice(Deposit deposit, double sum, string description)
        {
            var cashOfficeAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.CashOffice, deposit.CurrencyType.Id);

            CreateTransaction(cashOfficeAccount, deposit.InterestAccount, TransactionTypeIds.Expense, sum, description);

            cashOfficeAccount.Amount -= sum;
            cashOfficeAccount.Update();

            deposit.InterestAccount.Amount -= sum;
            deposit.InterestAccount.Update();

            deposit.WithdrawAmount += sum;
            deposit.Update();
        }

        private void TransactionFromСurrentAccountToCashOffice(Deposit deposit, double sum, string description)
        {
            var cashOfficeAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.CashOffice, deposit.CurrencyType.Id);

            CreateTransaction(cashOfficeAccount, deposit.CurrentAccount, TransactionTypeIds.Expense, sum, description);

            cashOfficeAccount.Amount -= sum;
            cashOfficeAccount.Update();

            deposit.CurrentAccount.Amount -= sum;
            deposit.CurrentAccount.Update();

            deposit.AvailableAmount -= sum;
            deposit.Update();
        }

        private void TransactionFromСurrentAccountToBankFund(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.BankFund, deposit.CurrencyType.Id);

            CreateTransaction(bankFundAccount, deposit.CurrentAccount, TransactionTypeIds.IncomeExpense, sum, description);

            bankFundAccount.Amount += sum;
            bankFundAccount.Update();

            deposit.CurrentAccount.Amount -= sum;
            deposit.CurrentAccount.Update();
        }

        private void TransactionFromInterestAccountToBankFund(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.BankFund, deposit.CurrencyType.Id);

            CreateTransaction(bankFundAccount, deposit.InterestAccount, TransactionTypeIds.IncomeExpense, sum, description);

            bankFundAccount.Amount += sum;
            bankFundAccount.Update();

            deposit.InterestAccount.Amount -= sum;
            deposit.InterestAccount.Update();
        }

        private void TransactionFromBankFundToCurrentAccount(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency((int) AccountTypeIds.BankFund, deposit.CurrencyType.Id);

            CreateTransaction(deposit.CurrentAccount, bankFundAccount, TransactionTypeIds.IncomeExpense, sum, description);

            deposit.CurrentAccount.Amount += sum;
            deposit.CurrentAccount.Update();

            bankFundAccount.Amount -= sum;
            bankFundAccount.Update();
        }

        private void CreateTransaction(Account account1, Account account2, TransactionTypeIds transactionTypeId, double sum, string description)
        {
            var operation = new Transaction()
            {
                AccountId1 = account1,
                AccountId2 = account2,
                Description = description,
                TransactionType = new TransactionType() {Id = (int) transactionTypeId},
                Sum = sum,
                DateTime = DateTime.Now,
                Operator = _currentOperator
            };

            operation.Insert();
        }

        private void CreateTransaction(Account account1, Account account2, TransactionTypeIds transactionTypeId, double sum, string description, PayerInfo payerInfo)
        {
            var operation = new Transaction()
            {
                AccountId1 = account1,
                AccountId2 = account2,
                Description = description,
                TransactionType = new TransactionType() {Id = (int) transactionTypeId},
                Sum = sum,
                DateTime = DateTime.Now,
                PayerInfo = payerInfo,
                Operator = _currentOperator
            };

            operation.Insert();
        }

        #endregion

        private string GenerateContractNumber()
        {
            var _charsToUse = "1234567890";
            var rn = new Random();
            return Regex.Replace("XXXXXXXXX", "X", (Match m) => _charsToUse[rn.Next(_charsToUse.Length)].ToString());
        }
    }
}
