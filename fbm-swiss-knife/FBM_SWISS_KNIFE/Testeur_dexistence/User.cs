﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE.Testeur_dexistence
{
    class User
    {
        public User(string Departement, string Username)
        {
            this.Departement = Departement;
            this.Username = Username;
        }

        public string Departement { get; set; }
        public string Username { get; set; }
    }
}
