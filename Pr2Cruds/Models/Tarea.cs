using System;
using System.ComponentModel.DataAnnotations;

namespace Pr2Cruds.Models
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        public string? Estado { get; set; }

        public int IdUsuario { get; set; }
    }
}
