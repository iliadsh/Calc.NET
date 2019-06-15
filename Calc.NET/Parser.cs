using System;
using System.Collections.Generic;

namespace Calc.NET
{
    public class Parser
    {
        private string inputString;
        private List<string> commandStack;
        private List<CalcOperator> operators;
        private List<CalcFunction> functions;

        public Parser() : this(string.Empty) { }

        public Parser(string input)
        {
            inputString = input;
            commandStack = new List<string>();
        }

        public void SetOperators(List<CalcOperator> operators)
        {
            this.operators = operators;
        }

        public void SetFunctions(List<CalcFunction> functions)
        {
            this.functions = functions;
        }

        public void SetInputString(string inputString)
        {
            this.inputString = inputString;
        }

        public string GetInputString()
        {
            return this.inputString;
        }

        public List<string> GetCommandStack()
        {
            return this.commandStack;
        }

        public void Clear()
        {
            this.inputString = string.Empty;
            this.commandStack = new List<string>();
        }

        private void AddAndEmptyBuffer(ref string buffer)
        {
            if (string.IsNullOrEmpty(buffer))
            {
                return;
            }
            commandStack.Add(buffer);
            buffer = string.Empty;
        }

        public void Parse()
        {
            if (string.IsNullOrEmpty(inputString))
            {
                throw new Exception("No input to parse!");
            }
            if (operators == null || functions == null)
            {
                throw new Exception("No operators or functions specified!");
            }
            char[] characters = inputString.ToCharArray();
            string buffer = string.Empty;
            foreach (char c in characters)
            {
                if (c == ' ')
                {
                    AddAndEmptyBuffer(ref buffer);
                }
                else if (c == '(')
                {
                    AddAndEmptyBuffer(ref buffer);
                    commandStack.Add("RIS");
                }
                else if (c == ')')
                {
                    AddAndEmptyBuffer(ref buffer);
                    commandStack.Add("LOW");
                }
                else if (c == ',')
                {
                    AddAndEmptyBuffer(ref buffer);
                    commandStack.Add("COM");
                }
                else
                {
                    bool found = false;
                    foreach (CalcOperator calcOperator in operators)
                    {
                        if (c == calcOperator.symbol)
                        {
                            AddAndEmptyBuffer(ref buffer);
                            commandStack.Add(calcOperator.code);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        buffer += c;
                    }
                }
                foreach (CalcFunction calcFunction in functions)
                {
                    if (buffer.Equals(calcFunction.name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        commandStack.Add(calcFunction.code);
                        buffer = string.Empty;
                    }
                }
            }
            AddAndEmptyBuffer(ref buffer);
        }
    }
}
