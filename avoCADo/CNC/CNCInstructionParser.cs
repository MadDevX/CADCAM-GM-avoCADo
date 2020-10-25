using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace avoCADo.CNC
{
    public class CNCInstructionSet
    {
        public CNCTool Tool;
        public List<CNCInstruction> Instructions = new List<CNCInstruction>();

        public CNCInstructionSet(CNCTool tool)
        {
            Tool = tool;
        }
    }
    public class CNCInstruction
    {
        public int N;
        public int G;
        public float X = float.NaN;
        public float Y = float.NaN;
        public float Z = float.NaN;
    }
    public static class CNCInstructionParser
    {
        private static CultureInfo _ci;

        static CNCInstructionParser()
        {
            _ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            _ci.NumberFormat.NumberDecimalSeparator = ".";
        }

        public static CNCInstructionSet ParsePathFile(string filepath)
        {
            try
            {
                var instSet = CreateInstructionSet(filepath);
                using(var sr = new StreamReader(filepath))
                {
                    while(sr.EndOfStream == false)
                    {
                        var inst = Parse(sr.ReadLine());
                        if (inst != null)
                        {
                            instSet.Instructions.Add(inst);
                        }
                    }
                }
                return instSet;
            }
            catch(Exception e)
            {
                MessageBox.Show($"Path file is corrupted!\n{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static CNCInstructionSet CreateInstructionSet(string filepath)
        {
            var extension = filepath.Split('.').Last();
            ToolType type;
            float radius;
            switch (extension[0])
            {
                case 'k':
                    type = ToolType.Round;
                    break;
                case 'f':
                    type = ToolType.Flat;
                    break;
                default:
                    throw new InvalidDataException("Unrecognized file extension format!");
            }
            radius = float.Parse(extension.Substring(1));
            return new CNCInstructionSet(new CNCTool(type, radius));
        }

        private static CNCInstruction Parse(string command)
        {
            var headers = Regex.Split(command, @"-?[0-9]+(?:\.[0-9]+)?").Where((s) => s.Length > 0).ToArray();
            var values = Regex.Split(command, @"[A-Z]").Where((s) => s.Length > 0).ToArray();

            if(headers.Length != values.Length)
            {
                throw new InvalidDataException($"Corrupt instruction: {command}");
            }
            var instruction = new CNCInstruction();
            for(int i = 0; i < headers.Length; i++)
            {
                SetValue(instruction, headers[i], values[i]);
            }
            return instruction;
        }

        private static void SetValue(CNCInstruction instruction, string header, string value)
        {
            switch(header[0])
            {
                case 'N':
                    instruction.N = int.Parse(value);
                    break;
                case 'G':
                    instruction.G = int.Parse(value);
                    break;
                case 'X':
                    instruction.X = float.Parse(value, _ci);
                    break;
                case 'Y':
                    instruction.Y = float.Parse(value, _ci);
                    break;
                case 'Z':
                    instruction.Z = float.Parse(value, _ci);
                    break;
            }
        }

    }
}
