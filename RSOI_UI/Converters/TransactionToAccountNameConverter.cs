using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using RSOI_Data.Entities;

namespace RSOI_UI.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    class TransactionToAccountNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Transaction && parameter is string)
            {
                var transaction = (Transaction)value;
                var param = (string)parameter;

                switch (transaction.TransactionType.Name)
                {
                    case "Приход (а/п)":
                        if (param == "D")
                        {
                            return transaction.AccountId1;
                        }
                        else
                        {
                            return transaction.AccountId2;
                        }
                    case "Приход/Расход (п)":
                        if (param == "D")
                        {
                            return transaction.AccountId2;
                        }
                        else
                        {
                            return transaction.AccountId1;
                        }
                    case "Расход (а/п)":
                        if (param == "D")
                        {
                            return transaction.AccountId2;
                        }
                        else
                        {
                            return transaction.AccountId1;
                        }
                }
            }
            

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
