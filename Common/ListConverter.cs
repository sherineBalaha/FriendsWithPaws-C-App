using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//added statements
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;

namespace FriendsWithPaws.Common
{
    class ListConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ObservableCollection<string> items =  (ObservableCollection<string>) value;
            StringBuilder builder = new StringBuilder();
            foreach (var item in items)
            {
                builder.Append(item);
                builder.Append("\r\n");
                
            }
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
