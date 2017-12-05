using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSOI_Data.Entities
{
    public class Operation
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public OperationType OperationType { get; set; }
        public long Sum { get; set; }
    }
}
