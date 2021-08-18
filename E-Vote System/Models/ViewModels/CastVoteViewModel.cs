using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models.ViewModels
{
    public class CastVoteViewModel
    {
        [Required]
        public int ElectionId { get; set; }
        [Required]
        public int PositionId { get; set; }
        [Required]
        public int CandidateId { get; set; }
    }
}