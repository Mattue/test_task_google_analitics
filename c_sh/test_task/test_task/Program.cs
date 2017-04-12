using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
//using Excel;

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

        const string input_file_name = "data.csv";

        static bool get_end_of_cicle(string a)
        {
            try
            {
                a = a.Remove(1, a.Length - 1);
                double dob = Convert.ToDouble(a);
            }
            catch (FormatException)
            {
                return true;
            }
            catch(InvalidCastException)
            {
                return true;
            }
            catch (OverflowException)
            {
                return true;
            }
            return false;
        }

        static void writeExcelFile(List<sourse> sourses)
        {
            Excel excel_file = new Excel();
            excel_file.NewDocument();
            excel_file.SetValue("A1", "Источник");
            excel_file.SetValue("B1", "Сумма");

            int j = 2;

            foreach(sourse channel in sourses)
            {
                excel_file.SetValue("A" + j, channel.name);
                excel_file.SetValue("B" + j, Convert.ToString(channel.amount));
                j++;
            }

            excel_file.CloseDocument();
        }

        static void Main(string[] args)
        {

            string line = "";
            string buf_line = "";

            double final_amount = 0;

            List<sourse> sourses = new List<sourse> { };

            StreamReader file;
         
            try
            {
                file = new StreamReader(input_file_name);
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

            while((line = file.ReadLine()) != null)
            {
                //проверка на нужность этой строчки
                var match = Regex.Match(line, @"(display|cpm|banner)+|(cpc|ppc|paidsearch)+|(referral)+");
                if (!match.Success)
                {
                    continue;
                }

                foreach (sourse channel in sourses)
                {
                    channel.count = 0;
                }

                buf_line = line;

                buf_line = buf_line.Replace("\\", "");

                //считываем сумму для этой строчки
                //var match = Regex.Match(buf_line, @"[0-9][0-9]+(?:\.[0-9]*)?");
                string amount_str;
                match = Regex.Match(buf_line, @"(?<=(\"")).*");
                if (match.Success)
                {
                    amount_str = match.ToString();
                }
                else
                {
                    continue;
                }

                amount_str = amount_str.Replace(" $\"", "");
                amount_str = amount_str.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                amount_str = amount_str.Replace(" ", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);
                //amount_str = amount_str.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator);

                double amount = 0;

                try
                {
                    amount = Convert.ToDouble(amount_str);
                }
                catch (FormatException ex)
                {
                    Console.WriteLine(amount_str);
                    Console.WriteLine("Format err " + ex.ToString());
                    Console.ReadLine();
                    return;
                }
                catch (OverflowException ex)
                {
                    Console.WriteLine("Over " + ex.ToString());
                    Console.ReadLine();
                }

                //0е конверсии тоже не нужны
                if (amount == 0)
                    continue;

                final_amount += amount;

                while (get_end_of_cicle(buf_line))
                {
                    //смотрим ближайший канал
                    sourse chanel = new sourse();
                    chanel.name = "";
                    chanel.count = 1;
                    chanel.amount = 0;

                    match = Regex.Match(buf_line, @"(.+?)(\/)");
                    if (match.Success)
                    {
                        chanel.name = match.ToString();
                        chanel.name = chanel.name.Substring(0, chanel.name.Length - 2);
                    }
                    else
                    {
                        Console.WriteLine("err finding chanel name");
                        continue;
                    }


                    if (!chanel.name.Equals("(direct)"))
                    {
                        int index = sourses.FindIndex(x => x.name == chanel.name);
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

                foreach(sourse channel in sourses)
                {
                    koef += channel.count;
                }

                double one_part = Math.Round(amount / koef, 2);

                foreach(sourse channel in sourses)
                {
                    channel.amount += one_part * channel.count;
                }
            }

            file.Close();

            for(int i = 0; i < sourses.Count; i++)
            {
                Console.Write(sourses[i].name + " ");
                Console.WriteLine("{0:0.00}", sourses[i].amount);
            }

            Console.WriteLine("{0:0.00}",final_amount);

            writeExcelFile(sourses);

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
