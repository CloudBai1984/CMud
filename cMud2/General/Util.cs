using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.ApplicationBlocks.Data;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class Util
    {
        public static void Wait(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        public static DataSet GetPathDataSet(string area, string address, int number)
        {
            DataSet ds = null;
            try
            {
                string sql = string.Format("SELECT * FROM Path NOLOCK WHERE Name = '{0}' and Number = {1} and area='{2}'", address, number, area);
                ds = SqlHelper.ExecuteDataset(GlobalParams.ConnnectionString, CommandType.Text, sql);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message + "\r\n" + e.StackTrace);
            }
            return ds;
        }

        #region Process Arithmetical Expression
        public static double ParseArithmeticalExpression(string infixExp)
        {
            infixExp = infixExp.Replace(" ", "");
            infixExp = Regex.Replace(infixExp, @"^-\d+|[^\d]-\d+", new MatchEvaluator(new Func<Match, string>(m =>
                {
                    if (m.Value.StartsWith("-"))
                    {
                        return "(0" + m.Value + ")";
                    }
                    else
                    {
                        return m.Value.First() + "(0" + m.Value.Remove(0, 1) + ")";
                    }
                })));

            double result = 0;
          
            try
            {
                List<string> postfixExp = ChangeToPostfixFromInfixArithmetical(infixExp);
                  Stack<string> stkTmp = new Stack<string>();
                for (int i = 0; i < postfixExp.Count; i++)
                {
                    string str = postfixExp[i];
                    if (!IsArithmeticalOperator(str.ToCharArray()[0]))
                    {
                        stkTmp.Push(str);
                    }
                    else
                    {

                        double second = double.Parse(stkTmp.Pop());
                        double first = double.Parse(stkTmp.Pop());
                        switch (str)
                        {
                            case "+":
                                result = first + second;
                                break;
                            case "-":
                                result = first - second;
                                break;
                            case "*":
                                result = first * second;
                                break;
                            case "/":
                                result = first / second;
                                break;
                        }
                        stkTmp.Push(result.ToString());
                    }
                }
                result = double.Parse(stkTmp.Pop());
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        private static List<string> ChangeToPostfixFromInfixArithmetical(string infixExp)
        {
            List<string> postfixExp = new List<string>();
            Stack<char> stkTmp = new Stack<char>();
            char[] charArray = infixExp.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                char c = charArray[i];
                if (char.IsDigit(c))
                {
                    StringBuilder tmp = new StringBuilder();
                    tmp.Append(c);
                    do
                    {
                        if (i < charArray.Length - 1 && char.IsDigit(charArray[i + 1]))
                        {
                            i++;
                            tmp.Append(charArray[i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true);
                    postfixExp.Add(tmp.ToString());
                }
                else if (IsArithmeticalOperator(c))
                {
                    if (stkTmp.Count > 0)
                    {
                        char charTop = stkTmp.Peek();
                        while (true)
                        {
                            if (IsHighArithmeticPriority(charTop, c))
                            {
                                postfixExp.Add(stkTmp.Pop().ToString());
                            }
                            else
                            {
                                break;
                            }
                            if (stkTmp.Count > 0)
                            {
                                charTop = stkTmp.Peek();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    stkTmp.Push(c);
                }
                else if (c == '(')
                {
                    stkTmp.Push(c);
                }
                else if (c == ')')
                {
                    do
                    {
                        char charTop = stkTmp.Pop();
                        if (charTop != '(')
                        {
                            postfixExp.Add(charTop.ToString());
                        }
                        else
                        {
                            break;
                        }
                    } while (true);
                }
            }
            while (stkTmp.Count > 0)
            {
                postfixExp.Add(stkTmp.Pop().ToString());
            }
            return postfixExp;
        }
        private static bool IsArithmeticalOperator(char c)
        {
            if ((c == '+') || (c == '-') || (c == '*') || (c == '/'))
                return true;
            return false;
        }
        private static bool IsHighArithmeticPriority(char oper1, char oper2)
        {
            return GetArithmeticPriority(oper1) >= GetArithmeticPriority(oper2);
        }
        private static int GetArithmeticPriority(char oper)
        {
            int priority = -1;
            switch (oper)
            {
                case '*':
                    priority = 1;
                    break;
                case '/':
                    priority = 1;
                    break;
                case '+':
                    priority = 0;
                    break;
                case '-':
                    priority = 0;
                    break;
            }
            return priority;
        }
        #endregion

        #region Process Boolean Expression
        public static bool ParseBooleanExpression(string infixExp)
        {
            infixExp = infixExp.Replace(" ", "");
            infixExp = Regex.Replace(infixExp, @"^-\d+|[^\d]-\d+", new MatchEvaluator(new Func<Match, string>(m =>
            {
                if (m.Value.StartsWith("-"))
                {
                    return "(0" + m.Value + ")";
                }
                else
                {
                    return m.Value.First() + "(0" + m.Value.Remove(0, 1) + ")";
                }
            })));


            bool result = false;
            try
            {
                List<string> postfixExp = ChangeToPostfixFromInfixBoolean(infixExp);
                Stack<string> stkTmp = new Stack<string>();
                for (int i = 0; i < postfixExp.Count; i++)
                {
                    string str = postfixExp[i];
                    if (!IsBooleanOperator(str))
                    {
                        stkTmp.Push(str);
                    }
                    else
                    {
                        if (!str.Equals("!"))
                        {
                            string second = stkTmp.Pop();
                            string first = stkTmp.Pop();
                            double tmp = 0;
                            switch (str)
                            {
                                case "/":
                                    tmp = double.Parse(first) / double.Parse(second);
                                    stkTmp.Push(tmp.ToString());
                                    break;
                                case "*":
                                    tmp = double.Parse(first) * double.Parse(second);
                                    stkTmp.Push(tmp.ToString());
                                    break;
                                case "-":
                                    tmp = double.Parse(first) - double.Parse(second);
                                    stkTmp.Push(tmp.ToString());
                                    break;
                                case "+":
                                    tmp = double.Parse(first) + double.Parse(second);
                                    stkTmp.Push(tmp.ToString());
                                    break;
                                case "||":
                                    result = bool.Parse(first) || bool.Parse(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case "&&":
                                    result = bool.Parse(first) && bool.Parse(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case ">":
                                    result = double.Parse(first) > double.Parse(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case ">=":
                                    result = double.Parse(first) >= double.Parse(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case "==":
                                    result = first.Equals(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case "<":
                                    result = double.Parse(first) < double.Parse(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case "<=":
                                    result = double.Parse(first) <= double.Parse(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                                case "!=":
                                    result = !first.Equals(second);
                                    stkTmp.Push(result.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            string first = stkTmp.Pop();
                            result = !bool.Parse(first);
                            stkTmp.Push(result.ToString());
                        }

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
        public static List<string> ChangeToPostfixFromInfixBoolean(string infixExp)
        {
            List<string> postfixExp = new List<string>();
            Stack<string> stkTmp = new Stack<string>();
            char[] charArray = infixExp.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                char c = charArray[i];
                if (!IsBooleanOperator(c) && c != '(' && c != ')')
                {
                    StringBuilder tmp = new StringBuilder();
                    tmp.Append(c);
                    do
                    {
                        if (i < charArray.Length - 1
                            && !IsBooleanOperator(charArray[i + 1])
                            && charArray[i + 1] != '('
                            && charArray[i + 1] != ')')
                        {
                            i++;
                            tmp.Append(charArray[i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                    while (true);
                    postfixExp.Add(tmp.ToString());
                }
                else if (IsBooleanOperator(c))
                {
                    StringBuilder tmp = new StringBuilder();
                    tmp.Append(c);
                    if (c == '!' || c == '>' || c == '<')
                    {
                        if (charArray[i + 1] == '=')
                        {
                            i++;
                            tmp.Append(charArray[i]);
                        }
                    }
                    else if (c == '=' || c == '|' || c == '&')
                    {
                        i++;
                        tmp.Append(charArray[i]);
                    }

                    string currentOper = tmp.ToString();

                    if (stkTmp.Count > 0)
                    {
                        string topOper = stkTmp.Peek();
                        while (true)
                        {
                            if (IsHighBooleanPriority(topOper, currentOper))
                            {
                                postfixExp.Add(stkTmp.Pop().ToString());
                            }
                            else
                            {
                                break;
                            }
                            if (stkTmp.Count > 0)
                            {
                                topOper = stkTmp.Peek();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    stkTmp.Push(currentOper);
                }
                else if (c == '(')
                {
                    stkTmp.Push(c.ToString());
                }
                else if (c == ')')
                {
                    do
                    {
                        string top = stkTmp.Pop();
                        if (!top.Equals("("))
                        {
                            postfixExp.Add(top);
                        }
                        else
                        {
                            break;
                        }
                    } while (true);
                }
            }
            while (stkTmp.Count > 0)
            {
                postfixExp.Add(stkTmp.Pop());
            }
            return postfixExp;
        }
        private static bool IsBooleanOperator(char c)
        {
            if ((c == '!') || (c == '>') || (c == '<') || (c == '=') || (c == '|')
                || (c == '&') || c == '+' || c == '-' || c == '*' || c == '/')
                return true;
            return false;
        }
        private static bool IsBooleanOperator(string oper)
        {
            if (oper.Equals(">") || oper.Equals(">=") || oper.Equals("<") || oper.Equals("<=") || oper.Equals("==") ||
                oper.Equals("!=") || oper.Equals("!") || oper.Equals("||") || oper.Equals("&&") || oper.Equals("+") ||
                oper.Equals("-") || oper.Equals("*") || oper.Equals("/"))
                return true;
            return false;
        }
        private static int GetBooleanOperatorPriority(string oper)
        {
            int priority = -1;
            switch (oper)
            {
                case "*":
                    priority = 5;
                    break;
                case "/":
                    priority = 5;
                    break;
                case "+":
                    priority = 4;
                    break;
                case "-":
                    priority = 4;
                    break;
                case ">":
                    priority = 3;
                    break;
                case "<":
                    priority = 3;
                    break;
                case ">=":
                    priority = 3;
                    break;
                case "<=":
                    priority = 3;
                    break;
                case "==":
                    priority = 3;
                    break;
                case "!=":
                    priority = 3;
                    break;
                case "!":
                    priority = 2;
                    break;
                case "||":
                    priority = 0;
                    break;
                case "&&":
                    priority = 1;
                    break;
            }
            return priority;
        }
        private static bool IsHighBooleanPriority(string oper1, string oper2)
        {
            return GetBooleanOperatorPriority(oper1) >= GetBooleanOperatorPriority(oper2);
        }
        #endregion

    }
}
