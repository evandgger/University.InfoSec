using System;
using InformationSecurity.Lab_2.Affine;
using InformationSecurity.Lab_2.Caesar;

namespace InformationSecurity.Lab_2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var caesarDemo = new CaesarDemo();
            var affineDemo = new AffineDemo();

            var userInput = string.Empty;

            while (userInput != Constants.StopText)
            {
                Console.Clear();
                Console.WriteLine("Eugene Gerasimov.InfoSec.LAB_2\n");
                Console.WriteLine("Available Ciphers:");
                Console.WriteLine("0. Exit from program");
                Console.WriteLine("1. Caesar");
                Console.WriteLine("2. Affine\n");
                Console.Write("Select cipher: ");
                userInput = Console.ReadLine();
                try
                {
                    var chosenCipher = (Cipher) Convert.ToInt32(userInput);
                    switch (chosenCipher)
                    {
                        case 0:
                            break;
                        case Cipher.Caesar:
                            caesarDemo.Show();
                            break;
                        case Cipher.Affine:
                            affineDemo.Show();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
                catch
                {
                    Console.WriteLine("\nError. Incorrect input");
                    return;
                }
            }
        }
    }
}