namespace Stage0
{
    partial class Program
    {
        private static void Main(string[] args)
        {
            Welcome1652();
            Welcome9845();
            Console.ReadKey();
        }

        private static void Welcome1652()
        {
            Console.Write("Enter your name:");
            string name = Console.ReadLine();
            Console.WriteLine("{0}, Welcome to my first console application", name);
        }
        static partial void Welcome9845();
    }
}

