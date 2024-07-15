using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Contracts;

[Serializable]
public class Customer
{
    public Guid id { get; set; } = new Guid();
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string phoneNumber { get; set; }
    public Guid agentId { get; set; }
    public DateTime connectionTime { get; set; }
}
