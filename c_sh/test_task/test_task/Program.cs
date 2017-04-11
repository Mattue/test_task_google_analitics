using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace test_task
{
    public class sourse
    {
        public string name;
        public int count;
        public double amount;
    }

    class Program
    {

        static bool get_end_of_cicle(string a)
        {
            try
            {
                a = a.Remove(1, a.Length - 1);
                double dob = Convert.ToDouble(a);
                //Console.WriteLine(amount);
            }
            catch (FormatException)
            {
                //Console.WriteLine("Format err");
                return true;
            }
            catch(InvalidCastException)
            {
                //Console.WriteLine("char");
                return true;
            }
            catch (OverflowException)
            {
                //Console.WriteLine("Over");
                return true;
            }
            return false;
        }

        static void Main(string[] args)
        {
            string line = "";
            string buf_line = "";

            double final_amount = 0;

            //List<string> sourses = new List<string> { };

            List<sourse> sourses = new List<sourse> { };

            StreamReader file;

            try
            {
                file = new StreamReader("data.csv");
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("File Not Found");
                Console.ReadLine();
                return;
            }

            //reading free first 7 lines
            for (int i = 1; i < 8; i++)
            {
                line = file.ReadLine();
            }

            //Console.WriteLine(line = file.ReadLine());

            while((line = file.ReadLine()) != null)
            {
                for (int i = 0; i < sourses.Count; i++)
                {
                    sourses[i].count = 0;
                }

                buf_line = line;

                buf_line = buf_line.Replace("\"", "");

                //считываем сумму для этой строчки
                //var match = Regex.Match(buf_line, @"[0-9][0-9]+(?:\.[0-9]*)?");
                
                var match = Regex.Match(buf_line, @"(?<=(\$)).*");
                if (match.Success)
                {
                    //Console.WriteLine(match.ToString());
                }
                string amount_str = match.ToString();
                //amount_str = amount_str.Replace(".", ",");
                //amount_str = "1,5";

                double amount = 0;

                try
                {
                    amount = Convert.ToDouble(amount_str);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Format err");
                }
                catch (OverflowException)
                {
                    //Console.WriteLine("Over");
                }

                final_amount += amount;

                while (get_end_of_cicle(buf_line))
                {
                    //смотрим ближайший канал

                    sourse chanel = new sourse();
                    chanel.name = "";
                    chanel.count = 1;
                    chanel.amount = 0;

                    //string chanel = "";
                    match = Regex.Match(buf_line, @"(.+?)(\/)");
                    if (match.Success)
                    {
                        //Console.WriteLine(match.ToString());
                        chanel.name = match.ToString();
                        chanel.name = chanel.name.Substring(0, chanel.name.Length - 2);
                        //Console.WriteLine(chanel);
                    }

                    //Console.WriteLine(sourses.Find((x) => x == chanel));

                    if (!chanel.name.Equals("(direct)"))
                    {
                        int index = sourses.FindIndex(x => x.name == chanel.name);
                        //var foundIt = sourses.Find((x) => x.name == chanel.name);
                        if (index >= 0)
                        {
                            //этот канал уже есть, поднимаем ему счетчик
                            sourses[index].count++;
                        }
                        else
                        {
                            //канала нет, заводим его и поднимаем счетчик
                            sourses.Add(chanel);
                        }
                    }

                    //удаление того, что мы уже посмотрели.
                    match = Regex.Match(buf_line, @"(?<=((\s\>)|(\,))).*");
                    if (match.Success)
                    {
                        buf_line = match.ToString();
                        buf_line = buf_line.Trim();
                    }
                }

                int koef = 0;

                for (int i = 0; i < sourses.Count; i++)
                {
                    koef += sourses[i].count;
                }

                double one_part = amount / koef;

                for (int i = 0; i < sourses.Count; i++)
                {
                    sourses[i].amount += one_part * sourses[i].count;
                }
            }

            file.Close();

            //File.WriteAllLines("output_JOPA.txt", "asdasd");

            string[] outputStringMas = new string[sourses.Count+1];

            double final_amount_test = 0;

            for(int i = 0; i < sourses.Count; i++)
            {
                Console.Write(sourses[i].name + " ");
                Console.WriteLine("{0:0.00}", sourses[i].amount);
                outputStringMas[i] = sourses[i].name + ",";
                final_amount_test += sourses[i].amount;
                outputStringMas[i] += Math.Round(sourses[i].amount, 2);
            }

            //outputStringMas[sourses.Count] = Convert.ToString(Math.Round(final_amount,2));

            File.WriteAllLines("output.csv", outputStringMas);

            Console.WriteLine("{0:0.00}",final_amount);
            //Console.WriteLine("{0:0.00}",final_amount_test);

            //sourses.ForEach(Console.WriteLine);
            //Console.WriteLine(buf_line);

            Console.ReadLine();
        }
    }
}


/*
            //смотрим ближайший канал
            string chanel = "";
            match = Regex.Match(buf_line, @"(.+?)(\/)");
            if (match.Success)
            {
                Console.WriteLine(match.ToString());
                chanel = match.ToString();
                chanel = chanel.Substring(0, chanel.Length - 2);
                Console.WriteLine(chanel);
            }

            Console.WriteLine(sourses.Find((x) => x == chanel));
            
            if (sourses.Find((x) => x == chanel) != null)
            {
                Console.WriteLine("not null");
                //этот канал уже есть, поднимаем ему счетчик
            }
            else
            {
                Console.WriteLine("null");
                //канала нет, заводим его и поднимаем счетчик
                sourses.Add(chanel);
            }

            //удаление того, что мы уже посмотрели.
            match = Regex.Match(buf_line, @"(?<=\>).*");
            if (match.Success)
            {
                Console.WriteLine(match.ToString());
                buf_line = match.ToString();
                buf_line = buf_line.Trim();
                Console.WriteLine(buf_line);
            }
 */
