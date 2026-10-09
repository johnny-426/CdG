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
    public class AdiarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdiarModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int EventoId { get; private set; }

        public string NomeEvento { get; private set; } = string.Empty;

        [BindProperty]
        public DateTime? NovaData { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
                return Challenge();

            var evento = await _context.Eventos
                .AsNoTracking()
                .FirstOrDefaultAsync(evento =>
                    evento.Id == id &&
                    evento.CriadorId == usuarioId);

            if (evento == null)
                return NotFound();

            if (!PodeAdiar(evento))
                return BloquearAlteracao();

            EventoId = evento.Id;
            NomeEvento = evento.Nome;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var usuarioId = _userManager.GetUserId(User);

            if (usuarioId == null)
                return Challenge();

            var evento = await _context.Eventos
                .FirstOrDefaultAsync(evento =>
                    evento.Id == id &&
                    evento.CriadorId == usuarioId);

            if (evento == null)
                return NotFound();

            if (!PodeAdiar(evento))
                return BloquearAlteracao();

            EventoId = evento.Id;
            NomeEvento = evento.Nome;

            if (!ModelState.IsValid)
                return Page();

            DateTime? novaDataUtc = null;

            if (NovaData.HasValue)
            {
                var fuso = TimeZoneInfo.FindSystemTimeZoneById(
                    "E. South America Standard Time");

                var horarioInformado = DateTime.SpecifyKind(
                    NovaData.Value,
                    DateTimeKind.Unspecified);

                if (fuso.IsInvalidTime(horarioInformado))
                {
                    ModelState.AddModelError(
                        nameof(NovaData),
                        "Informe um horário válido.");

                    return Page();
                }

                novaDataUtc = TimeZoneInfo.ConvertTimeToUtc(
                    horarioInformado, fuso);

                if (novaDataUtc.Value <= DateTime.UtcNow)
                {
                    ModelState.AddModelError(
                        nameof(NovaData),
                        "Informe uma data e um horário futuros.");

                    return Page();
                }
            }

            evento.DataHora = novaDataUtc;

            evento.Status = novaDataUtc.HasValue
                ? StatusEvento.Agendado
                : StatusEvento.Adiado;

            evento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Mensagem"] = novaDataUtc.HasValue
                ? "Evento reagendado com sucesso."
                : "Evento adiado. A nova data será definida depois.";

            return RedirectToPage("/Eventos/Index");
        }

        private static bool PodeAdiar(Evento evento)
        {
            return evento.Status == StatusEvento.Agendado ||
                   evento.Status == StatusEvento.Adiado;
        }

        private IActionResult BloquearAlteracao()
        {
            TempData["Mensagem"] =
                "Eventos cancelados ou concluídos não podem ser reagendados.";

            return RedirectToPage("/Eventos/Index");
        }
    }
}