using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Folder_Rename_Wpf
{
    public class Traduction
    {
        public Traduction() {}

        public Traduction(string Fr, string En)
        {
            this.Fr = Fr;
            this.En = En;
        }

        public string Fr { get; set; }
        public string En { get; set; }
    }
}
