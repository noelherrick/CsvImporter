using System;
using System.Collections.Generic;

namespace CsvImporter
{
    public static class Zipper
    {
        public static IEnumerable<Y> Zip<Y,T> (this IEnumerable<T> ienum1, IEnumerable<T> ienum2, Func<T, T, Y> zipper)
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

