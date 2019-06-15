using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.NET
{
    public class SimpleCalculator
    {
        private Parser parser;
        private SyntaxTree runner;
        private readonly List<CalcOperator> operators = new List<CalcOperator>
            {
                new CalcOperator()
                {
                    symbol = '+',
                    code = "ADD",
                    Operate = (curr, newval) => curr + newval
                },
                new CalcOperator()
                {
                    symbol = '-',
                    code = "SUB",
                    Operate = (curr, newval) => curr - newval
                },
                new CalcOperator()
                {
                    symbol = '*',
                    code = "MUL",
                    Operate = (curr, newval) => curr * newval,
                    precedence = true
                },
                new CalcOperator()
                {
                    symbol = '/',
                    code = "DIV",
                    Operate = (curr, newval) => curr / newval,
                    precedence = true
                },
                new CalcOperator()
                {
                    symbol = '^',
                    code = "POW",
                    Operate = (curr, newval) => (float)Math.Pow(curr, newval)
                },
                new CalcOperator()
                {
                    symbol = '%',
                    code = "MOD",
                    Operate = (curr, newval) => curr % newval
                },
            };
        private readonly List<CalcFunction> functions = new List<CalcFunction>
            {
                new CalcFunction()
                {
                    name = "sin",
                    code = "SIN",
                    Operate = (curr) => (float)Math.Sin(curr)
                },
                new CalcFunction()
                {
                    name = "cos",
                    code = "COS",
                    Operate = (curr) => (float)Math.Cos(curr)
                },
                new CalcFunction()
                {
                    name = "tan",
                    code = "TAN",
                    Operate = (curr) => (float)Math.Tan(curr)
                },
                new CalcFunction()
                {
                    name = "asin",
                    code = "ASN",
                    Operate = (curr) => (float)Math.Asin(curr)
                },
                new CalcFunction()
                {
                    name = "acos",
                    code = "ACS",
                    Operate = (curr) => (float)Math.Acos(curr)
                },
                new CalcFunction()
                {
                    name = "atan",
                    code = "ATN",
                    Operate = (curr) => (float)Math.Atan(curr)
                },
                new CalcFunction()
                {
                    name = "csc",
                    code = "CSC",
                    Operate = (curr) => 1 / (float)Math.Sin(curr)
                },
                new CalcFunction()
                {
                    name = "sec",
                    code = "SEC",
                    Operate = (curr) => 1 / (float)Math.Cos(curr)
                },
                new CalcFunction()
                {
                    name = "cot",
                    code = "COT",
                    Operate = (curr) => 1 / (float)Math.Tan(curr)
                },
                new CalcFunction()
                {
                    name = "sqrt",
                    code = "SQT",
                    Operate = (curr) => (float)Math.Sqrt(curr)
                },
                new CalcFunction()
                {
                    name = "abs",
                    code = "ABS",
                    Operate = (curr) => Math.Abs(curr)
                },
                new CalcFunction()
                {
                    name = "log",
                    code = "LOG",
                    Operate = (curr) => (float)Math.Log10(curr)
                },
                new CalcFunction()
                {
                    name = "ln",
                    code = "NLG",
                    Operate = (curr) => (float)Math.Log(curr)
                },
                new CalcFunction()
                {
                    name = "min",
                    code = "MIN",
                    ListOperate = (list) =>
                    {
                        float lowest = list[0];
                        foreach(float item in list)
                        {
                            if (item < lowest)
                            {
                                lowest = item;
                            }
                        }
                        return lowest;
                    }
                },
                new CalcFunction()
                {
                    name = "max",
                    code = "MAX",
                    ListOperate = (list) =>
                    {
                        float highest = list[0];
                        foreach(float item in list)
                        {
                            if (item > highest)
                            {
                                highest = item;
                            }
                        }
                        return highest;
                    }
                },
                new CalcFunction()
                {
                    name = "gcd",
                    code = "GCD",
                    maxListArgs = 2,
                    ListOperate = (list) => Gcd(list)
                },
                new CalcFunction()
                {
                    name = "lcm",
                    code = "LCM",
                    maxListArgs = 2,
                    ListOperate = (list) =>
                    {
                        float a = list[0];
                        float b = list[1];
                        return a * b / Gcd(new List<float> { a, b });
                    }
                },
            };

        private static float Gcd(List<float> list)
        {
            float a = list[0];
            float b = list[1];
            if (b == 0)
            {
                return a;
            }
            else
            {
                return Gcd(new List<float> { b, a % b });
            }
        }

        public SimpleCalculator()
        {
            parser = new Parser();
            runner = new SyntaxTree();

            parser.SetOperators(operators);
            parser.SetFunctions(functions);
            runner.SetOperators(operators);
            runner.SetFunctions(functions);

            runner.SetVariable("pi", (float)Math.PI);
            runner.SetVariable("e", (float)Math.E);
        }

        public void Clear()
        {
            parser.Clear();
            runner.Clear();
        }

        public void SetString(string input)
        {
            parser.SetInputString(input);
        }

        public float Calculate()
        {
            try
            {
                parser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Parsing Input!: " + e.ToString());
            }
            runner.SetCommandList(parser.GetCommandStack());
            try
            {
                runner.Build();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Compiling Input!: " + e.ToString());
            }
            try
            {
                return runner.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Running!: " + e.ToString());
            }
            return -1;
        }
    }
}
