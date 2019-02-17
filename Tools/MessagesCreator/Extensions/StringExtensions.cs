namespace MessagesCreator.Extensions
{
    public static class StringExtensions
    {

        public static string Capitalize(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();

            return $"{char.ToUpper(str[0])}{str.Substring(1)}";
        }

    }
}
