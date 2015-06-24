namespace TrainNotifier.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Truncates the string to the max length.
        /// If the string is longer than <paramref name="maxChars"/> then the string
        /// is returned at <paramref name="maxChars"/> -3 length with ... appended
        /// </summary>
        /// <param name="value">string to truncate</param>
        /// <param name="maxChars">max number of characters to return</param>
        /// <returns>the string, or truncated if necessary</returns>
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars-3) + "...";
        }
    }
}
