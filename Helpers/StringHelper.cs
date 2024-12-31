namespace STLServerlessNET.Helpers;

public static class StringHelper
{
    public static string StripIncompatableQuotes(this string inputStr)
    {
        if (string.IsNullOrWhiteSpace(inputStr))
        {
            return inputStr;
        }

        return inputStr.Replace('\u2018', '\'').Replace('\u2019', '\'').Replace('\u201c', '\"').Replace('\u201d', '\"').Replace("*", "");
    }
}