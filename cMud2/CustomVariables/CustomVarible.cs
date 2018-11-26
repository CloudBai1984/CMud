using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace cMud2
{
    public class CustomVariable
    {
        public static Hashtable Variables = new Hashtable();

        public static void AddVariable(string name, string value)
        {
            if (string.IsNullOrEmpty(name) || value.Contains("@")) return;
            if (string.IsNullOrEmpty(value))
            {
                Variables.Remove(name);
                return;
            }
            if (Variables.Contains(name))
                Variables.Remove(name);
            Variables.Add(name, value);
        }

        public static string GetVariable(string name)
        {
            string tmp = "";
            if (Variables.Contains(name))
            {
                tmp = Variables[name].ToString();
            }
            else
            {
                System.Windows.MessageBox.Show("不存在" + name + "自定义变量");
            }
            return tmp;
        }

        public static void RemoveVariable(string name)
        {
            Variables.Remove(name);
        }

        public static void InitVariable()
        {
            using (StreamReader sr = new StreamReader(@"Config\Variables.txt", Encoding.Default))
            {
                string var = string.Empty;
                do
                {
                    var = sr.ReadLine();
                    if (string.IsNullOrEmpty(var)) break;
                    try
                    {
                        string[] arrVar = var.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!Variables.ContainsKey(arrVar[0]))
                            Variables.Add(arrVar[0], arrVar[1]);
                    }
                    catch (Exception) { }
                } while (true);
            }
        }

        public static void SaveVariable()
        {
            lock (GlobalParams.Locker)
            {
                using (StreamWriter sw = new StreamWriter(@"Config\Variables.txt", false, Encoding.Default))
                {
                    foreach (string key in Variables.Keys)
                    {
                        sw.WriteLine(key + "=" + Variables[key]);
                    }
                }
            }
        }
    }
}
