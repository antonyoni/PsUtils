using System.Collections.Generic;
using System.Text;

namespace PsUtils {

    public class KMP {

        /// <summary>
        /// Creates a lookup for partial matches, also known as a failure function.
        /// </summary>
        /// <param name="pattern">The pattern for which to create the table.</param>
        /// <returns>Array of integers representing the failure function for each byte in <paramref name="pattern"/>.</returns>
        public static int[] NewFailureFunction(byte[] pattern) {

            int[] table = new int[pattern.Length];

            table[0] = -1;

            if (pattern.Length == 1)
                return table;

            table[1] = 0;

            int pos = 2;
            int nextChar = 0;

            while (pos < pattern.Length) {
                if (pattern[pos - 1] == pattern[nextChar]) {
                    table[pos] = nextChar + 1;
                    ++nextChar;
                    ++pos;
                } else if (nextChar > 0) {
                    nextChar = table[nextChar];
                } else {
                    table[pos] = 0;
                    ++pos;
                }
            }

            return table;
        }

        /// <summary>
        /// Uses the Knuth-Morris-Pratt algorithm to find all occurences of pattern in input.
        /// </summary>
        /// <remarks>Does not find overlapping patterns.</remarks>
        /// <param name="input">Byte array to search.</param>
        /// <param name="pattern">Byte array to find in <paramref name="input"/>.</param>
        /// <returns>Array of integers with each integer corresponding to the start of a found <paramref name="pattern"/>.</returns>
        public static int[] FindAll(byte[] input, byte[] pattern) {

            List<int> found = new List<int>();
            int[] table = NewFailureFunction(pattern);


            int posMatch = 0;
            int posChar  = 0;

            while (posMatch + posChar < input.Length) {
                if (pattern[posChar] == input[posMatch + posChar]) {
                    if (posChar == pattern.Length - 1) {
                        found.Add(posMatch);
                        posMatch += posChar + 1; // Skip ahead to end of match.
                        posChar = 0;
                    } else {
                        ++posChar;
                    }
                } else {
                    if (table[posChar] > -1) {
                        posMatch += posChar - table[posChar];
                        posChar = table[posChar];
                    } else {
                        ++posMatch;
                        posChar = 0;
                    }
                }
            }

            return found.ToArray();
        }

        /// <summary>
        /// Uses the Knuth-Morris-Pratt algorithm to find all occurences of pattern in input. Uses a default encoding of <see cref="Encoding.UTF8"/> to convert the strings to byte arrays.
        /// </summary>
        /// <param name="input">String to search.</param>
        /// <param name="pattern">String to find in <paramref name="input"/>.</param>
        /// <returns>Array of integers with each integer corresponding to the start of a found <paramref name="pattern"/>.</returns>
        public static int[] FindAll(string input, string pattern) {
            return FindAll(input, pattern, Encoding.UTF8);
        }

        /// <summary>
        /// Uses the Knuth-Morris-Pratt algorithm to find all occurences of pattern in input.
        /// </summary>
        /// <param name="input">String to search.</param>
        /// <param name="pattern">String to find in <paramref name="input"/>.</param>
        /// <param name="encoding">The encoding to use to convert the input and pattern into a byte array.</param>
        /// <returns>Array of integers with each integer corresponding to the start of a found <paramref name="pattern"/>.</returns>
        public static int[] FindAll(string input, string pattern, Encoding encoding) {
            var div = encoding.GetByteCount(pattern) / pattern.Length;
            var found = FindAll(encoding.GetBytes(input), encoding.GetBytes(pattern));
            if (div > 1) {
                for (int i = 0; i < found.Length; i++)
                    found[i] /= div;
            }
            return found;
        }

    }

}
