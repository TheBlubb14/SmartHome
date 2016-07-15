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

namespace SmartHome
{
    class Program
    {
        private static TelegramBotClient bot;

        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Run();
                    Console.ReadLine();
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
            CW("Server wurde gestartet", CWType.INFO);

            bot.MessageReceived += Bot_MessageReceived;
            bot.StartReceiving();
        }


        private static void Bot_MessageReceived(object sender, MessageEventArgs e)
        {

            UsersRow currentUser = ProcessUser(e);
            string text = e.Message.Text;
            // TODO: Hier Programm Ablauf fortsetzen

            CW(e.Message.Text, CWType.MESSAGE);

            List<string> currentUserStatus = GetStatusFromUser(currentUser);
            StatusRow[] currentUserStatusRow = GetStatusRowFromUser(currentUser);

            StatusRow currentStatus;

            #region Status
            if ((currentStatus = StatusExists(currentUserStatusRow, "s_aus")) != null && Compare(e, "ja"))
            {
                CW("Removing status..", CWType.INFO);
                DeleteStatusFromUser(currentStatus);
                CW("Status removed..", CWType.INFO);

                CW("System geht aus", CWType.WARNING);
                Environment.Exit(0);
            }

            else if ((currentStatus = StatusExists(currentUserStatusRow, "s_licht")) != null && (Compare(e, "an") || Compare(e, "aus")))
            {
                CW("Removing status..", CWType.INFO);
                DeleteStatusFromUser(currentStatus);
                CW("Status removed..", CWType.INFO);

                ObjectsRow currentObject = GetObjectByName("o_licht");

                if (Compare(e, "an"))
                {
                    currentObject.Status = "an";
                }
                else
                {
                    currentObject.Status = "aus";
                }

                UpdateObject(currentObject);

                CW("Licht ist " + currentObject.Status, CWType.INFO);
            }

            #endregion

            // Remove remaining Status, except perma status
            RemoveRemainingStatus(currentUser);

            #region Method
            if (Compare(e, "/aus"))
            {
                bot.SendTextMessageAsync(currentUser.UserID, "herunterfahren ? <ja>");

                InsertNewStatus(currentUser, "s_aus");
            }

            else if (Compare(e, "/licht"))
            {
                ObjectsRow currentObject = GetObjectByName("o_licht");

                if (currentObject != null)
                {
                    bot.SendTextMessageAsync(currentUser.UserID, "Licht  ist aktuell " + currentObject.Status, false, false, 0, GetTelegramKeyBoard(new List<string>() { "an", "aus" }));
                    InsertNewStatus(currentUser, "s_licht");
                }
                else
                {
                    bot.SendTextMessageAsync(currentUser.UserID, "Licht wurde neu erstellt", false, false, 0, GetTelegramKeyBoard(new List<string>() { "an", "aus" }));
                    InsertNewObject("o_licht", "aus");
                    InsertNewStatus(currentUser, "s_licht");
                }
            }

            else if (Compare(e, "/gruppe"))
            {
                bot.SendTextMessageAsync(currentUser.UserID, GetGroupNameFromUser(currentUser));
            }

            else if (Compare(e, "/zeit"))
            {
                bot.SendTextMessageAsync(currentUser.UserID,
                    $"Die aktuelle Zeit: {DateTime.Now.ToShortTimeString()}");
                CW($"Die aktuelle Zeit: {DateTime.Now.ToShortTimeString()}");
            }
            #endregion

        }

        private static StatusRow StatusExists(StatusRow[] currentStatus, string expectedStatus)
        {
            return currentStatus.Where(x => x.Status == expectedStatus).FirstOrDefault();
        }

        private static void RemoveRemainingStatus(UsersRow currentUser)
        {
            StatusRow[] remainingStatus = GetStatusRowFromUser(currentUser);

            foreach (StatusRow status in remainingStatus)
            {
                if (status.Perma)
                    continue;

                DeleteStatusFromUser(status);
            }
        }
    }
}
