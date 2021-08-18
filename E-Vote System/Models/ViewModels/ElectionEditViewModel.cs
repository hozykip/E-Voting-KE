using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models.ViewModels
{
    public class ElectionEditViewModel
    {
        [System.ComponentModel.DataAnnotations.Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Election Type")]
        public int TypeId { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required]
        [Display(Name = "Closing Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        [Required]
        [Display(Name = "Voter Category")]
        public int VoterCategoryId { get; set; }

        public VoterCategoryModel VoterCategory { get; set; }

        public ElectionTypes Type { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser Creator { get; set; }

        public List<ElectionPositionModel> ElectionPositionModels { get; set; }
    }
}