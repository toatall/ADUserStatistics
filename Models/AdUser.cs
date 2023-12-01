namespace ADUserStatistics.Models
{
    public class AdUser
    {

        /// <summary>
        /// Политика паролей
        /// </summary>
        public PasswordPolicy PasswordPolicy { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="passwordPolicy"></param>
        public AdUser(PasswordPolicy passwordPolicy)
        {
            PasswordPolicy = passwordPolicy;
        }


        /// <summary>
        /// ФИО
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Учетная запись
        /// </summary>
        public string? SAMaccountName { get; set; }

        /// <summary>
        /// Отдел
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Время последнего некоррекного ввода пароля
        /// </summary>
        public DateTime BadPasswordTimeAsDate
        {
            get
            {
                return DateTime.FromFileTime(BadPasswordTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public long BadPasswordTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long LockoutTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string LockoutTimeAsString
        {
            get
            {
                return DateTime.FromFileTime(LockoutTime).ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LogonCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastLogon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PostOfficeBox { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? PwdLastSet { get; set; }

        public string PwdLastSetAsString
        {
            get
            {
                if (PwdLastSet != null)
                {
                    return PwdLastSet.Value.ToString();
                }
                return "";
            }
        }

        
        /// <summary>
        /// Время, когда будет учетная запись разблокирована
        /// </summary>
        public DateTime LeftDateTimeBlock
        {
            get
            {
                if (LockoutTime == 0)
                {
                    return DateTime.MinValue;
                }

                // время блокировки (на основании политики)
                long tLock = DateTime.FromFileTime(PasswordPolicy.LockoutDuration.Negate().Ticks).ToFileTimeUtc();            
                // расчет времени
                return DateTime.FromFileTime(LockoutTime + tLock);
            }
        }

        public string LeftDateTimeBlockAsString
        {
            get
            {
                return LeftDateTimeBlock.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan LeftMinutesBlock
        {
            get
            {
                return LeftDateTimeBlock - DateTime.Now;
            }
        }

        public int LeftMinutesBlockAsInt
        {
            get
            {
                return Convert.ToInt32(LeftMinutesBlock.TotalMinutes);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int LeftPercent
        {
            get
            {
                double lockoutDuration  = PasswordPolicy.LockoutDuration.Negate().TotalMinutes;
                int res = 100 - Convert.ToInt32(LeftMinutesBlock.TotalMinutes / lockoutDuration * 100);
                if (res > 100)
                {
                    return 100;
                }
                else if (res < 0)
                {
                    return 0;
                }
                return res;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int GetExpriedPasswordDays
        {
            get
            {                
                return PwdLastSet != null ? Convert.ToInt32((DateTime.Now - PwdLastSet.Value + PasswordPolicy.MaxPwdAge).TotalDays) : 0;
            }
        }
       
    }
}
