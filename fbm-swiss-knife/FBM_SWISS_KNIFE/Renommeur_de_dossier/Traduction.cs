using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE.Renommeur_de_dossier
{
    public class Traduction
    {
        public Traduction() { }

        public Traduction(string Fr, string En)
        {
            this.Fr = Fr;
            this.En = En;
        }

        public string Fr { get; set; }
        public string En { get; set; }
    }
}
