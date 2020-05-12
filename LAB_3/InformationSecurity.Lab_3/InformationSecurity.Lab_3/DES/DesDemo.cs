using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InformationSecurity.Lab_3.Infrastructure;
using Action = InformationSecurity.Lab_3.Enums.Action;

namespace InformationSecurity.Lab_3.DES
{
    public class DESDemo
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
                        {
                            Console.Write("Enter a text: ");
                            var text = Console.ReadLine();
                            Console.Write("Enter a key: ");
                            var key = Console.ReadLine();

                            Encrypt(text, key);
                            break;
                        }
                        case Action.Decrypt:
                        {
                            Console.Write("Enter a text: ");
                            var text = Console.ReadLine();
                            Console.Write("Enter a key: ");
                            var key = Console.ReadLine();

                            Decrypt(text, key);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                }
                catch(Exception exception)
                {
                    Console.WriteLine("\nError. Incorrect input");
                    Console.WriteLine(exception.Message);
                    return;
                }
            }
        }

        private void Encrypt(string text, string key)
        {
            text = text.RoundTo64Blocks();
            key = key.PrepareKey();

            var blocks = text.CutStringIntoBlocks();

            var cryptResult = DESEncrypt(blocks, key);
            var merged = new List<int>();
            foreach (var partOfResult in cryptResult)
            {
                merged.AddRange(partOfResult);
            }

            var resultBitArray = new BitArray(merged.Count);
            for (var i = 0; i < resultBitArray.Length; i++)
            {
                resultBitArray[i] = merged[i] == 1;
            }

            var resultByteArray = BitArrayToByteArray(resultBitArray);
            var resultString = Encoding.UTF8.GetString(resultByteArray);

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"Encrypted text is {resultString}");
            File.WriteAllText("C:/temp/result.txt", $"Result bit set is {string.Join("", merged)}\nEncrypted text is {resultString}");
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        private List<List<int>> DESDecrypt(List<string> blocks, string key)
        {
            var result = new List<List<int>>();

            var keyBitArray = key.ToBitArray(Encoding.UTF8); // ключ в виде массива битов
            keyBitArray = KeyPermutation(keyBitArray); // перестановка битов в ключе
            var C0 = keyBitArray.Take(28).ToList();
            var D0 = keyBitArray.Skip(28).ToList();

            foreach (var block in blocks)
            {
                var blockBitArray = block.ToBitArray(Encoding.UTF8); // взять биты для блока из 64 бит
                blockBitArray = StartPermutation(blockBitArray); // перестановка IP

                var LI = new List<List<int>> { blockBitArray.Take(32).ToList() };
                var RI = new List<List<int>> { blockBitArray.Skip(32).Take(32).ToList() };

                for (var i = 1; i <= 16; i++)
                {
                    var keyAtIteration = GetKeyAtDecryptIteration(i, C0, D0); // взять k[i]

                    RI.Add(LI[i - 1]); // L[i] = R[i - 1]

                    var FI = F(LI[i - 1], keyAtIteration); // результат работы F

                    var newLI = new List<int>();

                    for (var j = 0; j < 32; j++)
                    {
                        newLI.Add(SumByMod2(RI[i - 1][j], FI[j]));
                    }

                    LI.Add(newLI); // L[i] = R[i-1] XOR F(L[i - 1], k[i])
                }

                var merged = new List<int>();
                merged.AddRange(LI.Last());
                merged.AddRange(RI.Last()); // объединение двух результирующих частей

                var permutedResult = new List<int>();
                for (var i = 0; i < 64; i++)
                {
                    permutedResult.Add(merged[Matrices.EndPermutation[i] - 1]);
                }

                result.Add(permutedResult);

            }

            return result;
        }

        private List<List<int>> DESEncrypt(List<string> blocks, string key)
        {
            var result = new List<List<int>>();

            var keyBitArray = key.ToBitArray(Encoding.UTF8); // ключ в виде массива битов
            keyBitArray = KeyPermutation(keyBitArray); // перестановка битов в ключе
            var C0 = keyBitArray.Take(28).ToList(); 
            var D0 = keyBitArray.Skip(28).ToList();

            foreach (var block in blocks)
            {
                var blockBitArray = block.ToBitArray(Encoding.UTF8); // взять биты для блока из 64 бит
                blockBitArray = StartPermutation(blockBitArray); // перестановка IP

                var LI = new List<List<int>>{ blockBitArray.Take(32).ToList() };
                var RI = new List<List<int>> { blockBitArray.Skip(32).Take(32).ToList() };

                for (var i = 1; i <= 16; i++)
                {
                    var keyAtIteration = GetKeyAtEncryptIteration(i, C0, D0); // взять k[i]

                    LI.Add(RI[i - 1]); // L[i] = R[i - 1]

                    var FI = F(RI[i - 1], keyAtIteration); // результат работы F

                    var newRI = new List<int>();

                    for (var j = 0; j < 32; j++)
                    {
                        newRI.Add(SumByMod2(LI[i-1][j] ,FI[j])); 
                    }

                    RI.Add(newRI); // R[i] = L[i-1] XOR F(R[i - 1], k[i])
                }

                var merged = new List<int> ();
                merged.AddRange(LI.Last());
                merged.AddRange(RI.Last()); // объединение двух результирующих частей

                var permutedResult = new List<int>();
                for (var i = 0; i < 64; i++)
                {
                    permutedResult.Add(merged[Matrices.EndPermutation[i] - 1]);
                }

                result.Add(permutedResult);
            }

            return result;
        }

        private List<int> F(List<int> RI, List<int> key)
        {
            var extendedRI = new List<int>();
            for (var i = 0; i < 48; i++)
            {
                extendedRI.Add(RI[Matrices.E[i] - 1]);
            }

            var riModWithKey = new List<int>();
            for (var i = 0; i < 48; i++)
            {
                riModWithKey.Add(SumByMod2(extendedRI[i], key[i]));
            }

            var BI = new List<List<int>>();
            for (var i = 0; i < 8; i++)
            {
                BI.Add(riModWithKey.Skip(i * 6).Take(6).ToList());
            }

            var BIAfterS = new List<List<int>>();

            foreach (var b in BI)
            {
                var firstNumber = GetFromBinary(new List<int>{b.First(), b.Last()});
                var secondNumber = GetFromBinary(b.Skip(1).Take(4).ToList());

                var numberFromS = Matrices.SI[BI.IndexOf(b), firstNumber, secondNumber];
                var numberAsBinary = NumberToBinaryArray(numberFromS);
                while (numberAsBinary.Count < 4)
                {
                    numberAsBinary.Insert(0, 0);
                }

                BIAfterS.Add(numberAsBinary);

            }

            var mergedBIAfterS = new List<int>();
            foreach (var BIforMerge in BIAfterS)
            {
                mergedBIAfterS.AddRange(BIforMerge);
            }

            var permutedBiAfterS = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                permutedBiAfterS.Add(mergedBIAfterS[Matrices.P[i] - 1]);
            }

            return permutedBiAfterS;
        }

        public int GetFromBinary(List<int> bitArray)
        {
            var maxPower = bitArray.Count - 1;
            var result = 0;

            foreach (var bit in bitArray)
            {
                if (bit == 1)
                {
                    result += (int)Math.Pow(2, maxPower);
                }

                maxPower--;
            }

            return result;
        }

        public List<int> NumberToBinaryArray(int value)
        {
            var binaryString = Convert.ToString(value, 2);

            var result = new List<int>();
            foreach (var bit in binaryString)
            {
                result.Add(bit - '0');
            }

            return result;
        }

        public int SumByMod2(int left, int right)
        {
            var sum = left + right;

            return sum == 2 ? 0 : sum;
        }

        private List<int> GetKeyAtEncryptIteration(int iteration, List<int> C0, List<int> D0)
        {
            var shift = 0;
            for (var i = 0; i < iteration; i++)
            {
                shift += Matrices.KeyIndexPermutation[i];
            }

            var ci = C0.Skip(shift).ToList();
            var di = D0.Skip(shift).ToList();
            ci.AddRange(C0.Take(shift));
            di.AddRange(D0.Take(shift));

            var merged = ci.Concat(di).ToList();

            var result = new List<int>();

            for (var i = 0; i < 48; i++) 
                result.Add(merged[Matrices.CIDIPermutation[i] - 1]);

            return result;
        }

        private List<int> GetKeyAtDecryptIteration(int iteration, List<int> C0, List<int> D0)
        {
            var shift = 0;
            for (var i = 0; i < iteration; i++)
            {
                shift += Matrices.KeyIndexPermutation[i];
            }

            var ci = C0.Skip(C0.Count - shift).ToList();
            var di = D0.Skip(C0.Count - shift).ToList();
            ci.AddRange(C0.Take(C0.Count - shift));
            di.AddRange(D0.Take(C0.Count - shift));

            var merged = ci.Concat(di).ToList();

            var result = new List<int>();

            for (var i = 0; i < 48; i++)
                result.Add(merged[Matrices.CIDIPermutation[i] - 1]);

            return result;
        }

        private List<int> StartPermutation(List<int> block)
        {
            var result = new List<int>();

            for (var i = 0; i < 64; i++)
            {
                result.Add(block[Matrices.StartPermutation[i] - 1]);
            }

            return result;
        }

        private List<int> KeyPermutation(List<int> key)
        {
            var result = new List<int>();

            for (var i = 0; i <= 7; i++)
            {
                var newRange = key.Skip(i * 7).Take(7).ToList();
                newRange.Add(newRange.Sum() % 2 == 0 ? 1 : 0);

                result.AddRange(newRange);
            }

            var permutationResult = new List<int>();

            for (var i = 0; i < 56; i++) 
                permutationResult.Add(result[Matrices.C0D0[i] - 1]);

            return permutationResult;
        }

        private void Decrypt(string text, string key)
        {
            text = text.RoundTo64Blocks();
            key = key.PrepareKey();

            var blocks = text.CutStringIntoBlocks();

            var cryptResult = DESDecrypt(blocks, key);
            var merged = new List<int>();
            foreach (var partOfResult in cryptResult)
            {
                merged.AddRange(partOfResult);
            }

            var resultBitArray = new BitArray(merged.Count);
            for (var i = 0; i < resultBitArray.Length; i++)
            {
                resultBitArray[i] = merged[i] == 1;
            }

            var resultByteArray = BitArrayToByteArray(resultBitArray);
            var resultString = Encoding.UTF8.GetString(resultByteArray);

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"Decrypted text is {resultString}");
            File.WriteAllText("C:/temp/result.txt", $"Result bit set is {string.Join("", merged)}\nDecrypted text is {resultString}");
        }
    }
}