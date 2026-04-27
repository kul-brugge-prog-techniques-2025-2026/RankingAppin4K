using Models;
using persistentie;
using System.Runtime.InteropServices;

namespace business
{
    public class Program
    {
        public Program() 
        {
            Console.WriteLine("hi");
        }

        public static void Main(string[] args) {
            Business bi = new Business(1, null);
            subjectItem[] items;
            while( (items = bi.Give_options())!= null)
            {
                if (Convert.ToInt32(items[0].Text[0]) < Convert.ToInt32(items[1].Text[0]))
                    items = new subjectItem[] { items[1], items[0] };
                bi.Give_result(items, false);
            }
            ;
            foreach(var item in bi.GetFinalRankedList())
            {
                Console.WriteLine($"{item.Rank}: {item.subjectitem.Text[0]}");
            }
        }
    }
}
