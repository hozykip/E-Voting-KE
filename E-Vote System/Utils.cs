using E_Vote_System.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace E_Vote_System
{
    public class Utils
    {



        private static ApplicationDbContext dc = new ApplicationDbContext();
        private static string ToastSeparator = Configs.ToastSeparator;

        public static ApplicationUser GetCurrentUser()
        {
            using (ApplicationDbContext applicationDbContext = new ApplicationDbContext())
                return applicationDbContext.Users.Where<ApplicationUser>((Expression<Func<ApplicationUser, bool>>)(x => x.UserName == HttpContext.Current.User.Identity.Name)).FirstOrDefault<ApplicationUser>();
        }

        
        public static VoterModel GetCurrentVoter()
        {
            try
            {

                ApplicationUser user = GetCurrentUser();

                if (user == null) return null;

                using(ApplicationDbContext dbContext = new ApplicationDbContext())
                {
                    VoterModel voterModel = dbContext.VoterModels.FirstOrDefault(y => y.UserId == user.Id);

                    return voterModel;
                }

            }catch(Exception e)
            {
                Utils.LogException(e);
            }
            return null;
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethod()
        {
            string methodName = "-";
            try
            {
                var st = new StackTrace();
                var sf = st.GetFrame(2);

                methodName = sf.GetMethod().Name;
                var method = sf.GetMethod();



                var fullName = method.DeclaringType.FullName + " - " + methodName;

                return fullName;

            }
            catch (Exception e)
            {

            }
            //methodName = "";
            return methodName;
        }


        public static void LogException(Exception e, string error_message = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            try
            {

                DateTime date = DateTime.Now;

                string today = date.ToString("dd-MM-yyyy");
                string fileName = $"{today} Exception Logs.txt";

                string filePath = Configs.LogsPath + fileName;
                string message = $"{date.ToLongTimeString()} -   Portal  -  {GetCurrentMethod()} [{caller}:{lineNumber}] -- {e.Message} -- custom: {error_message}";
                if (string.IsNullOrWhiteSpace(error_message))
                {
                    message = $"{date.ToLongTimeString()} -   Portal  -  {GetCurrentMethod()} [{caller}:{lineNumber}] -- {e.Message}";
                }
                string[] conttent = { message };

                if (!System.IO.Directory.Exists(Configs.LogsPath))
                {
                    System.IO.Directory.CreateDirectory(Configs.LogsPath);
                }

                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.WriteAllLines(filePath, conttent, Encoding.UTF8);
                }
                else
                {

                    using (StreamWriter file = new StreamWriter(filePath, append: true))
                    {
                        file.WriteLine(message);
                    }

                }

            }
            catch (Exception ex)
            {
                //Umezidi
            }
        }

        public static void LogErrors(string error_message,[CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {

            if (string.IsNullOrWhiteSpace(error_message))
            {
                return;
            }

            try
            {

                DateTime date = DateTime.Now;

                string today = date.ToString("dd-MM-yyyy");
                string fileName = $"{today} Error Logs.txt";

                string filePath = Configs.LogsPath + fileName;
                string message = $"{date.ToLongTimeString()} -   Portal  -  {GetCurrentMethod()} [{caller}:{lineNumber}] -- {error_message}";
                
                string[] conttent = { message };

                if (!System.IO.Directory.Exists(Configs.LogsPath))
                {
                    System.IO.Directory.CreateDirectory(Configs.LogsPath);
                }

                if (!System.IO.File.Exists(filePath))
                {
                    System.IO.File.WriteAllLines(filePath, conttent, Encoding.UTF8);
                }
                else
                {

                    using (StreamWriter file = new StreamWriter(filePath, append: true))
                    {
                        file.WriteLine(message);
                    }

                }

            }
            catch (Exception ex)
            {
                //Umezidi
            }
        }


        public static void LogDebug(dynamic e, string custom_message = "", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {

            bool write_log = Configs.WRITE_DEBUG_LOG;

            if (write_log)
            {
                try
                {

                    DateTime date = DateTime.Now;

                    string today = date.ToString("dd-MM-yyyy");
                    string fileName = $"{today} Debug Logs.txt";

                    string filePath = Configs.LogsPath + fileName;
                    string message = $"{date.ToLongTimeString()} -   Portal  -  {GetCurrentMethod()} [{caller}:{lineNumber}] -- {Newtonsoft.Json.JsonConvert.SerializeObject(e)} -- custom: {custom_message}";

                    if (string.IsNullOrWhiteSpace(custom_message))
                    {
                        message = $"{date.ToLongTimeString()} -   Portal  -  {GetCurrentMethod()} [{caller}:{lineNumber}] -- {Newtonsoft.Json.JsonConvert.SerializeObject(e)}";
                    }

                    string[] conttent = { message };

                    if (!System.IO.Directory.Exists(Configs.LogsPath))
                    {
                        System.IO.Directory.CreateDirectory(Configs.LogsPath);
                    }

                    if (!System.IO.File.Exists(filePath))
                    {
                        System.IO.File.WriteAllLines(filePath, conttent, Encoding.UTF8);
                    }
                    else
                    {

                        using (StreamWriter file = new StreamWriter(filePath, append: true))
                        {
                            file.WriteLine(message);
                        }

                        //using StreamWriter file = new(filePath, append: true);

                    }

                }
                catch (Exception ex)
                {
                    //Utils.LogException(ex, "Debug log failed");
                }
            }
        }

        private static string cleanToastMessage(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return input.Replace(ToastSeparator, " ");
        }


        public static string GenerateToastSuccess(string message, string title = "Success")
        {
            try
            {
                string finalMessage = $"success{ToastSeparator}{cleanToastMessage(message)}{ToastSeparator}{title}";
                return finalMessage;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return message;
            }
        }

        public static string GenerateToastInfo(string message, string title = "Message")
        {
            try
            {
                string finalMessage = $"info{ToastSeparator}{cleanToastMessage(message)}{ToastSeparator}{title}";
                return finalMessage;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return message;
            }
        }

        public static string GenerateToastError(string message = null, string title = "Error")
        {
            try
            {

                if (string.IsNullOrEmpty(message)) message = Configs.DefaultErrorMessage;

                string finalMessage = $"error{ToastSeparator}{cleanToastMessage(message)}{ToastSeparator}{title}";
                return finalMessage;
            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return message;
            }
        }


        public static string FormatDisplayDateTimeLocal(DateTime? date)
        {
            string output = "";

            if (!date.HasValue) return output;

            output = formatDisplayDateTimeLocal(date.Value);

            return output;
        }

        public static string FormatDate(string date)
        {

            try
            {
                return Convert.ToDateTime(date).ToString("dd MMM, yyyy hh:mm tt");
            }
            catch (Exception e)
            {
                return date;
            }


        }

        public static string FormatDate(DateTime? date)
        {

            try
            {
                if (!date.HasValue)
                {
                    return "";
                }

                return date.Value.ToString("dd MMM, yyyy hh:mm tt");
            }
            catch (Exception e)
            {
                return "";
            }

        }

        private static string formatDisplayDateTimeLocal(DateTime date)
        {
            string output = "";
                       

            try
            {

                output = date.ToString("yyyy-MM-ddTHH:mm");

            }catch(Exception e)
            {
                Utils.LogException(e);
            }

            return output;
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static bool CreateFolderIfNotExists(string path)
        {
            bool flag = false;

            try
            {

                if (System.IO.Directory.Exists(path))
                {
                    return true;
                }

                DirectoryInfo directoryInfo = System.IO.Directory.CreateDirectory(path);

                return directoryInfo.Exists;

            }catch(Exception e)
            {
                LogException(e);
            }

            return flag;
        }
    }
}