using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pr2Cruds.Data;
using Pr2Cruds.Models;

namespace Pr2Cruds.Pages
{
    public class EditModel : PageModel
    {
        private readonly TareaDbContext _context;

        public EditModel(TareaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tarea Tarea { get; set; } = default!;

        public SelectList EstadosSelectList { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var tarea = await _context.Tareas.FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null) return NotFound();

            Tarea = tarea;
            EstadosSelectList = BuildEstadosSelectListParaEditar(Tarea.Estado);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Tarea == null) return NotFound();

            
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

            
            var estadoOkEditar = Tarea.Estado != null &&
                                 Tarea.EstadosPermitidos.Any(e => e.Equals(Tarea.Estado, StringComparison.OrdinalIgnoreCase));
            if (!estadoOkEditar)
                ModelState.AddModelError("Tarea.Estado", "Estado inválido. Usa: pendiente, en progreso o completado.");

            if (!ModelState.IsValid)
            {
                EstadosSelectList = BuildEstadosSelectListParaEditar(Tarea.Estado);
                return Page();
            }

            try
            {
                Tarea.Estado = Tarea.Estado!.ToLowerInvariant();

                _context.Attach(Tarea).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.Tareas.AnyAsync(e => e.Id == Tarea.Id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToPage("./Index");
        }

        private static SelectList BuildEstadosSelectListParaEditar(string? selected)
        {
            
            var items = Tarea.EstadosPermitidos
                .Select(e => new SelectListItem
                {
                    Value = e,
                    Text = char.ToUpper(e[0]) + e.Substring(1)
                })
                .ToList();

            return new SelectList(items, "Value", "Text", selected?.ToLowerInvariant());
        }
    }
}
