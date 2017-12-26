using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSOI_Data.Entities;

namespace RSOI_UI.ViewModels
{
    public class DepositViewModel : BaseViewModel
    {
        private Deposit _deposit;
        private IList<Transaction> _transactions;

        #region Presentation Properties

        public Deposit Deposit
        {
            get => _deposit;
            set
            {
                _deposit = value;
                OnPropertyChanged();
            }
        }

        public IList<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public DepositViewModel(Deposit deposit)
        {
            Deposit = deposit;

            var accountOperations = Transaction.GetListOfTransactionsByAccountId(deposit.CurrentAccount.Id).ToList();
            accountOperations.AddRange(Transaction.GetListOfTransactionsByAccountId(deposit.InterestAccount.Id));

            Transactions = new ObservableCollection<Transaction>(accountOperations.OrderBy(item=>item.Id));
        }
    }
}
