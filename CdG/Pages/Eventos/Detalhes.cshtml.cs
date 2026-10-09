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
    public class DetalhesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DetalhesModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Evento Evento { get; private set; } = null!;

        public string DataFormatada { get; private set; }
            = "Data a definir";

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

            Evento = evento;

            if (evento.DataHora.HasValue)
            {
                var fuso = TimeZoneInfo.FindSystemTimeZoneById(
                    "E. South America Standard Time");

                DataFormatada = TimeZoneInfo
                    .ConvertTimeFromUtc(evento.DataHora.Value, fuso)
                    .ToString("dd/MM/yyyy 'às' HH:mm");
            }

            return Page();
        }

            public async Task<IActionResult> OnPostCancelarAsync(int id)
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

            if (evento.Status != StatusEvento.Agendado &&
                evento.Status != StatusEvento.Adiado)
            {
                TempData["Mensagem"] =
                    "Esse evento já foi cancelado ou concluído.";

                return RedirectToPage("/Eventos/Index");
            }

            evento.Status = StatusEvento.Cancelado;
            evento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Evento cancelado com sucesso.";

            return RedirectToPage("/Eventos/Index");
        }
        public async Task<IActionResult> OnPostConcluirAsync(int id)
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

            if (evento.Status != StatusEvento.Agendado ||
                !evento.DataHora.HasValue ||
                evento.DataHora.Value > DateTime.UtcNow)
            {
                TempData["Mensagem"] =
                    "Só é possível concluir um evento agendado cujo horário já chegou.";

                return RedirectToPage("/Eventos/Index");
            }

            evento.Status = StatusEvento.Concluido;
            evento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Evento concluído com sucesso.";

            return RedirectToPage("/Eventos/Index");
        }
    }
} 