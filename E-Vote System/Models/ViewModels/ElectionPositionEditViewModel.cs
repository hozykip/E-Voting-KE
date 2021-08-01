using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models.ViewModels
{
    public class ElectionPositionEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Election")]
        public int ElectionId { get; set; }
        [Required]
        public string Position { get; set; }


        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }


        public ElectionModel Election { get; set; }

        public List<ElectionCandidateModel> ElectionCandidateModels { get; set; }
    }
}