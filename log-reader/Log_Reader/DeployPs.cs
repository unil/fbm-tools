
namespace Log_Reader
{
    public class DeployPs
    {
        public DeployPs()
        {
        }

        public DeployPs(string MachineName, string Date, bool Etat)
        {
            this.MachineName = MachineName;
            this.Date = Date;
            this.Etat = Etat;
        }

        public string MachineName { get; set; }
        public string Date { get; set; }
        public bool Etat { get; set; }
    }
}
