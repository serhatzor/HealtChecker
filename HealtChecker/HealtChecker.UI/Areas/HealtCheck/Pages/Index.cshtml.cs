using HealtChecker.Shared.Models;
using HealtChecker.UI.Services.Implementations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealtChecker.UI.Areas.HealtCheck.Pages
{
    public class IndexModel : PageModel
    {
        private HealtCheckService _healtCheckService { get; init; }
        public List<HealtCheckEndpointModel> EndpointModels { get; set; }
        public IndexModel(HealtCheckService healtCheckService)
        {
            _healtCheckService = healtCheckService;
        }
        public async Task OnGet()
        {
            ServiceResult<List<HealtCheckEndpointModel>> result = await _healtCheckService.GetHealtCheckEndpoints();
            EndpointModels = result.Data;
        }
    }
}
