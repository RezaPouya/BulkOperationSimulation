using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Operator.Models
{
    public sealed class ExcelDb
    {
        [Key]
        public Guid Id { get; set; }    
    }
}
