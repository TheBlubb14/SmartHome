using SmartHome.SmartHomeDatabaseDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using static SmartHome.SmartHomeDatabaseDataSet;
using static SmartHome.Utility;
using static SmartHome.Database;
using System.Reflection;
using System.Runtime.Remoting;

namespace SmartHome
{
    class Program
    {
        private static TelegramBotClient bot;

        // Dic -> MethodenID(int) Tuple -> UserGruppe(string), Typ Parameter 1(type), Typ Parameter 2(type), Func<dynamic, dynamic> 
        Dictionary<int, Tuple<string, Type, Type, Func<MethodParameter, MethodParameter>>> dic = new Dictionary<int, Tuple<string, Type, Type, Func<MethodParameter, MethodParameter>>>();

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Run();
                    bot?.StopReceiving();
                }
                catch (Exception ex)
                {
                    CW(ex.ToString(), CWType.WARNING);
                    Thread.Sleep(1000);
                }
            }
        }

        private static void Run()
        {
            bot = new TelegramBotClient(Properties.Settings.Default["TelegramApiKey"].ToString());
            CW("Server online..", CWType.INFO);

            bot.OnMessage += Bot_OnMessage;
            bot.StartReceiving();
            Console.ReadLine();
        }

        private static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            UsersRow currentUser = ProcessUser(e);
            // TODO: continue program

            CW(e.Message.Text, CWType.MESSAGE);

            #region process userstatus
            List<Func<MethodParameter, MethodParameter>> methods = getAllMethodsForUser(currentUser);

            // call method for each userstatus
            foreach (var method in methods)
            {
                if (method == null)
                    continue;

                MethodParameter result = method(new MethodParameter() { UsersRow = currentUser, MessageEventArgs = e });
            }
            #endregion

            // Remove remaining status, except perma status
            RemoveRemainingStatus(currentUser);

            #region Method
            // TODO: check if retrieved command is in list of commands .. then process them with the user rights

            if (Compare(e, "/off"))
            {
                bot.SendTextMessageAsync(currentUser.UserID, "shutdown system ? <yes>");

                InsertNewStatus(currentUser, "off");
            }
            #endregion
        }

        public static StatusRow StatusExists(List<StatusRow> currentStatus, string expectedStatus)
        {
            return currentStatus.Where(x => x.Status == expectedStatus).FirstOrDefault();

            //IEnumerable<StatusRow> status = currentStatus.Where(x => x.Status == expectedStatus);
            //return status.ToArray<StatusRow>();
        }

        public static void RemoveRemainingStatus(UsersRow currentUser)
        {
            List<StatusRow> remainingStatus = GetStatusRowFromUser(currentUser);

            foreach (StatusRow status in remainingStatus)
            {
                if (status.Perma)
                    continue;

                DeleteStatusFromUser(status);
            }
        }
    }
}
