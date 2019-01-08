﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReFrontier
{
    class Pack
    {
        public static void ProcessPackInput(string input)
        {
            string logFile = $"{input}\\{input.Remove(0, input.LastIndexOf('\\')+1)}.log";
            if (!File.Exists(logFile)) { Console.WriteLine("ERROR: Log file does not exist."); return; }
            string[] logContent = File.ReadAllLines(logFile);

            if (logContent[0] == "SimpleArchive")
            {
                string fileName = logContent[1];
                int count = int.Parse(logContent[2]);
                Console.WriteLine($"Simple archive with {count} entries.");

                // Entries
                List<string> listFileNames = new List<string>();
                //List<int> listFileOffsets = new List<int>();
                //List<int> listFileSizes = new List<int>();
                //List<int> listFileMagics = new List<int>();

                for (int i = 3; i < logContent.Length; i++)
                {
                    string[] columns = logContent[i].Split(',');
                    listFileNames.Add(columns[0]);
                    //listFileOffsets.Add(int.Parse(columns[1]));
                    //listFileSizes.Add(int.Parse(columns[2]));
                    //listFileMagics.Add(int.Parse(columns[3]));
                }

                using (BinaryWriter bwOutput = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    bwOutput.Write(count);
                    int offset = 0x04 + count * 0x08;
                    for (int i = 0; i < count; i++)
                    {
                        Console.WriteLine($"{input}\\{listFileNames[i]}");
                        byte[] fileData = File.ReadAllBytes($"{input}\\{listFileNames[i]}");
                        bwOutput.BaseStream.Seek(0x04 + i * 0x08, SeekOrigin.Begin);
                        bwOutput.Write(offset);
                        bwOutput.Write(fileData.Length);
                        bwOutput.BaseStream.Seek(offset, SeekOrigin.Begin);
                        bwOutput.Write(fileData);
                        offset += fileData.Length;
                    }
                }
            }
        }
    }
}
