using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pomelo.Model.Database
{
    public class RedisModel
    {
        public bool IsUse { get; set; }
        public string Host { get; set; }

        public string Password { get; set; }

        public int DefaultDatabase { get; set; }
        public string Prefix { get; set; }
    }
}
