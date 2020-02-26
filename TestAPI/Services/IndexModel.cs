using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Services
{
    public class IndexModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly IConfiguration _config;
        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public int NumberConfig { get; private set; }

        public void OnGet()
        {
            NumberConfig = _config.GetValue<int>("NumberKey", 99);
        }
    }
}
