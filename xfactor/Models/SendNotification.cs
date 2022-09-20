using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace xfactor.Models
{
    public static class NotificaionService
    {
       static readonly string connString = @"Data Source=192.168.49.151;Initial Catalog=Xfactor_prod_web;Persist Security Info=True;User ID=medfactor;Password=xfactor2013;MultipleActiveResultSets=True";
     //   static readonly string connString = @"Data Source=51.210.243.165;Initial Catalog=Xfactor_R;Persist Security Info=True;User ID=xpertiseit;Password=xpertiseit2019;MultipleActiveResultSets=True";
        // static readonly string connString = @"Data Source=51.210.243.165;Initial Catalog=Xfactor_prod_web;Persist Security Info=True;User ID=xpertiseit;Password=xpertiseit2019;MultipleActiveResultSets=True";
        //  string con =
        internal static SqlCommand command = null;
        internal static SqlDependency dependency = null;


        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns></returns>
        public static string GetNotification()
        {
            try
            {

                var messages = new List<T_BORDEREAU>();
                using (var connection = new SqlConnection(connString))
                {

                    connection.Open();
                    //// Sanjay : Alwasys use "dbo" prefix of database to trigger change event
                    using (command = new SqlCommand(@"select [NUM_BORD] FROM [dbo].[T_BORDEREAU] where [DAT_SAISIE_BORD]=@datetody and [EMETTEUR] is not null", connection))
                    {

                    //    '" + DateTime.Today + "'"
                        command.Notification = null;

                        if (dependency == null)
                        {
                            dependency = new SqlDependency(command);
                            dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                        }

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        command.Parameters.Add("@datetody",SqlDbType.DateTime);
                        command.Parameters["@datetody"].Value = DateTime.Today;
                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            messages.Add(item: new T_BORDEREAU
                            {

                                NUM_BORD = (string)reader[0],
                              
                            });
                        }
                    }

                }
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(messages);
                return json;

            }
            catch (Exception ex)
            {

                return null;
            }


        }

        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (dependency != null)
            {
                dependency.OnChange -= dependency_OnChange;
                dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                Notification_Bordereau.Show();
            }
        }

    }
}