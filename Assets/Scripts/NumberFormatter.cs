using System.Text;

public static class NumberFormatter
{
    public static string FormatLargeNumber(double number)
    {
        StringBuilder formattedNumber = new StringBuilder();

        if (number >= 1e36) // Vigintillion
        {
            formattedNumber.Append((number / 1e36).ToString("F2")).Append(" V"); // Vigintillion
        }
        else if (number >= 1e33) // Decillion
        {
            formattedNumber.Append((number / 1e33).ToString("F2")).Append(" D"); // Decillion
        }
        else if (number >= 1e30) // Nonillion
        {
            formattedNumber.Append((number / 1e30).ToString("F2")).Append(" N"); // Nonillion
        }
        else if (number >= 1e27) // Octillion
        {
            formattedNumber.Append((number / 1e27).ToString("F2")).Append(" O"); // Octillion
        }
        else if (number >= 1e24) // Septillion
        {
            formattedNumber.Append((number / 1e24).ToString("F2")).Append(" S"); // Septillion
        }
        else if (number >= 1e21) // Sextillion
        {
            formattedNumber.Append((number / 1e21).ToString("F2")).Append(" Sx"); // Sextillion
        }
        else if (number >= 1e18) // Quintillion
        {
            formattedNumber.Append((number / 1e18).ToString("F2")).Append(" Q"); // Quintillion
        }
        else if (number >= 1e15) // Quadrillion
        {
            formattedNumber.Append((number / 1e15).ToString("F2")).Append(" Qa"); // Quadrillion
        }
        else if (number >= 1e12) // Trillion
        {
            formattedNumber.Append((number / 1e12).ToString("F2")).Append(" T"); // Trillion
        }
        else if (number >= 1e9) // Billion
        {
            formattedNumber.Append((number / 1e9).ToString("F2")).Append(" B"); // Billion
        }
        else if (number >= 1e6) // Million
        {
            formattedNumber.Append((number / 1e6).ToString("F2")).Append(" M"); // Million
        }
        else if (number >= 1e3) // Thousand
        {
            formattedNumber.Append((number / 1e3).ToString("F2")).Append(" K"); // Thousand
        }
        else
        {
            formattedNumber.Append(number.ToString("F2")); // Less than a thousand, two decimal places
        }

        return formattedNumber.ToString();
    }
}
