using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models.ViewModels
{
    public class VoteResultModel
    {
        public int Id { get; set; }
        public string VoterId { get; set; }
        public int CandidateId { get; set; }
        public int PositionId { get; set; }
    }
}