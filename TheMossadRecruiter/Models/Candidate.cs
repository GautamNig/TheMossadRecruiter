using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMossadRecruiter.Models
{
    public class Candidate
    {
        public string CandidateId { get; set; }
        public string FullName { get; set; }
        public List<Experience> Experience { get; set; }
        // Add other candidate properties as needed
    }
}
