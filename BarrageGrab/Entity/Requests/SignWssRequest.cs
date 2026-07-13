using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarrageGrab.Entity.Requests
{
    public class SignWssRequest
    {
        [NotNull]
        public string? ApiKey { get; set; }

        public string? BrowserName { get; set; }

        public string? BrowserVersion { get; set; }

        [NotNull]
        public string? UserUniqueId { get; set; }

        [NotNull]
        public string? RoomId { get; set; }
    }
}
