using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ConcurrencyProject.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        [Timestamp]
        public byte[] ChangeCheck { get; set; }
    }
}
