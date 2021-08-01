using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models.ViewModels
{
    public class ElectionTypesViewModels
    {
    }

    public class ElectionTypesEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Date Modified")]
        public DateTime? DateModified { get; set; }

        public string Description { get; set; }

        public List<ElectionModel> ElectionModels { get; set; }
    }

}