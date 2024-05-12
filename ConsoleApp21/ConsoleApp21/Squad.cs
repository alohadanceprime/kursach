using ConsoleApp15;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp21
{
    public class Squad
    {
        public List<astronaut> squad = new List<astronaut>();
        public int squad_compatibility = 0;
        public Squad(List<astronaut> squad) 
        {
            this.squad = squad;
            for (int i = 0; i < squad.Count; i++) 
            {
                for (int j = 0; j < squad[i].astronaut_list.Count; j++)
                {
                    for (int q = 0; q < squad.Count; q++)
                    {
                        if (squad[i].astronaut_list[j][0] == squad[q].astronaut_number)
                        {
                            squad_compatibility += squad[i].astronaut_list[j][1];
                        }
                    }
                }
            }
        }
    }
}
