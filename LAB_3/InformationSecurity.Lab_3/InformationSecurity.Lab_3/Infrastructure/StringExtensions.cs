using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InformationSecurity.Lab_3.Infrastructure
{
    public static class StringExtensions
    {
        public static List<int> ToBitArray(this string source, Encoding encoding)
        {
            var utf8Bytes = encoding.GetBytes(source);

            var bitArray = new BitArray(utf8Bytes);

            return (from bool bit in bitArray select bit ? 1 : 0).ToList();
        }

        public static string RoundTo64Blocks(this string source)
        {
            if (source.Length % Constants.SymbolsInBlock == 0)
                return source;

            var countOfSymbolsToAdd = Constants.SymbolsInBlock - source.Length % Constants.SymbolsInBlock;

            return source + new string(Constants.FillSymbol, countOfSymbolsToAdd);
        }

        public static string PrepareKey(this string source)
        {
            if (source.Length == Constants.SymbolsInKey)
                return source;

            if (source.Length > Constants.SymbolsInKey)
                return source.Substring(0, Constants.SymbolsInKey);

            var countOfSymbolsToAdd = Constants.SymbolsInKey - source.Length % Constants.SymbolsInKey;

            return source + new string(Constants.FillSymbol, countOfSymbolsToAdd);
        }

        public static List<string> CutStringIntoBlocks(this string source)
        {
            var countOfBlocks = source.Length / Constants.SymbolsInBlock;

            var result = new List<string>();

            for (var blockIndex = 0; blockIndex < countOfBlocks; blockIndex++)
                result.Add(source.Substring(Constants.SymbolsInBlock * blockIndex, Constants.SymbolsInBlock));

            return result;
        }

        public static List<List<int>> CutByteArrayIntoBlocks(this List<byte> source)
        {
            var result = new List<List<int>>();

            var byteBlocks = new List<List<byte>>();
            var countOfByteBlocks = source.Count / 8;

            for (var i = 0; i < countOfByteBlocks; i++)
            {
                byteBlocks.Add(source.Skip(i*8).Take(8).ToList());
            }

            var bitBlocks = new List<BitArray>();
            bitBlocks.AddRange(byteBlocks.Select(item => new BitArray(item.ToArray())));

            foreach (var bitBlock in bitBlocks)
            {
                var newBlock = new List<int>();

                foreach (bool bit in bitBlock)
                {
                    newBlock.Add(bit ? 1 : 0);
                }

                result.Add(newBlock);
            }

            return result;
        }
    }
}