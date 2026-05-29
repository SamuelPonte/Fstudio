using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Fstudio.Pages;

public class StatusCodeModel : PageModel
{
    public int ErrorCode { get; set; }

    public void OnGet(int code)
    {
        ErrorCode = code;
    }
}
