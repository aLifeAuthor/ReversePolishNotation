using RPN_App.CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPN_App.ReversePolishNote
{ 
    public class RPNCreator
    {
        public static readonly List<Tuple<int, char>> operations = new List<Tuple<int, char>>();
        public string calc_input_string { get; set; }
        public string calc_output_string { get; private set; }
        private Stack<char> _stack { get; set; }
        private Stack<string> _out_stack { get; set; }
        private ValuesReader _valuesReader { get; set; }
        private object values_arr { get; set; }

        public RPNCreator(string input, string ranges_str)
        {
            calc_input_string = input;
            operations.Add(new Tuple<int, char>(1, '+'));
            operations.Add(new Tuple<int, char>(1, '-'));
            operations.Add(new Tuple<int, char>(2, '*'));
            operations.Add(new Tuple<int, char>(2, '/'));
            operations.Add(new Tuple<int, char>(3, '^'));
            operations.Add(new Tuple<int, char>(4, '_')); // унарний мінус
            operations.Add(new Tuple<int, char>(5, 'E')); // знак суми

            _out_stack = new Stack<string>();
            _stack = new Stack<char>();
            _valuesReader = new ValuesReader();
            _valuesReader.SetRangeValues(ranges_str);
        }

        public void SetArray(object arr)
        {
            this.values_arr = arr;
        }

        public Stack<string> RunReversing()
        {
            string one_part = "";
            for(int i = 0; i < calc_input_string.Length; i++)
            {
                int sop = 0; // search_operation_priority
                if (calc_input_string[i] == '(')
                {
                    _stack.Push(calc_input_string[i]);
                }
                else if (calc_input_string[i] == ')')
                {
                    if (one_part != "")
                    {
                        _out_stack.Push(one_part);
                        one_part = "";
                    }

                    char lastChar = _stack.Pop();
                    while (lastChar != '(')
                    {
                        _out_stack.Push(lastChar.ToString());
                        lastChar = _stack.Pop();
                    }
                }
                else
                {
                    sop = TupleListContain(operations, calc_input_string[i]);
                    if (sop == 0)
                    {
                        one_part += calc_input_string[i];
                        if (calc_input_string[i] == ']')
                        {
                            _out_stack.Push(one_part);
                            one_part = "";
                        }
                    }
                    else
                    {
                        if (one_part != "")
                            _out_stack.Push(one_part);

                        if (_stack.Count > 0 && sop <= TupleListContain(operations, _stack.Peek()))
                        {
                            //if(sop == 5 && _out_stack.Count > 0)
                            _out_stack.Push(_stack.Pop().ToString());
                        }
                        _stack.Push(calc_input_string[i]);
                        one_part = "";
                    }
                }
            }
            if(one_part != "")
                _out_stack.Push(one_part);

            int stack_size = _stack.Count;
            for(int i = 0; i < stack_size; i++)
            {
                _out_stack.Push(_stack.Pop().ToString());
            }
            return _out_stack;
        } 

        private int TupleListContain(List<Tuple<int,char>> list, char find_var)
        {
            int res = 0;
            foreach(var tuple in list)
            {
                if(tuple.Item2 == find_var)
                {
                    res = tuple.Item1;
                    break;
                }
            }
            return res;
        }

        public float CalculateRPN(string[] arr)
        {
            Stack<string> stack = new Stack<string>();
            float value = 0;
            float num1, num2;
            for(int i = 0; i < arr.Length; i++)
            {
                if (TupleListContain(operations, arr[i][0]) == 0)
                {
                    stack.Push(arr[i]);
                }
                else {
                    if (arr[i] == "_")
                    {
                        num1 = float.Parse(stack.Pop());
                        num1 = -num1;
                        stack.Push(num1.ToString());
                    } 
                    else if(arr[i] == "E")
                    {
                        if(values_arr == null)
                        {
                            Console.WriteLine("Error given arr is null!");
                            return 0;
                        }
                        value = _valuesReader.ESumWithGivenArray(stack.Pop(), stack.Pop(), values_arr);
                        stack.Push(value.ToString());
                    }
                    else {
                        num2 = float.Parse(stack.Pop());
                        num1 = float.Parse(stack.Pop());
                        value = Operation(arr[i], num1, num2);
                        stack.Push(value.ToString());
                    }
                }
            }
            return float.Parse(stack.Pop());
        }

        public string LinearizeSumOfRPN(string[] arr)
        {
            Stack<string> stack = new Stack<string>();
            string value = "";
            string num1, num2;
            for (int i = 0; i < arr.Length; i++)
            {
                if (!IsOperation(arr[i][0]))
                {
                    stack.Push(arr[i]);
                }
                else
                {
                    if (arr[i] == "E")
                    {
                        num2 = stack.Pop();
                        num1 = stack.Pop();
                        List<string> sum_values = _valuesReader.LinearizeSum(num2, num1);
                        value = "(" + sum_values[0]; 
                        for(int j = 1; j < sum_values.Count; j++) 
                        {
                            value += "+" + sum_values[j];
                        }
                        value += ")"; 
                        stack.Push(value.ToString());
                    }
                    else if(arr[i] == "*" || arr[i] == "/" )
                    {
                        num2 = stack.Pop();
                        num1 = stack.Pop();
                        value = BracketOpener(num1, num2, arr[i]);
                        stack.Push(value);
                    }
                    else
                    {
                        num2 = stack.Pop();
                        num1 = stack.Pop();
                        value = '(' + num1 + arr[i] + num2 + ')';
                        stack.Push(value);
                    }
                }
            }
            value = stack.Pop();
            if(!value.Contains('*') && !value.Contains('/'))
            {
                string new_res = "";
                for(int i = 0; i < value.Length; i++)
                {
                    if(value[i] != '(' && value[i] != ')')
                    {
                        new_res += value[i];
                    }
                }
                value = new_res;
            }
            return value;
        }

        public string BracketOpener(string num1, string num2, string operation)
        {
            string result = "";
            if(num1[0] == '(')
            {
                string sub_str = num1.Substring(1, num1.Length - 2);
                string[] val_list = sub_str.Split("+");
                if(val_list != null && val_list.Length > 0)
                {
                    if(num2[0] == '(')
                    {
                        Console.WriteLine("Make method for multipling to bracket expresions");
                    }
                    else
                    {
                        string oper = "";
                        if (operation == "/")
                        {
                            oper = "^_1";
                        }
                        result += num2 + oper + val_list[0];
                        for (int j = 1; j < val_list.Length; j++)
                        {
                            result += "+" + num2 + oper + val_list[j];
                        }
                    }
                } else
                {
                    Console.WriteLine("BracketOpener: Error val list is null or empty!!!");
                }
            } else if(num2[0] == '(')
            {
                result = BracketOpener(num2, num1, operation);
            }
            return result;
        }
        public bool IsOperation(char val)
        {
            bool is_val_operation = false;
            switch(val)
            {
                case '+':
                    is_val_operation = true;
                    break;
                case '-':
                    is_val_operation = true;
                    break;
                case '*':
                    is_val_operation = true;
                    break;
                case '/':
                    is_val_operation = true;
                    break;
                case '^':
                    is_val_operation = true;
                    break;
                case 'E':
                    is_val_operation = true;
                    break;
            }
            return is_val_operation;
        }
        public float Operation(string o, float num1, float num2)
        {
            float result = 0;
            switch(o)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "*":
                    result = num1 * num2;
                    break;
                case "/":
                    result = num1 / num2;
                    break;
                case "^":
                    result = MathF.Pow(num1, num2);
                    break;
                default:
                    Console.WriteLine("Error cann`t define Operation");
                    break;
            }
            return result;
        }

        public Dictionary<string, int> getEquatationCoef(string line)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();
            if (line.Contains("-"))
            {
                line = line.Replace("-", "+#");
            }

            string[] arr = line.Split('+');
            if(arr != null && arr.Length > 0)
            {
                foreach(var str in arr)
                {
                    int pos = str.IndexOf('[');
                    if (pos == 1)
                    {
                        bool t = dict.ContainsKey(str);
                        if (t)
                        {
                            int res = dict[str];
                            res++;
                            dict[str] = res;
                        } else
                        {
                            dict.Add(str, 1);
                        }
                    }
                    else if (pos > 1)
                    {
                        string val = str.Substring(0, str.Length - pos - 3);
                        if (val[0] == '#')
                        {
                            string num_val = "";
                            for (int i = 0; i < val.Length; i++)
                            {
                                if (Char.IsDigit(val[i]))
                                {
                                    num_val += val[1];
                                }
                                if (num_val.Length == 0)
                                {
                                    val = "-1";
                                }
                                else
                                {
                                    val = "-" + num_val;
                                }
                            }
                        }

                        int coef = 0;
                        bool t1 = int.TryParse(val, out coef);
                        if(t1)
                        {
                            string key = str.Substring(pos - 1);
                            bool t2 = dict.ContainsKey(key);
                            if (t2)
                            {
                                int res = dict[key];
                                res += coef;
                                dict[key] = res;
                            }
                            else
                            {
                                dict.Add(key, coef);
                            }
                        } else
                        {
                            Console.WriteLine("Error while parsibng value: " + val);
                        }
                    }
                    else
                    {
                        if (dict.ContainsKey(str))
                        {
                            int val = dict[str];
                            dict[str] = val + 1;
                        }
                        else
                        {
                            dict.Add(str, 1);
                        }
                    }
                }
            }
            return dict;
        }
    }
}
