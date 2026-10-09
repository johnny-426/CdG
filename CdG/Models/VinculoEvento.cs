using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CdG.Models
{
    public class VinculoEvento
    {
        public int Id { get; set; }

        public int EventoId { get; set; }

        public Evento Evento { get; set; } = null!;

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        public IdentityUser Usuario { get; set; } = null!;

        public PapelEvento Papel { get; set; }
            = PapelEvento.Participante;

        public StatusPresenca StatusPresenca { get; set; }
            = global::CdG.Models.StatusPresenca.Pendente;

        public DateTime PresencaAtualizadaEm { get; set; }
            = DateTime.UtcNow;
    }
}