using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace RPN_App.CsvHelper
{
    public class List2DRecord<T> {
        public List<List<T>> list { get; set; }
        public string list_name { get; set; }

        public List2DRecord(List<List<T>> list, string name)
        {
            this.list = list;
            this.list_name = name;
        }
    }

    public class DictRecord<T>
    {
        public Dictionary<string, int> dict { get; set; }
        public string dict_name { get; set; }

        public DictRecord(Dictionary<string, int> dict, string name)
        {
            this.dict = dict;
            this.dict_name = name;
        }
    }

    public class CsvManager
    {

        public static string ReadCsvFile(string path)
        {
            string value = "";
            using (var streamReader = File.OpenText("users.csv"))
            {
                using (var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture))
                {
                    //csvReader.Configuration.HasHeaderRecord = true;
                    while (csvReader.Read())
                    {
                        for (int i = 0; csvReader.TryGetField<string>(i, out value); i++)
                        {
                            Console.Write($"{value} ");
                        }

                        Console.WriteLine();
                    }
                }
            }
            return value;
        }

        public static void WriteCsvFile()
        {

        }

        public static void ReadClassCsv(string path)
        {
            using var streamReader = File.OpenText("users.csv");
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            /*var users = csvReader.GetRecords<User>();

            foreach (var user in users)
            {
                Console.WriteLine(user);
            }*/

            //record User(string FirstName, String LastName, string Occupation);
        }

        public static void Write2DListCsv<T>(List<List<T>> classes)
        {
            using var mem = new MemoryStream();
            using var writer = new StreamWriter(mem);
            using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

            // creating header
            for (int i = 0; i < classes[0].Count; i++)
            {
                csvWriter.WriteField("A" + i.ToString("00000"));    
            }
            csvWriter.NextRecord();

            // writing data to csv file
            foreach (var user in classes)
            {
                foreach (var num in user)
                {
                    csvWriter.WriteField(num);
                }
                csvWriter.NextRecord();
            }

            writer.Flush();
            var result = Encoding.UTF8.GetString(mem.ToArray());
            Console.WriteLine(result);
        }

        public static void Write3DListCsv<T>(List<List2DRecord<T>> lists)
        {
            using (var mem = new MemoryStream())
            {
                using (var writer = new StreamWriter(mem))
                {
                    using (var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture))
                    {

                        // creating header
                        foreach (var l2drec in lists)
                        {
                            for (int i = 0; i < l2drec.list[0].Count; i++)
                            {
                                csvWriter.WriteField(l2drec.list_name + i.ToString("00000"));
                            }
                        }
                        csvWriter.NextRecord();

                        // writing data to csv file
                        long x_num_amount = 0;
                        foreach (var list in lists)
                        {
                            x_num_amount += list.list[0].Count;
                        }

                        for (var i = 0; i < lists[0].list.Count; i++)
                        {
                            int k = 0;
                            int temp_x_Pos = 0;
                            for (int j = 0; j < x_num_amount; j++)
                            {
                                csvWriter.WriteField(lists[k].list[i][j - temp_x_Pos]);
                                if (j - temp_x_Pos + 1 == lists[k].list[i].Count)
                                {
                                    temp_x_Pos += lists[k].list[i].Count;
                                    k++;
                                }
                            }
                            csvWriter.NextRecord();
                        }

                        writer.Flush();
                        var result = Encoding.UTF8.GetString(mem.ToArray());
                        Console.WriteLine(result);
                    }
                }
            }
        }

        public static void WriteDictToCsv(Dictionary<string, int> coefs, string path)
        {
            using var mem = new MemoryStream();
            using var writer = new StreamWriter(mem);
            using var csvWriter = new CsvWriter(writer, CultureInfo.CurrentCulture);

            // creating header
            foreach(var keypair in coefs)
            {
                csvWriter.WriteField(keypair.Key);
            }
            csvWriter.NextRecord();

            // writing data to csv file
            foreach (var keypair in coefs)
            {
                csvWriter.WriteField(keypair.Value);
            }
            csvWriter.NextRecord();

            writer.Flush();
            var byte_arr = mem.ToArray();
            //var result = Encoding.UTF8.GetString(mem.ToArray());

            // запись в файл
            using (FileStream fstream = new FileStream($"{path}note.csv", FileMode.OpenOrCreate))
            {
                // преобразуем строку в байты
                //byte[] array = System.Text.Encoding.Default.GetBytes(text);
                // запись массива байтов в файл
                fstream.Write(byte_arr, 0, byte_arr.Length);
                Console.WriteLine("Текст записан в файл");
            }

            //Console.WriteLine(result);
        }

    }
}
