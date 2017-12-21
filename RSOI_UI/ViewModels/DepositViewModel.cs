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
        public Deposit Deposit { get; set; }

        public DepositViewModel(Deposit deposit)
        {
            Deposit = deposit;

            var accountOperations = Transaction.GetListOfTransactionsByAccountId(deposit.CurrentAccount.Id).ToList();
            accountOperations.AddRange(Transaction.GetListOfTransactionsByAccountId(deposit.InterestAccount.Id));

            ListOfTransactions = new ObservableCollection<Transaction>(accountOperations.OrderBy(item=>item.DateTime));
        }

        public IList<Transaction> ListOfTransactions { get; set; }


    }
}
