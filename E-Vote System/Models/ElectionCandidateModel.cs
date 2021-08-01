using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models
{
    public class ElectionCandidateModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Position")]
        public int PositionId { get; set; }
        [Required]
        [Display(Name = "Surname")]
        public string SurName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Other Name")]
        public string OtherName { get; set; }
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }
        [Display(Name = "Manifesto")]
        public string ManifestoFile { get; set; }
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }


        [ForeignKey("PositionId")]
        public ElectionPositionModel ElectionPositionModel { get; set; }

        public List<VoteModel> Votes { get; set; }
    }
}