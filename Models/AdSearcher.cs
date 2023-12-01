using System.DirectoryServices;


namespace ADUserStatistics.Models
{
    public class AdSearcher<T> where T: class
    {

        private readonly string _ldapPath;


        public AdSearcher(string ldapPath)
        {
            _ldapPath = ldapPath;
        }

       
        public IEnumerable<T> FindAll(string filter, Func<SearchResult, AdSearcher<T>, T> func)
        {
            List<T> res = new List<T>();

            using (DirectoryEntry entry = new DirectoryEntry(_ldapPath))
            {
                using (DirectorySearcher directorySearcher = new DirectorySearcher(entry))
                {
                    directorySearcher.Filter = filter;
                    SearchResultCollection results = directorySearcher.FindAll();                    
                    foreach (SearchResult result in results)
                    {                       
                        res.Add(func(result, this));
                    }
                }
            }
            
            return res;
        }

        public T? FindOne(string filter, Func<SearchResult, AdSearcher<T>, T> func)
        {
            using (DirectoryEntry entry = new DirectoryEntry(_ldapPath))
            {
                using (DirectorySearcher directorySearcher = new DirectorySearcher(entry))
                {
                    directorySearcher.Filter = filter;
                    SearchResult? result = directorySearcher.FindOne();
                    if (result != null)
                    {
                        return func(result, this);
                    }
                    return null;
                }
            }
        }

        
        public string AsString(SearchResult result, string propertyName, string defaultValue = "", int propertyItem = 0)
        {
            if (result != null && result.Properties != null && result.Properties[propertyName] != null
                && result.Properties[propertyName].Count > propertyItem)
            {
                return result.Properties[propertyName][propertyItem].ToString();
            }
            return defaultValue;            
        }

        public DateTime AsDate(SearchResult result, string propertyName, int propertyItem = 0)
        {
            if (result != null && result.Properties != null && result.Properties[propertyName] != null
                && result.Properties[propertyName].Count > propertyItem)
            {
                return DateTime.FromFileTime(Convert.ToInt64(result.Properties[propertyName][propertyItem]));
            }                
            return DateTime.MinValue;
        }

        public TimeSpan AsTimeSpan(SearchResult result, string propertyName, int propertyItem = 0)
        {
            if (result != null && result.Properties != null && result.Properties[propertyName] != null
                && result.Properties[propertyName].Count > propertyItem)
            {
                return TimeSpan.FromTicks(Convert.ToInt64(result.Properties[propertyName][propertyItem]));
            }
            return new TimeSpan();
        }

        public long AsLong(SearchResult result, string propertyName, long defaultValue = 0, int propertyItem = 0)
        {
            if (result != null && result.Properties != null && result.Properties[propertyName] != null 
                && result.Properties[propertyName].Count > propertyItem)
            {
                return Convert.ToInt64(result.Properties[propertyName][propertyItem]);
            }
            return defaultValue;
        }

        public int AsInt(SearchResult result, string propertyName, int defaultValue = 0, int propertyItem = 0)
        {
            if (result != null && result.Properties != null && result.Properties[propertyName] != null
                && result.Properties[propertyName].Count > propertyItem)
            {
                return Convert.ToInt32(result.Properties[propertyName][propertyItem]);
            }
            return defaultValue;
        }



    }
}
