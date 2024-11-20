using System.Globalization;
using System.Text;

namespace RentACarDotNetCore.Utilities.StringMethods
{
    public class StringConverter : IStringConverter
    {
        public string ConvertTRCharToENChar(string text)
        {
            return String.Join("", text.Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }
    }
}
