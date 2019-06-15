using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.NET
{
    public class CalcOperator
    {
        public char symbol;
        public string code;
        public delegate float Operation(float current, float newval);
        public Operation Operate;
        public bool precedence = false;
    }
}
