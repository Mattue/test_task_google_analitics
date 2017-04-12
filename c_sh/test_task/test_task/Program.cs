using System;
using System.Collections.Generic;
using System.IO;
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
            //working while we can't get succesed convert from string to double
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
                Console.WriteLine("File Not Found.");
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
                //is line reletive
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

                //reading amount in line
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

                //we dont need zero conversions
                if (amount == 0)
                    continue;

                final_amount += amount;

                while (get_end_of_cicle(buf_line))
                {
                    //looking for the closest channel
                    sourse channel = new sourse();
                    channel.name = "";
                    channel.count = 1;
                    channel.amount = 0;

                    match = Regex.Match(buf_line, @"(.+?)(\/)");
                    if (match.Success)
                    {
                        channel.name = match.ToString();
                        channel.name = channel.name.Substring(0, channel.name.Length - 2);
                    }
                    else
                    {
                        Console.WriteLine("err finding channel name");
                        continue;
                    }


                    if (!channel.name.Equals("(direct)"))
                    {
                        int index = sourses.FindIndex(x => x.name == channel.name);
                         if (index >= 0)
                        {
                            //channel already exists, raising count
                            sourses[index].count++;
                        }
                        else
                        {
                            //channel does not exists, create and raise count
                            sourses.Add(channel);
                        }
                    }

                    //deleting what we already seen
                    match = Regex.Match(buf_line, @"(?<=((\s\>)|(\,))).*");
                    if (match.Success)
                    {
                        buf_line = match.ToString();
                        buf_line = buf_line.Trim();
                    }
                }

                //calculating amount for each channel

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