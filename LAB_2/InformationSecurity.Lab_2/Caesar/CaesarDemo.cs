using System;
using System.Linq;

namespace InformationSecurity.Lab_2.Caesar
{
    public class CaesarDemo
    {
        public void Show()
        {
            var userInput = string.Empty;

            while (userInput != Constants.StopText)
            {
                Console.Clear();
                Console.WriteLine("Caesar Crypt\n");
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
                            Console.Write("Enter a shift: ");
                            var shiftForEncryptInput = Console.ReadLine();
                            try
                            {
                                var shift = Convert.ToInt32(shiftForEncryptInput);
                                var result = Encrypt(textForEncrypt, shift);
                                Console.WriteLine($"\nEncrypted text: {result}");
                            }
                            catch
                            {
                                Console.WriteLine("\nError.Incorrect input");
                            }

                            break;
                        case Action.Decrypt:
                            Console.Write("Enter a text: ");
                            var textForDecrypt = Console.ReadLine();
                            Console.Write("Enter a shift: ");
                            var shiftForDecryptInput = Console.ReadLine();
                            try
                            {
                                var shift = Convert.ToInt32(shiftForDecryptInput);
                                var result = Decrypt(textForDecrypt, shift);
                                Console.WriteLine($"Decrypted text: {result}");
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

        private static char Cipher(char symbol, int shift)
        {
            if (!char.IsLetter(symbol)) return symbol;

            var upperCaseFlag = char.IsUpper(symbol) ? 'A' : 'a';
            return (char) ((symbol + shift - upperCaseFlag) % Constants.Mod + upperCaseFlag);
        }

        private static string Encrypt(string text, int shift)
        {
            return text.Aggregate(string.Empty, (current, symbol) => current + Cipher(symbol, shift));
        }

        private static string Decrypt(string text, int shift)
        {
            return text.Aggregate(string.Empty, (current, symbol) => current + Cipher(symbol, Constants.Mod - shift));
        }
    }
}