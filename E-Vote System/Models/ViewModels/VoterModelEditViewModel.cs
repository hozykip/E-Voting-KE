using E_Vote_System.Helper_Code.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models.ViewModels
{
    public class VoterModelEditViewModel
    {

        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "ID Number")]
        public string IdNumber { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }


        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        [Required]
        [Display(Name = "Voter Category")]
        public int VoterCategoryModelId { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        [NotMapped]
        [Display(Name = "Update Profile Picture")]
        [AllowFileSize(FileSize = 2 * 1024 * 1024, FileExtensions = "jpeg,jpg,png", ErrorMessage = "Please select an image file not bigger than 2MB")]
        public HttpPostedFileBase ProfilePictureUpload { get; set; }

        public VoterCategoryModel VoterCategoryModel { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [NotMapped]
        public string FullName { get { return GetFullName(); } }

        public string GetFullName()
        {
            try
            {
                return this.FirstName + " " + this.LastName;

            }
            catch (Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }

        public static VoterModelEditViewModel fromVoterModel(VoterModel voter)
        {
            try
            {

                string serialized = Newtonsoft.Json.JsonConvert.SerializeObject(voter);

                return Newtonsoft.Json.JsonConvert.DeserializeObject<VoterModelEditViewModel>(serialized);

            }catch(Exception e)
            {
                Utils.LogException(e);
                return null;
            }
        }
    }
}