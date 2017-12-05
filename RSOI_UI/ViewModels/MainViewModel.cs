using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using RSOI_UI.Views;

namespace RSOI_UI.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        #region Commands

        private DelegateCommand _openNewClientWindowCommand;
        public DelegateCommand OpenNewClientWindowCommand => _openNewClientWindowCommand ??
                                                      (_openNewClientWindowCommand = new DelegateCommand(OpenNewClientWindowExecute));

        #endregion

        #region Command Handlers
        

        private void OpenNewClientWindowExecute()
        {
            var newClientWindow = new NewClientView
            {
                DataContext = new NewClientViewModel()
            };

            newClientWindow.ShowDialog();
        }

        #endregion
    }
}
