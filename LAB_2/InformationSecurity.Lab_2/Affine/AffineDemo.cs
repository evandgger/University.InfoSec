using System;
using System.Linq;

namespace InformationSecurity.Lab_2.Affine
{
    public class AffineDemo
    {
        public void Show()
        {
            var userInput = string.Empty;

            while (userInput != Constants.StopText)
            {
                Console.Clear();
                Console.WriteLine("Affine Crypt\n");
                Console.WriteLine("Available actions:");
                Console.WriteLine("0. Exit to main menu");
                Console.WriteLine("1. Encrypt");
                Console.WriteLine("2. Decrypt\n");
                Console.Write("Select action: ");
                userInput = Console.ReadLine();
                try
                {
                    var chosenAction = (Action) Convert.ToInt32(userInput);
                    switch (chosenAction)
                    {
                        case Action.Exit:
                            return;
                        case Action.Encrypt:
                            Console.Write("Enter a text: ");
                            var textForEncrypt = Console.ReadLine();
                            Console.Write("Enter a first parameter: ");
                            var firstParameterForEncryptInput = Console.ReadLine();
                            Console.Write("Enter a second parameter: ");
                            var secondParameterForEncryptInput = Console.ReadLine();
                            try
                            {
                                var firstParameter = Convert.ToInt32(firstParameterForEncryptInput);
                                var secondParameter = Convert.ToInt32(secondParameterForEncryptInput);
                                var result = Encrypt(textForEncrypt, firstParameter, secondParameter);
                                Console.WriteLine($"\nDecrypted text: {result}");
                            }
                            catch
                            {
                                Console.WriteLine("\nError.Incorrect input");
                            }

                            break;
                        case Action.Decrypt:
                            Console.Write("Enter a text: ");
                            var textForDecrypt = Console.ReadLine();
                            Console.Write("Enter a first parameter: ");
                            var firstParameterForDecryptInput = Console.ReadLine();
                            Console.Write("Enter a second parameter: ");
                            var secondParameterForDecryptInput = Console.ReadLine();
                            try
                            {
                                var firstParameter = Convert.ToInt32(firstParameterForDecryptInput);
                                var secondParameter = Convert.ToInt32(secondParameterForDecryptInput);
                                var result = Decrypt(textForDecrypt, firstParameter, secondParameter);
                                Console.WriteLine($"\nDecrypted text: {result}");
                            }
                            catch
                            {
                                Console.WriteLine("Error.Incorrect input");
                            }

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

        public static string Encrypt(string text, int firstParameter, int secondParameter)
        {
            var cipherText = string.Empty;
            var chars = text.ToUpper().ToCharArray();

            return chars
                .Select(c => Convert.ToInt32(c - Constants.AffineParameter))
                .Aggregate(cipherText, (current, x)
                    => current + Convert.ToChar((firstParameter * x + secondParameter) % Constants.Mod + Constants.AffineParameter));
        }

        public static string Decrypt(string text, int firstParameter, int secondParameter)
        {
            var result = string.Empty;
            var firstParameterInverse = MultiplicativeInverse(firstParameter);
            var chars = text.ToUpper().ToCharArray();

            foreach (var c in chars)
            {
                var x = Convert.ToInt32(c - Constants.AffineParameter);

                if (x - secondParameter < 0) 
                    x = Convert.ToInt32(x) + Constants.Mod;

                result += Convert.ToChar(firstParameterInverse * (x - secondParameter) % Constants.Mod + Constants.AffineParameter);
            }

            return result;
        }

        public static int MultiplicativeInverse(int number)
        {
            for (var x = 1; x <= Constants.Mod; x++)
                if (number * x % Constants.Mod == 1)
                    return x;

            throw new Exception("No multiplicative inverse found!");
        }
    }
}