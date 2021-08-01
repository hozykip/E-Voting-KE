using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models
{
    public class VoteModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Voter")]
        public string VoterId { get; set; }
        [Required]
        [Display(Name = "Candidate")]
        public int CandidateId { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        public ApplicationUser Voter { get; set; }
        [ForeignKey("CandidateId")]
        public ElectionCandidateModel Candidate { get; set; }

    }
}