using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Common.Extension
{
    public static class TenantExtension
    {
        public static string SanitizeTenant(this string source)
        {
            return $"Tenant_{Regex.Replace(source, @"[^a-zA-Z0-9]", "")}";
        }
    }
}
