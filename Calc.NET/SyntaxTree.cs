using System;
using System.Collections.Generic;

namespace Calc.NET
{
    public class SyntaxTree
    {
        private List<string> commandList;
        private Node tree;
        private Dictionary<string, float> variables = new Dictionary<string, float>();
        private List<CalcOperator> operators;
        private List<CalcFunction> functions;
        private float lastAnswer = 0;

        public SyntaxTree() : this(null) { }

        public SyntaxTree(List<string> commandList)
        {
            this.commandList = commandList;
        }

        public float GetLastAnswer()
        {
            return this.lastAnswer;
        }

        public void SetOperators(List<CalcOperator> operators)
        {
            this.operators = operators;
        }

        public void SetFunctions(List<CalcFunction> functions)
        {
            this.functions = functions;
        }

        public void SetCommandList(List<string> commandList)
        {
            this.commandList = commandList;
        }

        public void SetVariable(string var, float value)
        {
            variables[var] = value;
        }

        public void RemoveVariable(string var)
        {
            variables.Remove(var);
        }

        public List<string> GetCommandList()
        {
            return this.commandList;
        }

        public void Clear()
        {
            commandList = null;
            tree = null;
        }

        public float Run()
        {
            if (tree == null)
            {
                throw new Exception("No parsed tree!");
            }
            if (operators == null || functions == null)
            {
                throw new Exception("No operators or functions specified!");
            }
            float result = tree.Evaluate();
            lastAnswer = result;
            SetVariable("ans", result);
            return result;
        }

        public void Build()
        {
            if (commandList == null)
            {
                throw new Exception("No commands! (this shouldn't happen.)");
            }
            if (operators == null || functions == null)
            {
                throw new Exception("No operators or functions specified!");
            }
            tree = new Node
            {
                master = this
            };
            Node current = tree;
            string leadingBuffer = string.Empty;
            foreach (string command in commandList)
            {
                if (command == "RIS")
                {
                    Node n = new Node
                    {
                        parent = current
                    };
                    current.nodes.Add((true, n));
                    current = n;
                    n.leadingCommand = leadingBuffer;
                    leadingBuffer = string.Empty;
                }
                else if (command == "LOW")
                {
                    current = current.parent;
                }
                else
                {
                    bool found = false;
                    foreach (CalcFunction calcFunction in functions)
                    {
                        if (command == calcFunction.code)
                        {
                            leadingBuffer = command;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        current.nodes.Add((false, command));
                    }
                }
            }
            tree.Order();
        }

        private class Node
        {
            public SyntaxTree master;
            public Node parent;
            public string leadingCommand = "";
            public List<(bool, object)> nodes = new List<(bool, object)>();
            private List<float> listedItems = new List<float>();

            public void Order()
            {
                foreach ((bool, object) node in this.nodes)
                {
                    if (node.Item1)
                    {
                        ((Node)node.Item2).Order();
                    }
                }
                Node tot = TopOfTree();
                for (int i = 0; i < this.nodes.Count; i++)
                {
                    (bool, object) node = this.nodes[i];
                    if (node.Item2 == null || node.Item1)
                    {
                        continue;
                    }
                    string command = (string)node.Item2;
                    foreach (CalcOperator calcOperator in tot.master.operators)
                    {
                        if (command == calcOperator.code && calcOperator.precedence)
                        {
                            if (i == 0 || i == this.nodes.Count - 1)
                            {
                                break;
                            }
                            (bool, object) n_before = this.nodes[i - 1];
                            (bool, object) n_after = this.nodes[i + 1];
                            if (n_before.Item2 == null || n_after.Item2 == null || n_before.Item1 || n_after.Item1)
                            {
                                break;
                            }
                            Node n = new Node
                            {
                                parent = this
                            };
                            n.nodes.Add((false, n_before.Item2));
                            n.nodes.Add((false, command));
                            n.nodes.Add((false, n_after.Item2));

                            n_before.Item2 = null;
                            n_after.Item2 = null;

                            node.Item1 = true;
                            node.Item2 = n;

                            this.nodes[i - 1] = n_before;
                            this.nodes[i] = node;
                            this.nodes[i + 1] = n_after;
                        }
                    }
                }
                List<(bool, object)> newNodes = new List<(bool, object)>();
                foreach ((bool, object) node in this.nodes)
                {
                    if (node.Item2 == null)
                    {
                        continue;
                    }
                    newNodes.Add(node);
                }
                this.nodes = newNodes;
            }

            private Node TopOfTree()
            {
                if (this.parent != null)
                {
                    return this.parent.TopOfTree();
                }
                else
                {
                    return this;
                }
            }

            public float Evaluate()
            {
                Node tot = TopOfTree();
                float res = 0;
                string op = tot.master.operators[0].code;
                foreach ((bool, object) node in this.nodes)
                {
                    if (node.Item1)
                    {
                        float get = ((Node)node.Item2).Evaluate();
                        res = Operate(res, get, op);
                    }
                    else
                    {
                        string code = (string)node.Item2;
                        if (code == "COM")
                        {
                            this.listedItems.Add(res);
                            res = 0;
                            op = tot.master.operators[0].code;
                        }
                        else if (float.TryParse(code, out float p_result))
                        {
                            res = Operate(res, p_result, op);
                        }
                        else if (tot.master.variables.ContainsKey(code.ToLower()))
                        {
                            res = Operate(res, tot.master.variables[code.ToLower()], op);
                        }
                        else
                        {
                            op = code;
                        }
                    }
                }
                this.listedItems.Add(res);
                if (!string.IsNullOrEmpty(leadingCommand))
                {
                    foreach (CalcFunction calcFunction in tot.master.functions)
                    {
                        if (leadingCommand == calcFunction.code)
                        {
                            if (listedItems.Count > 1)
                            {
                                if (calcFunction.ListOperate == null)
                                {
                                    throw new Exception("Function " + calcFunction.code + " not configured for list operations!");
                                }
                                if (calcFunction.maxListArgs != -1 && listedItems.Count > calcFunction.maxListArgs)
                                {
                                    throw new Exception("Function " + calcFunction.code + " only takes " + calcFunction.maxListArgs + " arguments! (" + listedItems.Count + " provided).");
                                }
                                res = calcFunction.ListOperate(listedItems);
                            }
                            else
                            {
                                if (calcFunction.Operate == null)
                                {
                                    throw new Exception("Function " + calcFunction.code + " not configured for normal operations!");
                                }
                                res = calcFunction.Operate(res);
                            }
                            break;
                        }
                    }
                }
                return res;
            }

            private float Operate(float curr, float neww, string op)
            {
                float res = curr;
                foreach (CalcOperator calcOperator in TopOfTree().master.operators)
                {
                    if (op == calcOperator.code)
                    {
                        res = calcOperator.Operate(curr, neww);
                        break;
                    }
                }
                return res;
            }
        }
    }
}
