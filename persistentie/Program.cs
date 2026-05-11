using System.Security.Cryptography.X509Certificates;
using Models;

namespace persistentie
{
    public class Program
    {
        public Program()
        {
            Console.WriteLine("hi");
            test();
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("hi");
            test();
        }
        public static void test()
        {
            PersistenceObject po = new PersistenceObject();
            List<Subject> subjects = po.Give_all_subjects();    //todate there is nothign returned, probably because of the category mismatch
            int i = 0;
        }
    }
}
