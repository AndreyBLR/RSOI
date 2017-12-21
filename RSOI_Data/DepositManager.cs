using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RSOI_Data.Enums;

namespace RSOI_Data.Entities
{
    public class DepositManager
    {
        #region Constants

        private string _charsToUse = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        #region Transaction Types

        private readonly TransactionType _income = new TransactionType()
        {
            Id = 3,
            Name = "Приход (а/п)"
        };

        private readonly TransactionType _incomeExpensePassive = new TransactionType()
        {
            Id = 4,
            Name = "Приход/Расход (п)"
        };

        private readonly TransactionType _expense = new TransactionType()
        {
            Id = 5,
            Name = "Расход (а/п)"
        };
        
        #endregion

        private readonly ContractType _depositContactType = new ContractType()
        {
            Id = 1,
            Name = "Депозит"
        };

        #region Accounts

        private readonly AccountType _currentAccountType = new AccountType()
        {
            Id = 3,
            Name = "Текущий счёт"
        };

        private readonly AccountType _interestAccountType = new AccountType()
        {
            Id = 4,
            Name = "Процентный счёт"
        };

        private readonly AccountType _cashOfficeAccountType = new AccountType()
        {
            Id = 5,
            Name = "Касса"
        };

        private readonly AccountType _bankFundAccountType = new AccountType()
        {
            Id = 6,
            Name = "Фонд развития банка"
        };

        #endregion

        #endregion

        private static DepositManager _depositManager;
        public static DepositManager Instance
        {
            get
            {
                if(_depositManager == null)
                    _depositManager = new DepositManager();

                return _depositManager;
            }
        }

        public static double CalculateDepositIncomeByDailyRate(Deposit deposit)
        {
            var dailyRate = deposit.Rate / 365;

            return (deposit.Amount + deposit.InterestAccount.Amount) / 100 * dailyRate;
        }

        private DepositManager()
        {
        }

        public Deposit OpenDeposit(Client client, DepositType depositType)
        {
            var generatedNumber = GenerateContractNumber();
            
            var currentAccount = new Account
            {
                CurrencyType = depositType.CurrencyType,
                Amount = 0,
                AccountType = _currentAccountType,
                IsActive = false
            };

            var interestAccount = new Account
            {
                CurrencyType = depositType.CurrencyType,
                Amount = 0,
                AccountType = _interestAccountType,
                IsActive = false
            };

            var contract = new Contract
            {
                Client = client,
                Number = generatedNumber,
                ContractType = _depositContactType,
                IsClosed = false,
                EarlyClosing = depositType.WithWithdraw
            };

            var deposit = new Deposit
            {
                CurrencyType = depositType.CurrencyType,
                OpenDate = DateTime.Now,
                Number = generatedNumber,
                Rate = depositType.Rate,
                Amount = 0,
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

        public bool CloseDeposit(Deposit deposit)
        {
            try
            {
                var contract = Contract.GetContractByNumber(deposit.Number);
                contract.IsClosed = true;

                contract.Update();
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                return false;
            }

            return true;
        }

        public bool AddToCurrentAccount(Deposit deposit, double sum, string passportSNumber, string fio)
        {
            var payerInfo = new PayerInfo()
            {
                FIO = fio,
                PassportSNumber = passportSNumber
            };

            return TransactionFromCashOfficeToCurrentAccount(deposit, sum, "Внесение средств на депозит", payerInfo);
        }

        public bool AddToInterestAccounts(Deposit deposit, double sum)
        {
            TransactionFromBankFundToInterestAccount(deposit, sum, "Начисление средств на процентный счёт депозита");

            return true;
        }

        public bool WithdrayFromCurrentAccount(Deposit deposit, double sum)
        {
            var result = TransactionFromBankFundToCurrentAccount(deposit, sum, "Перевод средств с банковского фонда на основной счёт депозита.");
            result = result && TransactionFromСurrentAccountToCashOffice(deposit, sum, "Снятие средств с основного счёта депозита");
           
            return result;
        }

        public bool WithdrayFromInterestAccount(Deposit deposit, double sum)
        {
            return TransactionFromInterestAccountToCashOffice(deposit, sum, "Снятие средств с процентного счёта депозита");
        }
        
        public void ZeroyingInterestAccount(Deposit deposit, string description)
        {
            TransactionFromInterestAccountToBankFund(deposit, deposit.InterestAccount.Amount, description);
        }
        
        #region  Transactions

        private bool TransactionFromCashOfficeToCurrentAccount(Deposit deposit, double sum, string description, PayerInfo payerInfo)
        {
            var cashOfficeAccount = Account.GetAccountByTypeAndCurrency(_cashOfficeAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(cashOfficeAccount, deposit.CurrentAccount, _income, sum, description, payerInfo))
            {
                cashOfficeAccount.Amount += sum;
                cashOfficeAccount.Update();

                deposit.CurrentAccount.Amount += sum;
                deposit.CurrentAccount.Update();

                deposit.Amount += sum;
                deposit.Update();

                return true;
            }

            return false;
        }

        private bool TransactionFromCurrentAccountToBankFund(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency(_bankFundAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(bankFundAccount, deposit.CurrentAccount, _incomeExpensePassive, sum, description))
            {
                bankFundAccount.Amount += sum;
                bankFundAccount.Update();

                deposit.CurrentAccount.Amount -= sum;
                deposit.CurrentAccount.Update();

                return true;
            }

            return false;
        }
        
        private bool TransactionFromBankFundToInterestAccount(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency(_bankFundAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(deposit.InterestAccount, bankFundAccount, _incomeExpensePassive, sum, description))
            {
                deposit.InterestAccount.Amount += sum;
                deposit.InterestAccount.Update();

                bankFundAccount.Amount -= sum;
                bankFundAccount.Update();

                return true;
            }

            return false;
        }

        private bool TransactionFromInterestAccountToCashOffice(Deposit deposit, double sum, string description)
        {
            var cashOfficeAccount = Account.GetAccountByTypeAndCurrency(_cashOfficeAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(deposit.InterestAccount, cashOfficeAccount, _expense, sum, description))
            {
                cashOfficeAccount.Amount -= sum;
                cashOfficeAccount.Update();

                deposit.InterestAccount.Amount -= sum;
                deposit.InterestAccount.Update();

                return true;
            }

            return false;
        }

        private bool TransactionFromСurrentAccountToCashOffice(Deposit deposit, double sum, string description)
        {
            var cashOfficeAccount = Account.GetAccountByTypeAndCurrency(_cashOfficeAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(deposit.CurrentAccount, cashOfficeAccount, _expense, sum, description))
            {
                cashOfficeAccount.Amount -= sum;
                cashOfficeAccount.Update();

                deposit.CurrentAccount.Amount -= sum;
                deposit.CurrentAccount.Update();

                deposit.Amount -= sum;
                deposit.Update();

                return true;
            }

            return false;
        }

        private bool TransactionFromInterestAccountToBankFund(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency(_bankFundAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(bankFundAccount, deposit.InterestAccount, _incomeExpensePassive, sum, description))
            {
                bankFundAccount.Amount += sum;
                bankFundAccount.Update();

                deposit.InterestAccount.Amount -= sum;
                deposit.InterestAccount.Update();

                return true;
            }

            return false;
        }
        
        private bool TransactionFromBankFundToCurrentAccount(Deposit deposit, double sum, string description)
        {
            var bankFundAccount = Account.GetAccountByTypeAndCurrency(_bankFundAccountType.Id, deposit.CurrencyType.Id);

            if (CreateTransaction(deposit.CurrentAccount, bankFundAccount, _incomeExpensePassive, sum, description))
            {
                deposit.CurrentAccount.Amount += sum;
                deposit.CurrentAccount.Update();

                bankFundAccount.Amount -= sum;
                bankFundAccount.Update();

                return true;
            }

            return false;
        }

        private bool CreateTransaction(Account account1, Account account2, TransactionType transactionType, double sum, string description)
        {
            try
            {
                var operation = new Transaction()
                {
                    AccountId1 = account1,
                    AccountId2 = account2,
                    Description = description,
                    TransactionType = transactionType,
                    Sum = sum,
                    DateTime = DateTime.Now
                };

                operation.Insert();

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                return false;
            }
        }

        private bool CreateTransaction(Account account1, Account account2, TransactionType transactionType, double sum, string description, PayerInfo payerInfo)
        {
            try
            {
                var operation = new Transaction()
                {
                    AccountId1 = account1,
                    AccountId2 = account2,
                    Description = description,
                    TransactionType = transactionType,
                    Sum = sum,
                    DateTime = DateTime.Now,
                    PayerInfo = payerInfo
                };

                operation.Insert();

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                return false;
            }
        }


        #endregion

        private string GenerateContractNumber()
        {
            var rn = new Random();
            return Regex.Replace("XXXXXX", "X", (Match m)=> _charsToUse[rn.Next(_charsToUse.Length)].ToString());
        }
    }
}
