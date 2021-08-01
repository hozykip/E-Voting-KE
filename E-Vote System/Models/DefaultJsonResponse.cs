using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Vote_System.Models
{
    public class DefaultJsonResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}