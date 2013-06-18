using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE.Createur_de_dossier
{
    class Folder
    {
        public string Name { get; set; }
        public bool CreateContent { get; set; }
        public string State { get; set; }
        public string StatePerso { get; set; }
        public string StatePublic { get; set; }
        public string StatePrivate { get; set; }
    }
}
