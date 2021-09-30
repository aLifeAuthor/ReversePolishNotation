using RPN_App.ReversePolishNote;
using System;

namespace RPN_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            Console.WriteLine("Hello World!\n");
            bool need_repeat = true;
            //while (need_repeat)
            {
                string line0 = "i=1..3$1;"
                             + "j=1..2$1;"
                             + "k=3..4$1;"
                             + "c=1..4$1;";
                Console.WriteLine("Index ranges: " + line0);
                //string line1 = "a+b*c/((d-a)*i - b)^d"; 
                //string line1 = "2+3*1/((5-8)*2 - 5)^2+E[i,j,k,p]y"; 
                //string line1 = "E[k=k'..kmax](E[c](E[h](y[i,j,k,c,h]))+E[c](E[h](y[i,j,k,c,h]))) - M*(1-x[i,j,k,c,h])";
                string line1 = "E[i](E[j](x[i,j])) - x[1,1]";// "E[i](E[j](t[i,j] + f[i,j])) + 7*(E[j](E[i](f[j,i] + t[i,j]))) + 67";
                Console.WriteLine("Enter formula: " + line1);
                //string line1 = Console.ReadLine(); 
                if (!String.IsNullOrEmpty(line1))
                {
                    line1 = line1.Replace(" ", "");
                    RPNCreator rpn = new RPNCreator(line1, line0);
                    var stack = rpn.RunReversing();
                    var arr = stack.ToArray();
                    string[] reverse_arr = new string[arr.Length];
                    string out_str = "";
                    var length = arr.Length - 1;
                    for (int i = length; i >= 0; i--)
                    {
                        out_str += arr[i];
                        reverse_arr[length - i] = arr[i];
                    }
                    Console.WriteLine("Reverse Polish notation: " + out_str);

                    string lineariztion = rpn.LinearizeSumOfRPN(reverse_arr);
                    Console.WriteLine("Linearization: " + lineariztion);
                    var dict = rpn.getEquatationCoef(lineariztion);
                    CsvHelper.CsvManager.WriteDictToCsv(dict, "D:\\Education\\11 semestr\\ReversePolishNotation\\");
                    /*Console.WriteLine("\tStack: ");
                    for(int i = 0; i < reverse_arr.Length; i++)
                    {
                        Console.WriteLine($"\t{reverse_arr[i]}");
                    }*/
                    

                    int[,,,] array_y = new int[,,,]
                    {
                        {
                            { { 0, 1, 0, 2, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 1, 1, 2, 1 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 1, 0, 0, 8, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 0, 3, 7 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 1, 2, 0 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} }
                        },
                        {
                            { { 0, 1, 0, 2, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 1, 1, 2, 1 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 1, 0, 0, 8, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 0, 3, 7 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 1, 2, 0 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} }
                        },
                        {
                            { { 0, 1, 0, 2, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 1, 1, 2, 1 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 1, 0, 0, 8, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 0, 3, 7 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 1, 2, 0 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} }
                        },
                        {
                            { { 0, 1, 0, 2, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 1, 1, 2, 1 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 1, 0, 0, 8, 3 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 0, 3, 7 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} },
                            { { 0, 0, 1, 2, 0 }, { 7, 9, 0, 3 , 2 }, { 0, 1, 0, 4, 7 }, { 7, 5, 4, 1, 0}, { 0, 3, 6, 3, 7} }
                        },
                    };

                    rpn.SetArray(array_y);
                    //float result = rpn.CalculateRPN(reverse_arr);
                    //Console.WriteLine("\nResult = " + result.ToString()+"\n");

                }
                else
                {
                    Console.WriteLine("Error, do you wan`t exit? (Y - yes|N - no): ");
                    string answ = Console.ReadLine();
                    if (answ == "Y")
                        need_repeat = false;
                }
            }
        }
    }
}
