using System;
using System.Collections.Generic;

namespace ARTHS_Data.Entities
{
    public partial class AccountRole
    {
        public AccountRole()
        {
            Accounts = new HashSet<Account>();
        }

        public Guid Id { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
