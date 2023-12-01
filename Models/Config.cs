namespace ADUserStatistics.Models
{
    public class Config
    {
        public string LdapPath { get; private set; }
        public Config(string ldapPath)
        {
            LdapPath = ldapPath;
        }
    }
}
