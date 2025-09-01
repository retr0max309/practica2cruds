using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pr2Cruds.Models;
using Pr2Cruds.Data;

namespace Pr2Cruds.Pages
{
    public class CreateModel : PageModel
    {
        private readonly TareaDbContext _context;

        public CreateModel(TareaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tarea Tarea { get; set; } = new();

        public SelectList EstadosSelectList { get; private set; } = default!;

        public IActionResult OnGet()
        {
            EstadosSelectList = BuildEstadosSelectListParaCrear(null);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            
            Tarea.Nombre = Tarea.Nombre?.Trim();
            Tarea.Estado = Tarea.Estado?.Trim();

            
            if (Tarea.Nombre == ".")
                ModelState.AddModelError("Tarea.Nombre", "El nombre no puede ser solo un punto.");

            if (Tarea.IdUsuario <= 0)
                ModelState.AddModelError("Tarea.IdUsuario", "El Id de usuario debe ser mayor que 0.");

            if (Tarea.FechaVencimiento == default)
                ModelState.AddModelError("Tarea.FechaVencimiento", "La fecha de vencimiento es obligatoria.");

            if (Tarea.FechaVencimiento.Date < DateTime.Today)
                ModelState.AddModelError("Tarea.FechaVencimiento", "La fecha de vencimiento no puede ser anterior a hoy.");

           
            var estadoOkCrear = Tarea.Estado != null &&
                                new[] { "pendiente", "en progreso" }
                                .Any(e => e.Equals(Tarea.Estado, StringComparison.OrdinalIgnoreCase));
            if (!estadoOkCrear)
                ModelState.AddModelError("Tarea.Estado", "En creación solo se permite 'pendiente' o 'en progreso'.");

            if (!ModelState.IsValid)
            {
                EstadosSelectList = BuildEstadosSelectListParaCrear(Tarea.Estado);
                return Page();
            }

           
            Tarea.Estado = Tarea.Estado!.ToLowerInvariant();

            _context.Tareas.Add(Tarea);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private static SelectList BuildEstadosSelectListParaCrear(string? selected)
        {
            
            var opciones = new[] { "pendiente", "en progreso" }
                .Select(e => new SelectListItem
                {
                    Value = e,
                    Text = char.ToUpper(e[0]) + e.Substring(1)
                })
                .ToList();

            return new SelectList(opciones, "Value", "Text", selected?.ToLowerInvariant());
        }
    }
}
