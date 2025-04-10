using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L2CSharP.Model
{
    public  class Accounts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string UserName { get; set; }
        public string PassWordCrypted { get; set; }
        public string PassWord  { get; set; }
        public string LastServer { get; set; }
        public string State { get; set; }

    }
}
