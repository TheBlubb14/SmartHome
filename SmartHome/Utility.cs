using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static SmartHome.SmartHomeDatabaseDataSet;
using static SmartHome.Database;

namespace SmartHome
{
    class Utility
    {
        public static ReplyKeyboardMarkup GetTelegramKeyBoard(List<string> items)
        {
            ReplyKeyboardMarkup Keyboard = new ReplyKeyboardMarkup();
            Keyboard.OneTimeKeyboard = true;
            Keyboard.ResizeKeyboard = true;

            KeyboardButton[][] keyboard = new KeyboardButton[0][];

            for (int i = 0; i < items.Count; i += 2)
            {
                Array.Resize<KeyboardButton[]>(ref keyboard, keyboard.Length + 1);
                keyboard[i] = new KeyboardButton[] { "", "" };

                keyboard[i][0] = items[i];
                keyboard[i][1] = items[i + 1];
            }

            Keyboard.Keyboard = keyboard;

            return Keyboard;
        }

        public static bool Compare(MessageEventArgs arg, string text)
        {
            return arg.Message.Text.ToLower().StartsWith(text);
        }

        public static List<Func<MethodParameter, MethodParameter>> getAllMethodsForUser(UsersRow currentUser)
        {
            List<Func<MethodParameter, MethodParameter>> userMethods = new List<Func<MethodParameter, MethodParameter>>();
            List<Func<MethodParameter, MethodParameter>> existingMethods = getAllMethods();
            List<StatusRow> userStatusList = GetStatusRowFromUser(currentUser);

            foreach (StatusRow row in userStatusList)
            {
                userMethods.Add(existingMethods.Where(x => x.Method.Name.Remove(0, 1).Contains($"_{row.Status}_")).FirstOrDefault());
            }

            return userMethods;
        }

        public static List<Func<MethodParameter, MethodParameter>> getAllMethods()
        {
            List<Func<MethodParameter, MethodParameter>> existingMethods = new List<Func<MethodParameter, MethodParameter>>();
            MethodInfo[] methodInfo = typeof(Methods).GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (MethodInfo info in methodInfo)
            {
                existingMethods.Add((Func<MethodParameter, MethodParameter>)Delegate.CreateDelegate(typeof(Func<MethodParameter, MethodParameter>), info));
            }

            return existingMethods;
        }

        public static void CW(String Text, CWType type = CWType.INFO)
        {
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()} {type.ToString()}]: {Text}");
            Debug.WriteLine($"[{DateTime.Now.ToLongTimeString()} {type.ToString()}]: {Text}");
        }

        public enum CWType
        {
            INFO,
            MESSAGE,
            WARNING
        }
    }

    public class MethodParameter
    {
        public UsersRow UsersRow { get; set; }
        public MessageEventArgs MessageEventArgs { get; set; }
    }
}
