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
    public static class Methods
    {
        public static dynamic m_off(dynamic parameter)
        {
            UsersRow currentUser = parameter[0] as UsersRow;
            MessageEventArgs args = parameter[1] as MessageEventArgs;

            if (args.Message.Text.ToLower() == "yes")
            {
                CW("Removing status..", CWType.INFO);
                DeleteStatusFromUser(GetStatusRowFromUser(currentUser).Where(x => x.Status.Remove(0, 2) == "off").FirstOrDefault());
                CW("Status removed..", CWType.INFO);

                CW("System is going off .. byebye world", CWType.WARNING);
                //Environment.Exit(0);
                return parameter;
            }

            return parameter;
        }
        public static dynamic m_light(dynamic parameter)
        {
            UsersRow currentUser = parameter[0] as UsersRow;
            MessageEventArgs args = parameter[1] as MessageEventArgs;

            if (args.Message.Text.ToLower() == "on")
            {
                CW("light on");
            }
            else if (args.Message.Text.ToLower() == "off")
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
