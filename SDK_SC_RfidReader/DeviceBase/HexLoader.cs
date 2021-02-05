using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace SDK_SC_RfidReader.DeviceBase
{
    class HexLoader
    {
        public const uint BytesPerInstructionWord = 3;
        public const uint AddressSpacePerInstructionWord = 2;
        public const uint InstructionWordsPerRow = 64;
        public const uint BytesPerRow = (InstructionWordsPerRow * BytesPerInstructionWord);
        public const uint AddressSpacePerRow = (InstructionWordsPerRow * AddressSpacePerInstructionWord);
        public const uint RowsPerPage = 8;
        public const uint InstructionWordsPerPage = (RowsPerPage * InstructionWordsPerRow);
        public const uint BytesPerPage = (InstructionWordsPerPage * BytesPerInstructionWord);
        public const uint AddressSpacePerPage = (RowsPerPage * AddressSpacePerRow);

        // The firmware program flash memory is allocated as follows:
        // o Reset and Interrupts vectors and the DeviceDescription struct in the first five rows of the
        //   first page.
        // o Bootloader code. Currently six pages.
        // o Application nonvolatile configuration storage. Currently two pages.
        // o Application code.
        public const uint BootPageBase = 1; // First page is for reset and interrupts.
        public const uint BootPageCount = 6;
        public const uint ConfigPageBase = 7;
        public const uint ConfigPageCount = 2;
        public const uint ApplicationPageBase = BootPageBase + BootPageCount + ConfigPageCount;

        public const uint FirstPageAllowableRows = 5; // The first five rows of the first page may be specified in the .Hex file.
        public const uint FirstBootLoaderRowIndex = (BootPageBase * RowsPerPage); // BootLoader starts at 0x400.
        public const uint EndBootLoaderRowIndex = ((BootPageBase + BootPageCount) * RowsPerPage);
        public const uint FirstConfigRowIndex = (ConfigPageBase * RowsPerPage);
        public const uint EndConfigRowIndex = ((ConfigPageBase + ConfigPageCount) * RowsPerPage);
        public const uint FirstApplicationRowIndex = ApplicationPageBase * RowsPerPage;
        public const uint LastAllowableApplicationRow = (0x800000 / AddressSpacePerRow);
        public const uint ConfigurationRegistersRow = (0xF80000 / AddressSpacePerRow);

        private enum RecordType
        {
            Data = 0,
            EndOfFile = 1,
            SegmentAddress = 2,
            LinearAddress = 4,
        };

        private readonly string fileName;
        private readonly bool hexLoaded;
        private readonly SortedList pageList = new SortedList();
        private readonly uint applicationRowCount;
        private readonly uint totalRowCount;
        private readonly UInt16 crc;

        private byte[] iteratorRowBytes = new byte[BytesPerRow];
        private UInt32[] iteratorRowWords = new UInt32[InstructionWordsPerRow];
        private uint iteratorPageListIndex = 0;
        private uint iteratorRowIndex = 0;

        static byte[] BlankRow = new byte[]
		{
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
			0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF, 0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
		};

        private HexLoader(string fileName, bool bBootLoaderCode)
        {
            this.fileName = fileName;

            // Assume failure until finished loading.
            hexLoaded = false;
            applicationRowCount = 0;
            crc = 0;

            // Parse the lines of the hex file. If the entire file is parsed successfully, set
            // hexLoaded to true to indicate the file was successfully loaded. Otherwise if
            // an unrecognized line is reached, return early with hexLoaded = false.
            string[] hexLines = System.IO.File.ReadAllLines(fileName);
            if (hexLines != null)
            {
                // Loop to load hex records from the input file. The format is as follows:
                //
                //	:BBAAAATTHHHH....HHHCC
                //
                // where:
                //
                // BB    A two digit hexadecimal byte count representing the number of data
                //       bytes that will appear on the line.
                // AAAA  A four digit hexadecimal address representing the starting address of
                //       the data record.
                // TT    A two digit record type:
                //    00 - Data record
                //    01 - End of File record
                //    02 - Segment address record
                //    04 - Linear address record
                // HH    A two digit hexadecimal data byte, presented in low byte/high byte
                //       combinations.
                // CC    A two digit hexadecimal checksum that is the two's complement of the
                //       sum of all preceding bytes in the record.
                //
                uint baseAddress = 0;
                bool endOfFileReached = false;
                foreach (string line in hexLines)
                {
                    if (endOfFileReached)
                        return; // Failed; extra data found after end-of-file marker.

                    // Parse the line from the hex file.
                    if ((line.Length < 11) || (line[0] != ':') || ((line.Length & 1) == 0))
                        return; // Line does not have minimum length, does not start with ':', or does not have an even number of hex digits.

                    // Extract field values.
                    uint dataByteCount = Convert.ToUInt32(line.Substring(1, 2), 16);
                    uint address = Convert.ToUInt32(line.Substring(3, 4), 16);
                    RecordType recordType = (RecordType)Convert.ToUInt32(line.Substring(7, 2), 16);
                    uint checksumDigit = Convert.ToUInt32(line.Substring(line.Length - 2, 2), 16);
                    byte checksum = (byte)(dataByteCount + (address >> 8) + (address & 0x00FF) + (uint)recordType + checksumDigit);
                    int dataDigitIndex = 9;
                    int dataDigitEnd = line.Length - 2;
                    MyDebug.Assert(2 * dataByteCount == dataDigitEnd - dataDigitIndex);
                    byte[] dataBytes = new byte[dataByteCount];
                    uint dataByteIndex = 0;
                    while (dataDigitIndex < dataDigitEnd)
                    {
                        byte dataByte = Convert.ToByte(line.Substring(dataDigitIndex, 2), 16);
                        dataDigitIndex += 2;
                        dataBytes[dataByteIndex++] = dataByte;
                        checksum += dataByte;
                    }

                    // The final byte is the checksum byte. All of the bytes in the string should add up to zero.
                    if (checksum != 0)
                        return; // Checksum failed.

                    switch (recordType)
                    {
                        case RecordType.Data:
                            AddData(baseAddress + address, dataBytes);
                            break;
                        case RecordType.EndOfFile:
                            endOfFileReached = true;
                            break;
                        case RecordType.SegmentAddress:
                            return; // Unexpected record type.
                        case RecordType.LinearAddress:
                            MyDebug.Assert(address == 0);
                            baseAddress = (((uint)dataBytes[0] << 8) | dataBytes[1]) << 16;
                            break;
                        default:
                            return; // Unexpected record type.
                    }
                }
            }

            // Hex file has loaded successfully. Calculate the row count and CRC.
            if (CalculateRowCountAndCRC(bBootLoaderCode, out applicationRowCount, out totalRowCount, out crc))
            {
                if (applicationRowCount == 0)
                    throw new Exception("File contains no application data.");
                hexLoaded = true; // Success!
            }
        }

        public uint ApplicationRowCount { get { return applicationRowCount; } }
        public uint TotalRowCount { get { return totalRowCount; } }
        public uint CRC { get { return crc; } }

        private bool CalculateRowCountAndCRC(bool bBootLoaderCode,
            out uint applicationRowCount, out uint totalRowCount, out ushort crc)
        {
            applicationRowCount = 0;
            totalRowCount = 0;
            crc = 0;

            uint startAllowableRowIndex;
            uint endAllowableRowIndex;
            if (bBootLoaderCode)
            {
                startAllowableRowIndex = FirstBootLoaderRowIndex;
                endAllowableRowIndex = EndBootLoaderRowIndex;
            }
            else
            {
                startAllowableRowIndex = FirstApplicationRowIndex;
                endAllowableRowIndex = LastAllowableApplicationRow;
            }

            MyDebug.Assert(BlankRow.Length == BytesPerRow);
            uint currentRowIndex = 0;
            uint rowIndex;
            byte[] row;
            bool bFirst = true;
            CRC crcCalculator = new CRC();
            while (EnumerateRows(bFirst, out row, out rowIndex))
            {
                bFirst = false;
                totalRowCount++;
                MyDebug.Assert(row.Length == BytesPerRow);
                MyDebug.Assert(rowIndex >= currentRowIndex); // Failure indicates a bug in EnumerateRows().

                // The CRC calculation does not include flash memory above the user program flash
                // memory area, as defined in chapter 3 of the dsPIC33FJXXXMCX06/X08/X10 Motor
                // Control Family datasheet. Skip any such data.
                if (rowIndex >= LastAllowableApplicationRow)
                {
                    MyDebug.Assert((rowIndex == 0x1F000) || IsBlank(row)); // Only the single processor configuration row is expected.
                    continue;
                }

                // Take care of any rows that are skipped in the hex file.
                while (currentRowIndex < rowIndex)
                {
                    // Ignore blank rows in the protected area. They are not included in the CRC calculation.
                    if ((currentRowIndex < FirstPageAllowableRows) || (currentRowIndex >= startAllowableRowIndex))
                    {
                        // Assume the skipped row is all 0xFF's.
                        crcCalculator.addBuffer(BlankRow);
                    }
                    currentRowIndex++;
                }

                if ((rowIndex < FirstPageAllowableRows) ||
                    ((rowIndex >= startAllowableRowIndex) && (rowIndex < endAllowableRowIndex)))
                {
                    // Update the CRC with this row.
                    crcCalculator.addBuffer(row);
                }
                else
                {
                    // Code was specified for the protected areas. If the specified code is
                    // something other than all FF's, fail the hex loading.
                    foreach (byte rowByte in row)
                    {
                        if (rowByte != 0xFF)
                            throw new Exception("File contains flash data in a protected page.");
                    }
                }

                currentRowIndex++;
            }

            if (currentRowIndex > startAllowableRowIndex)
                applicationRowCount = currentRowIndex - startAllowableRowIndex;
            crc = crcCalculator.crc;

            return true;
        }

        private bool IsBlank(byte[] row)
        {
            MyDebug.Assert(row.Length == BytesPerRow);
            foreach (byte rowByte in row)
            {
                if (rowByte != 0xFF)
                    return false;
            }

            return true;
        }

        private void AddData(uint address, byte[] dataBytes)
        {
            // A page contains 512 instruction words (1536 bytes) but has an address space range
            // of 1024 from the processor's point of view, but 2048 from the .hex file's point of
            // view. Although each instruction is 24 bits, they are specified in 32 bits, so they
            // will be stored in pageList as 32 bits.
            MyDebug.Assert((address & 3) == 0); // Address must be multiple of 4 for 16-bit processor.
            MyDebug.Assert((dataBytes.Length & 3) == 0); // Data is expected in 4-byte words for 16-bit processor.
            uint dataWords = (uint)dataBytes.Length / 4;
            uint wordIndex = address / 4;
            uint pageIndex = wordIndex / InstructionWordsPerPage;
            uint dstWordIndex = wordIndex % InstructionWordsPerPage;
            uint srcByteIndex = 0;
            uint srcByteEndIndex = (uint)dataBytes.Length;
            uint srcByteEndIndexNextPass = 0;

            // If the byte series overlaps pages, restrict the number of bytes that will be copied
            // on the first pass.
            if (dstWordIndex + dataWords > InstructionWordsPerPage)
            {
                srcByteEndIndexNextPass = srcByteEndIndex;
                srcByteEndIndex -= 4 * (dstWordIndex + dataWords - InstructionWordsPerPage);
            }

            // The following loops one or two times depending on if the data overlaps a page boundary.
            while (true)
            {
                // Find the page for the specified address. If it hasn't yet been created, allocate
                // it now and initialize its contents to all 0xFFs.
                UInt32[] page = (UInt32[])pageList[pageIndex];
                while (page == null)
                {
                    page = new UInt32[InstructionWordsPerPage];
                    for (int i = 0; i < page.Length; i++)
                        page[i] = 0x00FFFFFF; // Only 24 bits are actually stored.
                    pageList.Add(pageIndex, page);
                }

                // Add the new data from srcByteIndex to srcByteEndIndex to its destination page.
                while (srcByteIndex < srcByteEndIndex)
                {
                    UInt32 word = 0;
                    for (int i = 0; i < 32; i += 8)
                        word |= (UInt32)dataBytes[srcByteIndex++] << i;
                    MyDebug.Assert(word < 0x01000000);

                    // If any word is specified multiple times in the .hex file, make sure they
                    // always specify the same value.
                    MyDebug.Assert((page[dstWordIndex] == 0x00FFFFFF) || (page[dstWordIndex] == word));
                    page[dstWordIndex++] = word;
                }

                // Break out of the loop if all of the data has been saved.
                if (srcByteEndIndexNextPass == 0)
                    break;

                // The data byte series overlaps two pages. Set up for the next page and loop to
                // copy to the second page.
                pageIndex++;
                dstWordIndex = 0;
                srcByteEndIndex = srcByteEndIndexNextPass;
                srcByteEndIndexNextPass = 0;
            }
        }

        public bool EnumerateRows(bool bStartOver, out UInt32[] rowWords, out uint rowIndex)
        {
            if (bStartOver)
            {
                iteratorPageListIndex = 0;
                iteratorRowIndex = 0;
            }

            // If already at end of list, return false.
            if (iteratorPageListIndex >= pageList.Count)
            {
                rowWords = null;
                rowIndex = 0;
                return false; // End of list.
            }

            // Copy the next row into iteratorRowBytes.
            uint pageIndex = (uint)pageList.GetKey((int)iteratorPageListIndex);
            rowIndex = RowsPerPage * pageIndex + iteratorRowIndex;
            UInt32[] pageWords = (UInt32[])pageList[pageIndex];
            Array.Copy(pageWords, iteratorRowIndex * InstructionWordsPerRow, iteratorRowWords, 0, InstructionWordsPerRow);
            rowWords = iteratorRowWords;

            // Increment to next row.
            iteratorRowIndex++;
            if (iteratorRowIndex >= RowsPerPage)
            {
                iteratorPageListIndex++;
                iteratorRowIndex = 0;
            }

            return true;
        }

        public bool EnumerateRows(bool bStartOver, out byte[] row, out uint rowIndex)
        {
            UInt32[] rowWords;
            row = null; // Value for failures.
            if (!EnumerateRows(bStartOver, out rowWords, out rowIndex))
                return false; // End of list.

            // Copy the next row into iteratorRowBytes.
            uint dstByte = 0;
            for (uint wordIndex = 0; wordIndex < InstructionWordsPerRow; wordIndex++)
            {
                uint word = rowWords[wordIndex];
                for (int shift = 0; shift < 24; shift += 8)
                    iteratorRowBytes[dstByte++] = (byte)(word >> shift);
            }
            row = iteratorRowBytes;

            return true;
        }

        private static string HexLine(uint address, RecordType recordType, string dataFormat, params object[] dataArgs)
        {
            string data = string.Format(dataFormat, dataArgs);
            MyDebug.Assert((data.Length & 1) == 0); // Must have an even number of digits.
            MyDebug.Assert(data.Length <= 32);
            uint dataBytes = (uint)data.Length / 2;
            uint digitSum = dataBytes + ((address >> 8) & 0xFF) + (address & 0xFF) + (uint)recordType;
            for (int dataByteIndex = 0; dataByteIndex < dataBytes; dataByteIndex++)
            {
                uint dataByte = Convert.ToUInt32(data.Substring(2 * dataByteIndex, 2), 16);
                digitSum += dataByte;
            }
            uint checksum = (0x100 - digitSum) & 0xFF;
            return string.Format(":{0:X2}{1:X4}{2:X2}{3}{4:X2}", dataBytes, address, (uint)recordType, data, checksum);
        }

        public void WriteHexFile(string outHexFileName)
        {
            // Add the Application Check Block. The firmware BootLoader expects the
            // FirmwareApplicationCheckType block at the beginning of row 7 in the following
            // format:
            //
            //     uint16 checkTag; // This should be CheckTag (0xC6C6)
            //     uint16 firstRowIndex; // This should be FirstApplicationRowIndex.
            //     uint16 rowCount; // The number of application rows starting at firstRowIndex.
            //     uint16 applicationCRC; // The CRC of the first FirstPageAllowableRows and rowCount application rows.
            //
            // This struct is packed into the 3-byte instruction words, using all three bytes of
            // each instruction word. For example, the both bytes of the checkTag and the low-order
            // byte of firstRowIndex are packed into the first instruction word. The high-order
            // byte of firstRowIndex and the two bytes of rowCount are packed in the second
            // instruction word, etc.
            const uint CheckTag = 0xC6C6;
            const uint CheckBlockRowIndex = 7;
            const uint HexAddress = CheckBlockRowIndex * InstructionWordsPerRow * 4;
            uint[] checkBlock = new uint[] { CheckTag, FirstApplicationRowIndex, applicationRowCount, crc };
            byte[] applicationCheckBlock = new byte[4 * checkBlock.Length];
            uint dstByte = 0;
            for (uint fieldIndex = 0; fieldIndex < checkBlock.Length; fieldIndex++)
            {
                for (int shift = 0; shift < 16; shift += 8)
                {
                    applicationCheckBlock[dstByte++] = (byte)(checkBlock[fieldIndex] >> shift);
                    if (dstByte % 4 == 3)
                        applicationCheckBlock[dstByte++] = 0;
                }
            }
            while (dstByte < applicationCheckBlock.Length)
                applicationCheckBlock[dstByte++] = 0;
            AddData(HexAddress, applicationCheckBlock);

            // Create a text file writer to write the hex file.
            System.IO.StreamWriter writer = System.IO.File.CreateText(outHexFileName);
            uint lastLinearAddress = 0xFFFFFFFF; // Force output of linear address before first data record.
            bool bFirst = true;
            uint rowIndex;
            UInt32[] rowWords;
            while (EnumerateRows(bFirst, out rowWords, out rowIndex))
            {
                if (rowIndex > ConfigurationRegistersRow)
                    continue;

                bFirst = false;

                uint address = rowIndex * InstructionWordsPerRow * 4; // Addresses in hex file increment four per 3-byte instruction word.
                uint linearAddress = address >> 16; // Upper sixteen bits.
                uint linearOffset = address & 0xFFFF; // Lower sixteen bits.

                // If moving into a new linear address range, output the linear address now.
                if (linearAddress != lastLinearAddress)
                {
                    lastLinearAddress = linearAddress;
                    writer.WriteLine(HexLine(0, RecordType.LinearAddress, "{0:X4}", linearAddress));
                }

                // Write the row data, four instruction words at a time.
                uint wordOffset = 0;
                uint wordsToOutput = InstructionWordsPerRow;
                uint wordsPerLine = 4;

                if (rowIndex == ConfigurationRegistersRow)
                {
                    // The configuration registers must be output one register at a time, and only
                    // the first seven registers should be output. The eighth register is
                    // undocumented and results in strange behavior when programmed.
                    wordsToOutput = 7;
                    wordsPerLine = 1;
                }

                while (wordOffset < wordsToOutput)
                {
                    StringBuilder builder = new StringBuilder(32);
                    for (uint wordIndex = 0; wordIndex < wordsPerLine; wordIndex++)
                    {
                        UInt32 word = rowWords[wordOffset++];
                        for (int shift = 0; shift < 32; shift += 8)
                            builder.AppendFormat("{0:X2}", (word >> shift) & 0xFF);
                    }
                    writer.WriteLine(HexLine(linearOffset, RecordType.Data, builder.ToString()));
                    linearOffset += 4 * wordsPerLine;
                }
            }
            writer.WriteLine(HexLine(0, RecordType.EndOfFile, ""));
            writer.Close();
        }

        public void MergeHex(HexLoader other)
        {
            bool bStartOver = true;
            UInt32[] rowWords;
            uint rowIndex;
            while (other.EnumerateRows(bStartOver, out rowWords, out rowIndex))
            {
                bStartOver = false;
                uint address = rowIndex * InstructionWordsPerRow * 4; // Address in hex file increment four per 3-byte instruction word.

                // Trim unused instruction words.
                uint startWordIndex = 0;
                uint endWordIndex = InstructionWordsPerRow;
                while ((endWordIndex > 0) && (rowWords[endWordIndex - 1] == 0x00FFFFFF))
                    endWordIndex--;
                if (endWordIndex == 0)
                    continue; // Empty row.
                while (rowWords[startWordIndex] == 0x00FFFFFF)
                    startWordIndex++;
                address += 4 * startWordIndex;

                // Copy the next row into row.
                byte[] row = new byte[4 * (endWordIndex - startWordIndex)];
                uint dstByte = 0;
                for (uint wordIndex = startWordIndex; wordIndex < endWordIndex; wordIndex++)
                {
                    uint word = rowWords[wordIndex];
                    for (int shift = 0; shift < 32; shift += 8)
                        row[dstByte++] = (byte)(word >> shift);
                }

                AddData(address, row);
            }
        }

        // Static method to create a HexLoader. Throws an Exception on failure.
        public static HexLoader LoadHexFile(string hexFileName, bool bBootLoader)
        {
            if (hexFileName != null)
            {
                try
                {
                    HexLoader loader = new HexLoader(hexFileName, bBootLoader);
                    if (loader.hexLoaded)
                        return loader;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error: Could not open file: '" + hexFileName +
                        "'. Reason: " + ex.Message);
                }
            }

            return null;
        }
    }
}
