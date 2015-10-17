using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stemmers;

namespace TestBengaliStemmer
{
    class Program
    {
        static void Main(string[] args)
        {
            BengaliStemmer stemmer = new BengaliStemmer();
            string word = "বলিভিয়ানদের";
            //put a breakpoint here and check the root word. Console may not display unicode characters properly.
            string root = stemmer.Stem(word);
            Console.WriteLine("Press any key to exit.........");
            Console.ReadLine();
        }
    }
}
