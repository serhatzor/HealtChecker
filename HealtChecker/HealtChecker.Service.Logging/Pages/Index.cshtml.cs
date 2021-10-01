using HealtChecker.Service.Logging.Data.Entities;
using HealtChecker.Service.Logging.Data.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.Service.Logging
{
    public class LogsModel : PageModel
    {
        private readonly ILogDbContext _context;

        public LogsModel(ILogDbContext context)
        {
            _context = context;
        }

        public IList<Log> Log { get; set; }

        public async Task OnGetAsync()
        {
            Log = await _context.Logs.ToListAsync();
        }
    }
}
