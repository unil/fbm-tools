using System;
using System.Diagnostics;

namespace ProgrammeInformatiqueWpf
{
    class ProcessExplorer
    {
        public static string Create(string process, string arguments)
        {
            string res = "Changements appliqués avec succès.";
            Process pr = new Process();

            try
            {
                pr.StartInfo.FileName = process;
                pr.StartInfo.Arguments = arguments;
                pr.StartInfo.UseShellExecute = true;
                pr.StartInfo.Verb = "runas";
                pr.Start();
            }
            catch (Exception ex)
            {
                res = ex.Message;
            }

            return res;
        }
    }
}
