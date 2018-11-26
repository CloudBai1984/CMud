using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace cMud2
{
    public class AliasProcessor
    {
        private static AliasProcessor Instance;
        public static AliasProcessor GetInstance()
        {
            if (Instance == null)
            {
                Instance = new AliasProcessor();
            }
            return Instance;
        }

        Hashtable _aliases = new Hashtable();

        private AliasProcessor()
        {
            LoadAlias();
        }

        private void LoadAlias()
        {
            using (StreamReader sr = new StreamReader(@"Config\Alias.txt", Encoding.Default))
            {
                string alias = string.Empty;
                do
                {
                    alias = sr.ReadLine();
                    if (string.IsNullOrEmpty(alias)) break;
                    try
                    {
                        string[] arrAlias = alias.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!_aliases.ContainsKey(arrAlias[0]))
                            _aliases.Add(arrAlias[0], arrAlias[1]);
                    }
                    catch (Exception) { }
                } while (true);
            }
        }

        public void AddAlias(string alias, string cmd)
        {
            if (_aliases.Contains(alias))
            {
                _aliases.Remove(alias);
            }
            if (!string.IsNullOrEmpty(cmd))
            {
                _aliases.Add(alias, cmd);
            }
        }

        public void SaveAlias()
        {
            lock (GlobalParams.Locker)
            {
                using (StreamWriter sw = new StreamWriter(@"Config\Alias.txt", false, Encoding.UTF8))
                {
                    foreach (string key in _aliases.Keys)
                    {
                        sw.WriteLine(key + "|" + _aliases[key]);
                    }
                }
            }
        }

        public void ReloadAlias()
        {
            _aliases.Clear();
            LoadAlias();
        }

        public string GetFullText(string text)
        {
            if (text == null) return "";
            if (_aliases.ContainsKey(text))
                text = _aliases[text].ToString();
            return text;
        }
    }
}
