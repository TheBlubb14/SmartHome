using SmartHome.SmartHomeDatabaseDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartHome.SmartHomeDatabaseDataSet;

namespace SmartHome
{
    static class Database
    {
        #region Users
        public static bool CheckUserExisting(String UserID)
        {
            UsersRow currentUser = GetUsers().Where(x => x.UserID == UserID).FirstOrDefault();

            return currentUser != null;
        }

        public static UsersRow ProcessUser(Telegram.Bot.Types.Update update)
        {
            if (!CheckUserExisting(update.Message.Chat.Id.ToString()))
            {
                InsertNewUser
                    (
                        update.Message.Chat.Id.ToString(),
                        update.Message.Chat.FirstName,
                        update.Message.Chat.LastName,
                        DateTime.UtcNow.ToString()
                    );
                Utility.CW($"Added new user with id ({update.Message.Chat.Id})");
            }

            return GetUsers().Where(x => x.UserID == update.Message.Chat.Id.ToString()).FirstOrDefault();
        }

        public static int InsertNewUser(String UserID, String FirstName, String LastName, String FirstJoined, int Group = 0)
        {
            try
            {
                UsersTableAdapter usersAdapter = new UsersTableAdapter();
                usersAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                usersAdapter.Connection.Open();
                int status = usersAdapter.Insert(UserID, FirstName, LastName, FirstJoined, Group);
                usersAdapter.Connection.Close();

                return status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static UsersDataTable GetUsers()
        {
            try
            {
                UsersTableAdapter usersAdapter = new UsersTableAdapter();
                usersAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                usersAdapter.Connection.Open();
                UsersDataTable usersDataTable = usersAdapter.GetData();
                usersAdapter.Connection.Close();

                return usersDataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Group
        public static string GetGroupNameFromUser(UsersRow currentUser)
        {
            return GetGroups().Where(x => x.GroupID == currentUser.Group).FirstOrDefault().Name;
        }

        public static GroupsDataTable GetGroups()
        {
            try
            {
                GroupsTableAdapter groupsAdapter = new GroupsTableAdapter();
                groupsAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                groupsAdapter.Connection.Open();
                GroupsDataTable groupsDataTable = groupsAdapter.GetData();
                groupsAdapter.Connection.Close();

                return groupsDataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Status
        public static StatusDataTable GetStatus()
        {
            try
            {
                StatusTableAdapter statusTableAdapter = new StatusTableAdapter();
                statusTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                statusTableAdapter.Connection.Open();
                StatusDataTable statusDataTable = statusTableAdapter.GetData();
                statusTableAdapter.Connection.Close();

                return statusDataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<string> GetStatusFromUser(UsersRow currentUser)
        {
            StatusRow[] statusRow = GetStatus().Where(x => x.UserID == currentUser.UserID).ToArray();

            List<string> status = new List<string>();
            foreach (StatusRow row in statusRow)
            {
                status.Add(row.Status);
            }

            return status;
        }

        public static StatusRow[] GetStatusRowFromUser(UsersRow currentUser)
        {
            return GetStatus().Where(x => x.UserID == currentUser.UserID).ToArray();
        }

        public static int InsertNewStatus(UsersRow currentUser, string status, bool perma = false)
        {
            try
            {
                StatusTableAdapter statusTableAdapter = new StatusTableAdapter();
                statusTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                statusTableAdapter.Connection.Open();
                int _status = statusTableAdapter.Insert(currentUser.UserID, status, perma);
                statusTableAdapter.Connection.Close();

                return _status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int DeleteStatusFromUser(StatusRow currentStatus)
        {
            try
            {
                StatusTableAdapter statusTableAdapter = new StatusTableAdapter();
                statusTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                statusTableAdapter.Connection.Open();
                int status = statusTableAdapter.Delete(currentStatus.StatusID, currentStatus.UserID, currentStatus.Perma);
                statusTableAdapter.Connection.Close();

                return status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int UpdateStatusFromUser(StatusRow currentStatus)
        {
            try
            {
                StatusTableAdapter statusTableAdapter = new StatusTableAdapter();
                statusTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                statusTableAdapter.Connection.Open();
                int status = statusTableAdapter.Update(currentStatus);
                statusTableAdapter.Connection.Close();

                return status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region Objects
        public static ObjectsDataTable GetObjects()
        {
            try
            {
                ObjectsTableAdapter objectsTableAdapter = new ObjectsTableAdapter();
                objectsTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                objectsTableAdapter.Connection.Open();
                ObjectsDataTable objectsDataTable = objectsTableAdapter.GetData();
                objectsTableAdapter.Connection.Close();

                return objectsDataTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ObjectsRow GetObjectByName(string objectName)
        {
            return GetObjects().Where(x => x.Name == objectName).FirstOrDefault();
        }

        public static int InsertNewObject(string name, string objectStatus)
        {
            try
            {
                ObjectsTableAdapter objectsTableAdapter = new ObjectsTableAdapter();
                objectsTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                objectsTableAdapter.Connection.Open();
                int status = objectsTableAdapter.Insert(name, objectStatus);
                objectsTableAdapter.Connection.Close();

                return status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int DeleteObject(ObjectsRow currentObject)
        {
            try
            {
                ObjectsTableAdapter objectsTableAdapter = new ObjectsTableAdapter();
                objectsTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                objectsTableAdapter.Connection.Open();
                int status = objectsTableAdapter.Delete(currentObject.ObjectID);
                objectsTableAdapter.Connection.Close();

                return status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int UpdateObject(ObjectsRow currentObject)
        {
            try
            {
                ObjectsTableAdapter objectsTableAdapter = new ObjectsTableAdapter();
                objectsTableAdapter.Connection.ConnectionString = Properties.Settings.Default["SmartHomeDatabaseConnectionString"].ToString();
                objectsTableAdapter.Connection.Open();
                int status = objectsTableAdapter.Update(currentObject);
                objectsTableAdapter.Connection.Close();

                return status;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion
    }
}


