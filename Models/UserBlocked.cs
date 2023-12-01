namespace ADUserStatistics.Models
{
    public class UserBlocked
    {
        private readonly PasswordPolicy _passwordPolicy;
        private readonly string _ldapPath;
        public IEnumerable<AdUser> Results { get; private set; } = new List<AdUser>();

        public UserBlocked(PasswordPolicy passwordPolicy, string ldapPath)
        {
            _passwordPolicy = passwordPolicy;
            _ldapPath = ldapPath;
            Run();
        }

        /// <summary>
        /// Расчет времени блокировки учетной записи
        /// </summary>
        /// <returns></returns>
        private long FilterGetDiff()
        {
            long tNow = DateTime.Now.ToFileTimeUtc();
            long tLock = DateTime.FromFileTime(_passwordPolicy.LockoutDuration.Negate().Ticks).ToFileTimeUtc();
            return tNow - tLock;
        }


        protected void Run()
        {
            AdSearcher<AdUser> adSearcher = new AdSearcher<AdUser>(_ldapPath);
            string filter = "(&(objectCategory=user)(lockoutTime>=" + FilterGetDiff() + "))";
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

       
        

    }
}
