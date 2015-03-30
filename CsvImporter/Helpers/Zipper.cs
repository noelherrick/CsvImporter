using System;
using System.Collections.Generic;

namespace CsvImporter
{
	/// <summary>
	/// Static class to hold the Zip extension method.
	/// </summary>
    public static class Zipper
    {
		/// <summary>
		/// Zip two IEnumerables together using the specified function.
		/// </summary>
		/// <param name="ienum1">First IEnumerable. This will be the first argument to the function.</param>
		/// <param name="ienum2">Second IEnumerable. This will be the second argument to the function.</param>
		/// <param name="zipper">Zipper. The function that takes two parameters of type T and returns an object of type Y</param>
		/// <typeparam name="T">The type of two IEnumerables.</typeparam>
		/// <typeparam name="Y">The return type of the function.</typeparam>
        public static IEnumerable<Y> Zip<T,Y> (this IEnumerable<T> ienum1, IEnumerable<T> ienum2, Func<T, T, Y> zipper)
        {
            var enumer1 = ienum1.GetEnumerator ();
            var enumer2 = ienum2.GetEnumerator ();

            while (enumer1.MoveNext () && enumer2.MoveNext ()) {
                yield return zipper(enumer1.Current, enumer2.Current);
            }

            yield break;
        }
    }
}

