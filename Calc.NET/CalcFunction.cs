using System;
using System.Collections.Generic;
using System.Text;

namespace Calc.NET
{
    public class CalcFunction
    {
        public string name;
        public string code;
        public delegate float Operation(float current);
        public Operation Operate;
        public delegate float ListOperation(List<float> listedItems);
        public ListOperation ListOperate;
        public int maxListArgs = -1;
    }
}
