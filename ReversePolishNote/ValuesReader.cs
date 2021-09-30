using RPN_App.CsvHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPN_App.ReversePolishNote
{
    public class ValueRange<T1,T2>
    {
        public string name { get; set; }
        public T1 start_value { get; set; }
        public T1 endValue { get; set; }
        public T2 step { get; set; }
    }
    public class ValuesReader
    {
        private List<DictRecord<float>> dict_list;
        private List<ValueRange<int, int>> valueRanges;

        public void InitializeValuesForESum(List<DictRecord<float>> dict_list, List<ValueRange<int, int>> valueRanges)
        {
            this.dict_list = dict_list;
            this.valueRanges = valueRanges;
        }
        
        public void InitializeValuesForESum(List<DictRecord<float>> dict_list, string valueRanges)
        {
            this.dict_list = dict_list;
            this.valueRanges = GetValues(valueRanges);
        }


        public void SetRangeValues(string valueRanges)
        {
            this.valueRanges = GetValues(valueRanges);
        }


        public List<string> LinearizeSum(string value, string indexes, char symb = '+', string val_range = "")
        {
            //string result = "";
            List<string> sum_val = new List<string>();
            if (valueRanges == null || val_range != "")
            {
                this.valueRanges = GetValues(val_range);
            }

            if (value.Contains('+'))
            {
                string new_val = value;
                if (value[0] == '(')
                    new_val = value.Substring(1, value.Length - 2);

                string[] val_list = new_val.Split('+');
                foreach (var str in val_list)
                {
                    sum_val.AddRange(LinearizeSum(str, indexes));
                }
            }
            else if (value.Contains('-'))
            {
                string new_val = value;
                if (value[0] == '(')
                    new_val = value.Substring(1, value.Length - 2);

                string[] val_list = new_val.Split('-');
                sum_val.AddRange(LinearizeSum(val_list[0], indexes));
                for (int i = 1; i < val_list.Length; i++)
                {
                    sum_val.AddRange(LinearizeSum(val_list[i], indexes, '-'));
                }
            }
            else
            {
                int sq_braket_pos = indexes.IndexOf('[') + 1;
                string index_sub = indexes.Substring(sq_braket_pos, indexes.Length - sq_braket_pos - 1);
                string[] list_ind = index_sub.Split(',');
                if (list_ind.Length > 0)
                {
                    string sign = "";
                    if (symb == '-')
                        sign = "_";


                    foreach (var index in list_ind)
                    {
                        ValueRange<int, int> range = FindRange(index);
                        if (range != null)
                        {
                            for (int i = range.start_value; i <= range.endValue; i += range.step)
                            {
                                sum_val.Add(sign + value.Replace(range.name, i.ToString()));
                            }
                        }
                    }
                }
            }
            return sum_val;
        }

        private ValueRange<int, int> FindRange(string index_name)
        {
            ValueRange<int, int> r = null;
            foreach(var val in valueRanges)
            {
                if(val.name == index_name)
                {
                    r = val;
                    break;
                }
            }
            return r;
        }

        /// <summary>
        /// Метод для расчета суммы елементов заданого масива в заданом диапазоне и с заданым шагом
        /// с рандомной генерацией масива(пока не готово)
        ///  ------------------------------- ПОКА ЧТО НЕ ИСПОЛЬЗОВАТЬ ---------------------------------------------
        /// </summary>
        /// <param name="index_list"></param>
        /// <param name="variable"></param>
        /// <param name="valRanges"></param>
        /// <returns></returns>
        public float ESumWithRandom(string index_list, string variable, string valRanges = "")
        {
            float value = 0;
            if (valRanges == "")
            {
                this.valueRanges = GetValues(valRanges);
            }
            else if (valueRanges == null || valueRanges.Count == 0)
            {
                Console.WriteLine("Please define index ranges before run this method!!!");
                return 0;
            }

            string indexes = index_list.Substring(1, index_list.Length - 2);
            string[] list_ind = indexes.Split(',');
            if (list_ind.Length > 0)
            {
                switch (list_ind.Length)
                {
                    case 1:
                        int[] arr = new int[valueRanges[0].endValue + 1]; // TODO Сделать генерацию рандомного масива
                        value = Array1DSum(valueRanges, arr);
                        break;
                    case 2:
                        int[,] arr2 = new int[valueRanges[0].endValue + 1,0]; // TODO Сделать генерацию рандомного масива
                        value = Array2DSum(valueRanges, arr2);
                        break;
                    case 3:
                        int[,,] arr3 = new int[valueRanges[0].endValue + 1,0,0]; // TODO Сделать генерацию рандомного масива
                        value = Array3DSum(valueRanges, arr3);
                        break;
                    case 4:
                        int[,,,] arr4 = new int[valueRanges[0].endValue + 1,0,0,0]; // TODO Сделать генерацию рандомного масива
                        value = Array4DSum(valueRanges, arr4);
                        break;
                    default:
                        value = 0;
                        Console.WriteLine("Sorry such dimensions is not yet supported!");
                        break;
                }
            }
            else
            {
                throw new Exception("Error, invalid format of indexes found! List: " + index_list + " \n");
            }
            return value;
        }

        public float ESumWithGivenArray(string arr_name, string index_list, object array, string valRanges = "")
        {
            float value = 0;
            
            if (valueRanges == null)
            {
                this.valueRanges = GetValues(valRanges);
            }
            else if (valueRanges == null || valueRanges.Count == 0)
            {
                Console.WriteLine("Please define index ranges before run this method!!!");
                return 0;
            }

            string indexes = index_list.Substring(1, index_list.Length - 2);
            string[] list_ind = indexes.Split(',');
            if (list_ind.Length > 0)
            {
                switch (list_ind.Length)
                {
                    case 1:
                        int[] arr = (int[]) array; // TODO избежать этого(unboxing) в будущем, если прийдеться где-то ипользовать
                        value = Array1DSum(valueRanges, arr);
                        break;
                    case 2:
                        int[,] arr2 = (int[,])array; // TODO избежать этого(unboxing) в будущем, если прийдеться где-то ипользовать
                        value = Array2DSum(valueRanges, arr2);
                        break;
                    case 3:
                        int[,,] arr3 = (int[,,])array; // TODO избежать этого(unboxing) в будущем, если прийдеться где-то ипользовать
                        value = Array3DSum(valueRanges, arr3);
                        break;
                    case 4:
                        int[,,,] arr4 = (int[,,,])array; // TODO избежать этого(unboxing) в будущем, если прийдеться где-то ипользовать
                        value = Array4DSum(valueRanges, arr4);
                        break;
                    default:
                        value = 0;
                        Console.WriteLine("Sorry such dimensions is not yet supported!");
                        break;
                }
            }
            else
            {
                throw new Exception("Error, invalid format of indexes found! List: " + index_list + " \n");
            }
            return value;
        }

        /// <summary>
        ///  function for calculating Sum for variables with defined list of indexes, for example: [i,j,k,p]
        /// </summary>
        /// <param name="index_list"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public float ESum(string index_list, string variable)
        {
            float value = 0;
            string indexes = index_list.Substring(1, index_list.Length - 2);
            string[] list_ind = indexes.Split(',');
            if (list_ind.Length > 0)
            {
                value = ESumOperation(list_ind, variable);
            }
            else
            {
                throw new Exception("Error, invalid format of indexes found! List: " + index_list + " \n");
            }
            return value;
        }

        public float ESumOperation(string[] index_list, string variable, string val_n="", int pos = 0)
        {
            DictRecord<float> curDict = dict_list.Find(d => d.dict_name == variable);
            if (curDict == null)
                return -1;
            float res_value = 0;

            ValueRange<int, int> range = valueRanges.Find(r => r.name == index_list[pos]);
            for(int i = range.start_value; i <= range.endValue; i+=range.step)
            {
                if (val_n == "")
                    val_n = variable;

                if(pos == index_list.Length - 1)
                {
                    res_value += ESumOperation(index_list, variable, val_n + i.ToString(), pos++);
                } else
                {
                    res_value += curDict.dict.GetValueOrDefault(val_n + i.ToString());
                }
            }
            return res_value;

        }
        public List<ValueRange<int, int>> GetValues(string str)
        {
            List<ValueRange<int, int>> valueRanges = new List<ValueRange<int, int>>();
            var arr = str.Split(";");
            foreach (var el in arr)
            {
                if (el == "")
                    continue;

                int pos = el.IndexOf('=');
                if (pos != -1 && pos != 0)
                {
                    ValueRange<int, int> range = new ValueRange<int, int>();
                    range.name = el.Substring(0, pos);
                    int pos2 = el.IndexOf("..");
                    string fl_str = el.Substring(pos + 1, pos2 - pos - 1);
                    range.start_value = int.Parse(fl_str);
                    int pos3 = el.IndexOf('$');
                    range.endValue = int.Parse(el.Substring(pos2 + 2, pos3 - pos2 - 2));
                    range.step = int.Parse(el.Substring(pos3 + 1, el.Length - 1 - pos3));
                    valueRanges.Add(range);
                }
                else
                {
                    Console.WriteLine("Error, the string is in bag format");
                }
            }
            return valueRanges;
        }

        public float Array1DSum(List<ValueRange<int, int>> range, int[] arr)
        {
            if (arr.Length <= range[0].endValue)
            {
                Console.WriteLine("Error the range end_value index is bigger than arrray size!!!");
                return 0;
            }

            float result = 0;
            for(int i = range[0].start_value; i <= range[0].endValue; i+= range[0].step)
            {
                result += arr[i];
            }
            return result;
        }

        public float Array2DSum(List<ValueRange<int, int>> ranges, int[,] arr)
        {
            if(ranges.Count <=1)
            {
                Console.WriteLine("Error. the ranges list is smaller than arr dimension");
                return 0;
            }

            if (arr.GetLength(0) <= ranges[0].endValue || arr.GetLength(1) <= ranges[1].endValue)
            {
                Console.WriteLine("Error the range end_value index is bigger than arrray size!!!");
                return 0;
            }

            float result = 0;
            for (int i = ranges[0].start_value; i <= ranges[0].endValue; i += ranges[0].step)
            {
                for (int j = ranges[1].start_value; j <= ranges[1].endValue; j += ranges[1].step)
                {
                    result += arr[i,j];
                }
            }
            return result;
        }

        public float Array3DSum(List<ValueRange<int, int>> ranges, int[,,] arr)
        {
            if (ranges.Count <= 2)
            {
                Console.WriteLine("Error. the ranges list is smaller than arr dimension");
                return 0;
            }

            if (arr.GetLength(0) <= ranges[0].endValue || arr.GetLength(1) <= ranges[1].endValue || arr.GetLength(2) <= ranges[2].endValue)
            {
                Console.WriteLine("Error the range end_value index is bigger than arrray size!!!");
                return 0;
            }

            float result = 0;
            for (int i = ranges[0].start_value; i <= ranges[0].endValue; i += ranges[0].step)
            {
                for (int j = ranges[1].start_value; j <= ranges[1].endValue; j += ranges[1].step)
                {
                    for (int k = ranges[2].start_value; k <= ranges[2].endValue; k += ranges[2].step)
                    {
                        result += arr[i,j,k];
                    }
                }
            }
            return result;
        }

        public float Array4DSum(List<ValueRange<int, int>> ranges, int[,,,] arr)
        {
            if (ranges.Count <= 3)
            {
                Console.WriteLine("Error. the ranges list is smaller than arr dimension");
                return 0;
            }

            if (arr.GetLength(0) <= ranges[0].endValue 
                || arr.GetLength(1) <= ranges[1].endValue 
                || arr.GetLength(2) <= ranges[2].endValue
                || arr.GetLength(3) <= ranges[3].endValue)
            {
                Console.WriteLine("Error the range end_value index is bigger than arrray size!!!");
                return 0;
            }

            float result = 0;
            for (int i = ranges[0].start_value; i <= ranges[0].endValue; i += ranges[0].step)
            {
                for (int j = ranges[1].start_value; j <= ranges[1].endValue; j += ranges[1].step)
                {
                    for (int k = ranges[2].start_value; k <= ranges[2].endValue; k += ranges[2].step)
                    {
                        for (int p = ranges[3].start_value; p <= ranges[3].endValue; p += ranges[3].step)
                        {
                            result += arr[i,j,k,p];
                        }
                    }
                }
            }
            return result;
        }
    }
}
