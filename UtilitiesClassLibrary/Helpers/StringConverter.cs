using System.Globalization;
using System.Text;

namespace UtilitiesClassLibrary.Helpers
{
	public class StringConverter : IStringConverter
	{
		public string ConvertTRCharToENChar(string text)
		{
			return string.Join("", text.Normalize(NormalizationForm.FormD)
			.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
		}
	}
}
