using CdG.Data;
using CdG.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CdG.Pages.Eventos
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Evento> Eventos { get; set; } = new();

        [TempData]
        public string? Mensagem { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
                return Challenge();

            Eventos = await _context.Eventos
                .AsNoTracking()
                .Where(evento => evento.CriadorId == usuarioId)
                .OrderByDescending(evento => evento.CriadoEm)
                .ToListAsync();

            return Page();
        }

        public string FormatarData(DateTime? dataUtc)
        {
            if (dataUtc == null)
                return "Data a definir";

            var fuso = TimeZoneInfo.FindSystemTimeZoneById(
                "E. South America Standard Time");

            return TimeZoneInfo
                .ConvertTimeFromUtc(dataUtc.Value, fuso)
                .ToString("dd/MM/yyyy 'às' HH:mm");
        }
    }
}