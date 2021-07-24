using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace E_Vote_System
{
    public class Configs
    {

        public static readonly string LogsPath = ConfigurationManager.AppSettings["LogsPath"];
        public static readonly bool WRITE_DEBUG_LOG = Convert.ToBoolean(ConfigurationManager.AppSettings["WRITE_DEBUG_LOG"]);
        public static readonly string ToastSeparator = ConfigurationManager.AppSettings["ToastSeparator"];
        public static readonly string DefaultErrorMessage = ConfigurationManager.AppSettings["DefaultErrorMessage"];
        public static readonly string CompanyName = ConfigurationManager.AppSettings["CompanyName"];
        public static readonly string ProfileUrl = ConfigurationManager.AppSettings["ProfileUrl"];
        public static readonly string ProfilePath = ConfigurationManager.AppSettings["ProfilePath"];

        public static readonly string SendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
        public static readonly string SendGridEmail = ConfigurationManager.AppSettings["SendGridEmail"];
        public static readonly string SendGridDisplayName = ConfigurationManager.AppSettings["SendGridDisplayName"];


        //public static readonly string AdminUser_UserName = ConfigurationManager.AppSettings["AdminUserName"];
        public static readonly string AdminUserEmail = ConfigurationManager.AppSettings["AdminUserEmail"];
        public static readonly string AdminUserFirstName = ConfigurationManager.AppSettings["AdminUserFirstName"];
        public static readonly string AdminUserLastName = ConfigurationManager.AppSettings["AdminUserLastName"];
        public static readonly string AdminUserPhoneNumber = ConfigurationManager.AppSettings["AdminUserPhoneNumber"];
        public static readonly string AdminUserPassword = ConfigurationManager.AppSettings["AdminUserPassword"];
        public static readonly string AdminUserAddress = ConfigurationManager.AppSettings["AdminUserAddress"];

    }
}