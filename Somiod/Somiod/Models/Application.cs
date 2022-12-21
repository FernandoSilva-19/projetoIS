using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Somiod.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string Res_type { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}