using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace E_Vote_System.Helper_Code.Common
{
    /// <summary>  
    /// Allow file size attribute class  
    /// </summary>  
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class AllowFileSizeAttribute : ValidationAttribute
    {
        #region Public / Protected Properties  

        /// <summary>  
        /// Gets or sets file size property. Default is 1GB (the value is in Bytes).  
        /// </summary>  
        public int FileSize { get; set; } = 1 * 1024 * 1024 * 1024;
        public string FileExtensions { get; set; } = "pdf";

        #endregion

        #region Is valid method  

        /// <summary>  
        /// Is valid method.  
        /// </summary>  
        /// <param name="value">Value parameter</param>  
        /// <returns>Returns - true is specify extension matches.</returns>  
        public override bool IsValid(object value)
        {
            // Initialization  
            HttpPostedFileBase file = value as HttpPostedFileBase;
            bool isValid = true;

            // Settings.  
            int allowedFileSize = this.FileSize;

            // Verification.  
            if (file != null)
            {
                // Initialization.  
                var fileSize = file.ContentLength;

                string extension = Path.GetExtension(file.FileName).Substring(1);

                // Settings.
                List<string> extensionsList = FileExtensions.Split(',').ToList();


                isValid = fileSize <= allowedFileSize && extensionsList.Contains(extension);

                
            }

            // Info  
            return isValid;
        }

        #endregion
    }
}