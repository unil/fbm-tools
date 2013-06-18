using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE
{
    public class User_Test
    {
        public User_Test(string Departement, string Username)
        {
            this.Departement = Departement;
            this.Username = Username;
        }

        public string Departement { get; set; }
        public string Username { get; set; }
    }
}
