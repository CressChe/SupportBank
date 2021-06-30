using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Member
    {
        public string Name { get; private set; }
        public double Total { get; set; }
        public Member(string name)
        {
            Name = name;
            Total = 0.0;
        }
    }
}
