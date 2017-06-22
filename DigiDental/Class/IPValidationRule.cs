using System;
using System.Globalization;
using System.Windows.Controls;

namespace DigiDental.Class
{
    class IPValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (ValidatorHelper.IsIP(value.ToString()))
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, "IP格式有誤");
                }
            }
            catch(Exception ex)
            {
                return new ValidationResult(false, ex.ToString());
            }
        }
    }
}
