using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PlcMonitor.UI.Models.S7
{
    public static class AddressParser
    {
        // Missing support for DT, DTZ, DTL, DTLZ, which are also currently not supported by Sally7
        private static readonly Regex AddressRegex = new Regex(@"^DB(\d+),((B|BYTE|I|INT|WORD|DI|DINT|DW|DWORD|R|REAL|C|CHAR|S)(\d+)|X(\d+)\.([0-8]))$");

        public static bool TryParse(string address, out int dataBlock, out TypeCode typeCode, out int startByte, out int? bitIndex)
        {
            const int idxDataBlock = 1, idxType = 3, idxStartByte = 4, idxBitStartByte = 5, idxBitIndex = 6;

            var match = AddressRegex.Match(address);
            if (!match.Success)
            {
                dataBlock = default;
                typeCode = default;
                startByte = default;
                bitIndex = default;

                return false;
            }

            int ToInt(string input) => int.Parse(input, NumberStyles.None, CultureInfo.InvariantCulture);
            int MatchValue(int groupIndex) => ToInt(match!.Groups[groupIndex].Value);

            dataBlock = MatchValue(idxDataBlock);

            if (match.Groups[idxBitStartByte].Success)
            {
                typeCode = TypeCode.Boolean;
                startByte = MatchValue(idxBitStartByte);
                bitIndex = MatchValue(idxBitIndex);
            }
            else
            {
                bitIndex = null;
                startByte = MatchValue(idxStartByte);

                typeCode = match.Groups[idxType].Value switch
                {
                    "B" => TypeCode.Byte,
                    "BYTE" => TypeCode.Byte,
                    "I" => TypeCode.Int16,
                    "INT" => TypeCode.Int16,
                    "WORD" => TypeCode.UInt16,
                    "DI" => TypeCode.Int32,
                    "DINT" => TypeCode.Int32,
                    "DW" => TypeCode.UInt32,
                    "DWORD" => TypeCode.UInt32,
                    "R" => TypeCode.Single,
                    "REAL" => TypeCode.Single,
                    "C" => TypeCode.Byte, // S7 CHAR is 1 byte only
                    "CHAR" => TypeCode.Byte,
                    "S" => TypeCode.String,
                    _ => TypeCode.Empty // RegEx match already excludes all other options
                };
            }

            return true;
        }
    }
}