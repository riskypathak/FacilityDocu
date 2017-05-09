using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Tablet_App
{
    public class VisibilityConverter : IValueConverter
    {
        //Convert the textbox value into Grades
        public object Convert(object value, System.Type type, object parameter, string language)
        {
            bool _value;
            Visibility _grade = Visibility.Collapsed;
            //try parsing the value to int
            if (Boolean.TryParse(value.ToString(), out _value))
            {
                if (_value == true)
                {
                    _grade = Visibility.Visible;
                }
                else{
                    _grade = Visibility.Collapsed;
                }
            }
            return _grade;
        }
        public object ConvertBack(object value, System.Type type, object parameter, string language)
        {
            throw new NotImplementedException(); //doing one-way binding so this is not required.
        }
    }
}
