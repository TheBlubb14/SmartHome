using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using static SmartHome.SmartHomeDatabaseDataSet;
using static SmartHome.Utility;
using static SmartHome.Database;

namespace SmartHome
{
    public static class UserMethods
    {
        private static Dictionary<int, UserMethod> _userMethods;
        private static int userMethodsCount

        {
            get
            {
                return -1;
            }
        }

        public static Dictionary<int, UserMethod> userMethods
        {
            get
            {
                if (_userMethods == null)
                {
                    _userMethods = new Dictionary<int, UserMethod>();
                }

                if (_userMethods.Count != userMethodsCount)
                {
                    List<Func<MethodParameter, MethodParameter>> methods = getAllMethods();

                    _userMethods.Clear();

                    for (int i = 0; i < methods.Count; i++)
                    {
                        // typ aus methodennanmen extrahirenen

                        _userMethods.Add(i, new UserMethod()
                        {
                            method = new Tuple<string, Type, Type, Func<MethodParameter, MethodParameter>>
                            ("gast", typeof(string), typeof(string),
                            methods[i])
                        });
                    }
                }
                return _userMethods;
            }
        }


    }
    public struct UserMethod
    {
        public Tuple<string, Type, Type, Func<MethodParameter, MethodParameter>> method;
    }
    public static class Methods
    {
        public static MethodParameter m_off_object_bool(MethodParameter parameter)
        {
            if (parameter.MessageEventArgs.Message.Text.ToLower() == "yes")
            {
                CW("Removing status..", CWType.INFO);
                DeleteStatusFromUser(GetStatusRowFromUser(parameter.UsersRow).Where(x => x.Status.Remove(0, 2) == "off").FirstOrDefault());
                CW("Status removed..", CWType.INFO);

                CW("System is going off .. byebye world", CWType.WARNING);
                //Environment.Exit(0);
                return parameter;
            }

            return parameter;
        }
        public static MethodParameter m_light_object_string(MethodParameter parameter)
        {
            if (parameter.MessageEventArgs.Message.Text.ToLower() == "on")
            {
                CW("light on");
            }
            else if (parameter.MessageEventArgs.Message.Text.ToLower() == "off")
            {
                CW("light off");
            }
            else
            {
                CW("ignored light");
            }
            return parameter;

        }
    }
}
