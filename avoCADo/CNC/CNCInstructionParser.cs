using OpenTK;
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

        public float PathsLength { get; private set; } 

        public CNCInstructionSet(CNCTool tool)
        {
            Tool = tool;
        }

        public void UpdatePathsLength()
        {
            PathsLength = 0.0f;
            for(int i = 0; i < Instructions.Count - 1; i++)
            {
                PathsLength += (Instructions[i].Position - Instructions[i + 1].Position).Length;
            }
        }
    }
    public class CNCInstruction
    {
        public int N;
        public int G;
        public float X = float.NaN;
        public float Y = float.NaN;
        public float Z = float.NaN;

        public Vector3 Position => new Vector3(X, Y, Z);
    }
    public static class CNCInstructionParser
    {
        private static CultureInfo _ci;
        private static float _unitsMult = 0.001f;

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

                RemoveNANs(instSet);
                instSet.UpdatePathsLength();
                return instSet;
            }
            catch(Exception e)
            {
                MessageBox.Show($"Path file is corrupted!\n{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private static void RemoveNANs(CNCInstructionSet instructionSet)
        {
            var currentPosition = Vector3.UnitY;
            foreach (var instruction in instructionSet.Instructions)
            {
                if(float.IsNaN(instruction.X)) instruction.X = currentPosition.X;
                if(float.IsNaN(instruction.Y)) instruction.Y = currentPosition.Y;
                if(float.IsNaN(instruction.Z)) instruction.Z = currentPosition.Z;
                currentPosition = instruction.Position;
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
            radius = float.Parse(extension.Substring(1)) * _unitsMult * 0.5f;
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
                    instruction.X = float.Parse(value, _ci) * _unitsMult;
                    break;
                case 'Y':
                    instruction.Z = -float.Parse(value, _ci) * _unitsMult; //Z and Y swapped because format intends Z axis as "up", and avoCADo has Y as "up"
                    break;
                case 'Z':
                    instruction.Y = float.Parse(value, _ci) * _unitsMult;
                    break;
            }
        }

    }
}
