using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tip7
{
    class Program
    {
        static Week week;

        static void Main(string[] args)
        {
            Console.WriteLine(week);
        }

    }

    enum Week
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }

}
