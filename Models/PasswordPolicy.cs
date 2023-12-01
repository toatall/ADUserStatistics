using System.DirectoryServices;

namespace ADUserStatistics.Models
{
    public class PasswordPolicy
    {
        private int? _minPwdLength;
        private int? _pwdHistoryLength;
        private TimeSpan? _maxPwdAge;
        private TimeSpan? _minPwdAge;
        private int? _lockoutThreshold;
        private TimeSpan? _lockoutDuration;
        private string? _distinguishedName;    

        public int MinPwdLength { get => _minPwdLength ?? 0; }
        public int PwdHistoryLength { get => _pwdHistoryLength ?? 0;  }
        public TimeSpan MaxPwdAge { get => _maxPwdAge ?? new TimeSpan(); }
        
        public TimeSpan MinPwdAge { get => _minPwdAge ?? new TimeSpan(); }
        public int LockoutThreshold { get => _lockoutThreshold ?? 0; }
        public TimeSpan LockoutDuration { get => _lockoutDuration ?? new TimeSpan(); }
        public string DistinguishedName { get => _distinguishedName ?? ""; }

        /// <summary>
        /// 
        /// </summary>
        public PasswordPolicy()
        {
            Run();
        }

        protected void Run()
        {
            /*
            AdSearcher<PasswordPolicy> adSearcher = new AdSearcher<PasswordPolicy>(_ldapPath);
            adSearcher.FindOne("(objectClass=domainDNS)", AdPasswordPolicyCreator);
            */
            
            using (DirectoryEntry entry = new DirectoryEntry("LDAP://rootDSE"))
            {
                // Получение DN (Distinguished Name) домена
                string domainDn = Convert.ToString(entry.Properties["defaultNamingContext"].Value);

                // Создание объекта DirectoryEntry для домена
                using (DirectoryEntry domainEntry = new DirectoryEntry($"LDAP://{domainDn}"))
                {
                    // Создание объекта DirectorySearcher с фильтром для политики паролей
                    using (DirectorySearcher searcher = new DirectorySearcher(domainEntry))
                    {
                        if (searcher != null && searcher is DirectorySearcher)
                        {
                            searcher.Filter = "(objectClass=domainDNS)";
                            searcher.PropertiesToLoad.Add("minPwdLength");
                            searcher.PropertiesToLoad.Add("pwdHistoryLength");
                            searcher.PropertiesToLoad.Add("maxPwdAge");
                            searcher.PropertiesToLoad.Add("minPwdAge");
                            searcher.PropertiesToLoad.Add("lockoutThreshold");
                            searcher.PropertiesToLoad.Add("distinguishedName");
                            searcher.PropertiesToLoad.Add("lockoutDuration");

                            
                            // выполнение поиска и получение результата
                            SearchResult result = searcher.FindOne();
                            if (result != null)
                            {
                                _minPwdLength = Convert.ToInt32(result.Properties["minPwdLength"][0]);
                                _pwdHistoryLength = Convert.ToInt32(result.Properties["pwdHistoryLength"][0]);
                                _maxPwdAge = TimeSpan.FromTicks((long)result.Properties["maxPwdAge"][0]);
                                _minPwdAge = TimeSpan.FromTicks((long)result.Properties["minPwdAge"][0]);
                                _lockoutThreshold = Convert.ToInt32(result.Properties["lockoutThreshold"][0]);
                                _lockoutDuration = TimeSpan.FromTicks((long)result.Properties["lockoutDuration"][0]);
                                _distinguishedName = result.Properties["distinguishedName"][0].ToString();
                            }
                        }
                    }
                }
            }
        }
        /*
        private PasswordPolicy AdPasswordPolicyCreator(dynamic resultSearch, AdSearcher<PasswordPolicy> searcher)
        {
            _minPwdLength = searcher.AsInt(resultSearch, "minPwdLength");
            _pwdHistoryLength = searcher.AsInt(resultSearch, "pwdHistoryLength");
            _maxPwdAge = searcher.AsTimeSpan(resultSearch, "maxPwdAge");
            _minPwdAge = searcher.AsTimeSpan(resultSearch, "minPwdAge");
            _lockoutThreshold = searcher.AsInt(resultSearch, "lockoutThreshold");
            _lockoutDuration = searcher.AsTimeSpan(resultSearch, "lockoutDuration");
            _distinguishedName = searcher.AsString(resultSearch, "distinguishedName");
            return this;
        }
        */

    }
}
