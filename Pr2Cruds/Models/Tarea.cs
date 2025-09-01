using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Pr2Cruds.Models
{
    public class Tarea : IValidatableObject
    {
        public int Id { get; set; }

        
        public const string NombreRegex = @"^(?=.{3,100}$)(?=.*[A-Za-zÁÉÍÓÚÜáéíóúüÑñ])[A-Za-zÁÉÍÓÚÜáéíóúüÑñ ]+$";

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(NombreRegex, ErrorMessage = "El nombre solo puede contener letras y espacios (3–100 caracteres).")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [StringLength(30, ErrorMessage = "El estado no debe exceder 30 caracteres.")]
        public string? Estado { get; set; }

        [Required(ErrorMessage = "El Id de usuario es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El Id de usuario debe ser mayor que 0.")]
        public int IdUsuario { get; set; }

       
        public static readonly string[] EstadosPermitidos = new[]
        {
            "pendiente",
            "en progreso",
            "completado"
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            var nombreTrim = (Nombre ?? string.Empty).Trim();
            var estadoNorm = (Estado ?? string.Empty).Trim().ToLowerInvariant();

           
            if (nombreTrim == ".")
            {
                yield return new ValidationResult(
                    "El nombre no puede ser solo un punto.",
                    new[] { nameof(Nombre) });
            }

           
            if (!Regex.IsMatch(nombreTrim, NombreRegex))
            {
                yield return new ValidationResult(
                    "El nombre solo puede contener letras (incluye tildes/ñ) y espacios, entre 3 y 100 caracteres.",
                    new[] { nameof(Nombre) });
            }

            
            if (FechaVencimiento.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "La fecha de vencimiento no puede ser anterior a hoy.",
                    new[] { nameof(FechaVencimiento) });
            }

            
            var okEstado = Array.Exists(EstadosPermitidos, e => e.Equals(estadoNorm, StringComparison.OrdinalIgnoreCase));
            if (!okEstado)
            {
                yield return new ValidationResult(
                    "Estado inválido. Usa: pendiente, en progreso o completado.",
                    new[] { nameof(Estado) });
            }
        }
    }
}
