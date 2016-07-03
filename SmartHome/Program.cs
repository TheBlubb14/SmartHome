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
                //try
                //{
                    Run().Wait();
                //}
                //catch (Exception ex)
                //{
                //    CW(ex.ToString(), CWType.WARNING);
                //    Thread.Sleep(1000);
                //}
            }
        }

        private static async Task Run()
        {
            bot = new TelegramBotClient(Properties.Settings.Default["TelegramApiKey"].ToString());
            CW("Server wurde gestartet", CWType.INFO);

            int offset = 0;
            while (true)
            {
                var updates = await bot.GetUpdatesAsync(offset);
                foreach (Update update in updates)
                {
                    ProcessUpdate(update);
                    offset = update.Id + 1;
                }
            }
        }


        private static void ProcessUpdate(Update update)
        {
            UsersRow currentUser = ProcessUser(update);
            string text = update.Message.Text;
            // TODO: Hier Programm Ablauf fortsetzen

            CW(update.Message.Text, CWType.MESSAGE);

            List<string> currentUserStatus = GetStatusFromUser(currentUser);
            StatusRow[] currentUserStatusRow = GetStatusRowFromUser(currentUser);

            StatusRow currentStatus;

            #region Status
            if ((currentStatus = StatusExists(currentUserStatusRow, "s_aus")) != null && Compare(update, "ja"))
            {
                CW("Removing status..", CWType.INFO);
                DeleteStatusFromUser(currentStatus);
                CW("Status removed..", CWType.INFO);

                CW("System geht aus", CWType.WARNING);
                Environment.Exit(0);
            }

            else if ((currentStatus = StatusExists(currentUserStatusRow, "s_licht")) != null && (Compare(update, "an") || Compare(update, "aus")))
            {
                CW("Removing status..", CWType.INFO);
                DeleteStatusFromUser(currentStatus);
                CW("Status removed..", CWType.INFO);

                ObjectsRow currentObject = GetObjectByName("o_licht");

                if (Compare(update, "an"))
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
            if (Compare(update, "/aus"))
            {
                bot.SendTextMessageAsync(currentUser.UserID, "herunterfahren ? <ja>");

                InsertNewStatus(currentUser, "s_aus");
            }

            else if (Compare(update, "/licht"))
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

            else if (Compare(update, "/gruppe"))
            {
                bot.SendTextMessageAsync(currentUser.UserID, GetGroupNameFromUser(currentUser));
            }

            else if (Compare(update, "/zeit"))
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
