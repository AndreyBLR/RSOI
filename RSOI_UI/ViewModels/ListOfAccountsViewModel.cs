using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RSOI_Data.Entities;

namespace RSOI_UI.ViewModels
{
    public class ListOfAccountsViewModel : BaseViewModel
    {
        private IList<Account> _accounts;
        public IList<Account> Accounts
        {
            get => _accounts;
            set
            {
                _accounts = value;
                OnPropertyChanged();
            }
        }

        public ListOfAccountsViewModel()
        {
            Accounts = Account.GetListOfAccounts();
        }
    }
}
