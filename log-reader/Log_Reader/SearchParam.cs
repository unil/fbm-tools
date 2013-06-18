using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log_Reader
{
    class SearchParam
    {
        public SearchParam(string Dep, string Ord, string Dat, string Scr, string Use, string Heu)
        {
            this.Dep = Dep;
            this.Ord = Ord;
            this.Scr = Scr;
            this.Use = Use;
            this.Dat = Dat;
            this.Heu = Heu;
        }

        public SearchParam(string Dep, string Ord, bool? Res)
        {
            this.Dep = Dep;
            this.Ord = Ord;
            this.Res = Res;
        }

        public SearchParam(string Dep, string Ord, string Dat, string Heu)
        {
            this.Dep = Dep;
            this.Ord = Ord;
            this.Dat = Dat;
            this.Heu = Heu;
        }

        public string Dep { get; set; }
        public string Ord { get; set; }
        public string Scr { get; set; }
        public string Use { get; set; }
        public string Dat { get; set; }
        public string Heu { get; set; }
        public bool? Res { get; set; }
    }
}
