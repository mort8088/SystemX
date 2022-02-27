// -----------------------------------------------------------------------
// <copyright file="StringBuilderExtensions.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Text;

namespace SystemX.Extensions {
    /// <summary>
    ///     Options for StringBuilder extension methods.
    /// </summary>
    [Flags]
    public enum AppendNumberOptions {
        // Normal format.
        None = 0,

        // Added "+" sign for positive value.
        PositiveSign = 1,

        // Insert Number group separation characters.
        // In Use, added "," for every 3 digits.
        NumberGroup = 2
    }

    /// <summary>
    ///     Static class for string builder extension methods.
    /// </summary>
    /// <remarks>
    ///     You can specified StringBuilder for SpriteFont.DrawString from XNA GS 3.0.
    ///     And you can save unwanted memory allocations.
    ///     But there are still problems for adding numerical value to StringBuilder.
    ///     One of them is boxing occurred when you use StringBuilder.AppendFormat method.
    ///     Another issue is memory allocation occurred when you specify int or float for
    ///     StringBuild.Append method.
    ///     This class provides solution for those issue.
    ///     All methods are defined as extension methods as StringBuilder. So, you can use
    ///     those method like below.
    ///     stringBuilder.AppendNumber(12345);
    /// </remarks>
    public static class StringBuilderExtensions {
        /// <summary>
        ///     Convert integer to string and add to string builder.
        /// </summary>
        public static void AppendNumber(this StringBuilder builder, int number) {
            AppendNumbernternal(builder, number, 0, AppendNumberOptions.None);
        }

        /// <summary>
        ///     Convert integer to string and add to string builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="number"></param>
        /// <param name="options">Format options</param>
        public static void AppendNumber(this StringBuilder builder, int number, AppendNumberOptions options) {
            AppendNumbernternal(builder, number, 0, options);
        }

        /// <summary>
        ///     Convert float to string and add to string builder.
        /// </summary>
        /// <remarks>It shows 2 decimal digits.</remarks>
        public static void AppendNumber(this StringBuilder builder, float number) {
            AppendNumber(builder, number, 2, AppendNumberOptions.None);
        }

        /// <summary>
        ///     Convert float to string and add to string builder.
        /// </summary>
        /// <remarks>It shows 2 decimal digits.</remarks>
        public static void AppendNumber(this StringBuilder builder, float number, AppendNumberOptions options) {
            AppendNumber(builder, number, 2, options);
        }

        /// <summary>
        ///     Convert float to string and add to string builder.
        /// </summary>
        public static void AppendNumber(this StringBuilder builder, float number, int decimalCount, AppendNumberOptions options) {
            // Handle NaN, Infinity cases.
            if (float.IsNaN(number)) builder.Append("NaN");
            else if (float.IsNegativeInfinity(number)) builder.Append("-Infinity");
            else if (float.IsPositiveInfinity(number)) builder.Append("+Infinity");
            else {
                int intNumber =
                    (int)(number * (float)Math.Pow(10, decimalCount) + 0.5f);

                AppendNumbernternal(builder, intNumber, decimalCount, options);
            }
        }

        private static void AppendNumbernternal(StringBuilder builder, int number, int decimalCount, AppendNumberOptions options) {
            // Initialize variables for conversion.
            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;

            int idx = NumberString.Length;
            int decimalPos = idx - decimalCount;

            if (decimalPos == idx)
                decimalPos = idx + 1;

            int numberGroupIdx = 0;
            int numberGroupCount = NumberGroupSizes[numberGroupIdx] + decimalCount;

            bool showNumberGroup = (options & AppendNumberOptions.NumberGroup) != 0;
            bool showPositiveSign = (options & AppendNumberOptions.PositiveSign) != 0;

            bool isNegative = number < 0;
            number = Math.Abs(number);

            // Converting from smallest digit.
            do {
                // Add decimal separator ("." in US).
                if (idx == decimalPos) NumberString[--idx] = nfi.NumberDecimalSeparator[0];

                // Added number group separator ("," in US).
                if (--numberGroupCount < 0 && showNumberGroup) {
                    NumberString[--idx] = nfi.NumberGroupSeparator[0];

                    if (numberGroupIdx < NumberGroupSizes.Length - 1)
                        numberGroupIdx++;

                    numberGroupCount = NumberGroupSizes[numberGroupIdx] - 1;
                }

                // Convert current digit to character and add to buffer.
                NumberString[--idx] = (char)('0' + (number % 10));
                number /= 10;
            } while (number > 0 ||
                     decimalPos <= idx);

            // Added sign character if needed.
            if (isNegative) NumberString[--idx] = nfi.NegativeSign[0];
            else if (showPositiveSign) NumberString[--idx] = nfi.PositiveSign[0];

            // Added converted string to StringBuilder.
            builder.Append(NumberString, idx, NumberString.Length - idx);
        }

        #region Fields
        /// <summary>
        ///     Cache for NumberGroupSizes of NumberFormat class.
        /// </summary>
        private static readonly int[] NumberGroupSizes = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSizes;

        /// <summary>
        ///     string conversion buffer.
        /// </summary>
        private static readonly char[] NumberString = new char[32];
        #endregion
    }
}