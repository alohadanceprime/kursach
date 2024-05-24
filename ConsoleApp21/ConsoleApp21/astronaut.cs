using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp15
{
    public class astronaut
    {
        public int astronaut_number;
        public List<int[]> astronaut_list = new List<int[]>();
        public astronaut(int astronaut_number)
        {
            this.astronaut_number = astronaut_number;
        }
    }
}
