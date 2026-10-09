using System.ComponentModel.DataAnnotations;
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
    public class EditarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditarModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public int EventoId { get; private set; }

        [BindProperty]
        public DadosEdicao Entrada { get; set; } = new();

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

            if (!PodeEditar(evento))
            {
                TempData["Mensagem"] =
                    "Eventos cancelados ou concluídos não podem ser editados.";

                return RedirectToPage("/Eventos/Index");
            }

            EventoId = evento.Id;

            Entrada = new DadosEdicao
            {
                Nome = evento.Nome,
                Descricao = evento.Descricao,
                Local = evento.Local,
                LimiteParticipantes = evento.LimiteParticipantes
            };

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

            if (!PodeEditar(evento))
            {
                TempData["Mensagem"] =
                    "Eventos cancelados ou concluídos não podem ser editados.";

                return RedirectToPage("/Eventos/Index");
            }

            EventoId = evento.Id;

            if (!ModelState.IsValid)
                return Page();

            evento.Nome = Entrada.Nome.Trim();
            evento.Descricao = Entrada.Descricao?.Trim();
            evento.Local = Entrada.Local.Trim();
            evento.LimiteParticipantes = Entrada.LimiteParticipantes;
            evento.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Mensagem"] = "Evento atualizado com sucesso.";

            return RedirectToPage("/Eventos/Index");
        }

        private static bool PodeEditar(Evento evento)
        {
            return evento.Status == StatusEvento.Agendado ||
                   evento.Status == StatusEvento.Adiado;
        }

        public class DadosEdicao
        {
            [Required(ErrorMessage = "Informe o nome do evento.")]
            [StringLength(100)]
            public string Nome { get; set; } = string.Empty;

            [StringLength(2000)]
            public string? Descricao { get; set; }

            [Required(ErrorMessage = "Informe o local.")]
            [StringLength(300)]
            public string Local { get; set; } = string.Empty;

            [Range(1, int.MaxValue,
                ErrorMessage = "O limite deve ser maior que zero.")]
            public int? LimiteParticipantes { get; set; }
        }
    }
}