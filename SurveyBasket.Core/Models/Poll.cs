using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyBasket.Core.Models
{
    public class Poll
    {
        public int Id { get; set; }
        public string Title {  get; set; }
        public string Description { get; set; } = string.Empty;


    }
}
