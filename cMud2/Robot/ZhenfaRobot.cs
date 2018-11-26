using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace cMud2
{
    public class ZhenfaRobot : Robot
    {
        private static ZhenfaRobot Instance;

        public static ZhenfaRobot GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ZhenfaRobot();
            }
            return Instance;
        }

        private ZhenfaRobot()
            : base()
        {
            Name = "阵法";
        }
       
        bool _flagZhenfa;
        string _strPosition;


        


        private void DoStep1()
        {
           
            _comm.SendText("southwest;#wa 500;zhenfa");
            _stepNumber = 1;
           
        }

        private void DoStep2(Regex regex)
        {
          
           
            string position = regex.Match(_currentMessage).Groups["position"].Value;
            switch (position)
            {
                case "金":
                    _comm.SendText("#wa 1000;zouwei 金");
                    _strPosition = "金";
                    break;
                case "木":
                    _comm.SendText("#wa 1000;zouwei 木");
                    _strPosition = "木";
                    break;
                case "水":
                    _comm.SendText("#wa 1000;zouwei 水");
                    _strPosition = "水";
                    break;
                case "火":
                    _comm.SendText("#wa 1000;zouwei 火");
                    _strPosition = "火";
                    break;
                case "土":
                    _comm.SendText("#wa 1000;zouwei 土");
                    _strPosition = "土";
                    break;
            }
            _stepNumber = 2;
            
        }

        private void DoStep3(Regex regex,int status)
        {
          
            switch (status)
            {
                case 1:
                    if (_flagZhenfa)
                    {
                        switch (_strPosition)
                        {
                            case "金":
                                _comm.SendText("#wa 1000;zouwei 水");
                                _strPosition = "水";
                                break;
                            case "木":
                                _comm.SendText("#wa 1000;zouwei 火");
                                _strPosition = "火";
                                break;
                            case "水":
                                _comm.SendText("#wa 1000;zouwei 木");
                                _strPosition = "木";
                                break;
                            case "火":
                                _comm.SendText("#wa 1000;zouwei 土");
                                _strPosition = "土";
                                break;
                            case "土":
                                _comm.SendText("#wa 1000;zouwei 金");
                                _strPosition = "金";
                                break;
                        }
                    }
                    else
                    {
                        switch (_strPosition)
                        {
                            case "金":
                                _comm.SendText("#wa 1000;zouwei 木");
                                _strPosition = "木";
                                break;
                            case "木":
                                _comm.SendText("#wa 1000;zouwei 土");
                                _strPosition = "土";
                                break;
                            case "水":
                                _comm.SendText("#wa 1000;zouwei 火");
                                _strPosition = "火";
                                break;
                            case "火":
                                _comm.SendText("#wa 1000;zouwei 金");
                                _strPosition = "金";
                                break;
                            case "土":
                                _comm.SendText("#wa 1000;zouwei 水");
                                _strPosition = "水";
                                break;
                        }
                    }
                    break;
                case 2:
                   
                    _comm.SendText("#wa 1000;ne;wc;#wa 500;");
                    Stop();
                    break;
            }
        }


        public override void Stop()
        {
            base.Stop();
            _stepNumber = 0;
        }

        public override void Run()
        {
            base.Run();
          
            DoStep1();
        }

        protected override void GoNext()
        {
            switch (_stepNumber)
            {
                case 1:
                    _regex = new Regex(@"阵法教习.*反五行", RegexOptions.Multiline);
                    if (_regex.IsMatch(_currentMessage))
                    {
                        _flagZhenfa = false;
                    }
                    _regex = new Regex(@"阵法教习.*正五行", RegexOptions.Multiline);
                    if (_regex.IsMatch(_currentMessage))
                    {
                        _flagZhenfa = true;
                    }
                    _regex = new Regex(@"云自遥一会儿站在.*(?<position>[水火金木土]).*$", RegexOptions.Multiline);
                    if (_regex.IsMatch(_currentMessage))
                    {
                        DoStep2(_regex);
                    }
                    break;
                case 2:
                    _regex = new Regex(@"阵法教习.*喊道.*『.*反五行阵.*", RegexOptions.Multiline);
                    if (_regex.IsMatch(_currentMessage))
                    {
                        _flagZhenfa = false;
                        DoStep3(_regex, 1);
                    }
                    _regex = new Regex(@"阵法教习.*喊道.*『.*正五行阵.*", RegexOptions.Multiline);
                     if (_regex.IsMatch(_currentMessage))
                     {
                         _flagZhenfa = true;
                         DoStep3(_regex, 1);
                     }

                     _regex = new Regex(@"阵法教习挥了挥旗喊道：五行阵演练到此结束", RegexOptions.Multiline);
                     if (_regex.IsMatch(_currentMessage))
                     {
                         DoStep3(_regex, 2);
                     }
                     _regex = new Regex(@"你站错了方位，整个五行阵全都乱套了", RegexOptions.Multiline);
                     if (_regex.IsMatch(_currentMessage))
                     {
                         _comm.SendText("#wa 1000;ne;qx;#wa 2000;rw");
                         Stop();
                     }
                    break;
                case 3:

                    break;
                default:
                    break;
            }
        }

      
    }
}
