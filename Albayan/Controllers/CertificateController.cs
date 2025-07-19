using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Albayan.Areas.Admin.Data;

namespace Albayan.Controllers
{
    public class CertificateController : Controller
    {
        private readonly PlatformDbContext _context;

        public CertificateController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: /Certificate/View/{guid}
        [Route("Certificate/View/{guid}")]
        public async Task<IActionResult> View(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return NotFound("الرقم المرجعي للشهادة غير صحيح.");
            }

            var certificate = await _context.Certificates
                .Include(c => c.Student)
                .Include(c => c.Course)
                .ThenInclude(co => co.Teacher)
                .FirstOrDefaultAsync(m => m.CertificateGuid == guid);

            if (certificate == null)
            {
                return NotFound("لم يتم العثور على الشهادة المطلوبة.");
            }

            return View("~/Areas/Admin/Views/Certificates/Details.cshtml", certificate);
        }
    }
}
