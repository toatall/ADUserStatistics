namespace ADUserStatistics.Models
{
    public class UserPasswordExpired
    {
        private readonly PasswordPolicy _passwordPolicy;
        private readonly string _ldapPath;
        public IEnumerable<AdUser> Results { get; private set; } = new List<AdUser>();

        public UserPasswordExpired(PasswordPolicy passwordPolicy, string ldapPath)
        {
            _passwordPolicy = passwordPolicy;
            _ldapPath = ldapPath;
            Run();
        }

        protected void Run()
        {
            AdSearcher<AdUser> adSearcher = new AdSearcher<AdUser>(_ldapPath);
            string filter = "(&(objectCategory=user)(pwdLastSet<=" + FilterGetDiff() + ")(!pwdLastSet=0)(!userAccountControl:1.2.840.113556.1.4.803:=2))";
            Results = adSearcher.FindAll(filter, AdUserCreator);          
        }

        private AdUser AdUserCreator(dynamic resultSearch, AdSearcher<AdUser> searcher)
        {
            return new AdUser(_passwordPolicy)
            {
                DisplayName = searcher.AsString(resultSearch, "displayName"),
                SAMaccountName = searcher.AsString(resultSearch, "sAMAccountName"),
                PwdLastSet = searcher.AsDate(resultSearch, "pwdLastSet"),
                BadPasswordTime = searcher.AsLong(resultSearch, "badPasswordTime"),
                LockoutTime = searcher.AsLong(resultSearch, "lockoutTime"),
                Title = searcher.AsString(resultSearch, "Title"),
                Department = searcher.AsString(resultSearch, "department"),
            };
        }


        /// <summary>
        /// Расчет времени блокировки учетной записи
        /// </summary>
        /// <returns></returns>
        public long FilterGetDiff()
        {
            long tNow = DateTime.Now.ToFileTimeUtc();
            long tLock = DateTime.FromFileTime(_passwordPolicy.MaxPwdAge.Negate().Ticks).ToFileTimeUtc();
            return tNow - tLock;
        }
        

    }
}
