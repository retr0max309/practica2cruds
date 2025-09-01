using System;
using System.Collections.Generic;
using DA = System.ComponentModel.DataAnnotations; // <— alias para evitar choque con Bogus
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pr2Cruds.Data;
using Pr2Cruds.Models;

namespace Pr2Cruds.Seeders
{
    public class TareaSeeder
    {
        public void Run(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<TareaDbContext>();
            context.Database.EnsureCreated();
            SeedTareaData(context);
        }

        private void SeedTareaData(TareaDbContext context)
        {
            context.Tareas.RemoveRange(context.Tareas);
            context.SaveChanges();

            var faker = new Faker<Tarea>("es")
                .RuleFor(t => t.Nombre, f =>
                {
                    var words = f.Lorem.Words(f.Random.Int(2, 4));
                    var nombre = string.Join(" ", words);
                    if (nombre == "." || nombre.Trim().Length < 3) nombre = "Tarea Generica";
                    return nombre;
                })
                .RuleFor(t => t.FechaVencimiento, f => f.Date.Future(1).Date)
                .RuleFor(t => t.Estado, f => f.PickRandom(new[] { "pendiente", "en progreso" }))
                .RuleFor(t => t.IdUsuario, f => f.Random.Int(1, 10));

            var tareasValidas = new List<Tarea>();
            int intentos = 0;

            while (tareasValidas.Count < 50 && intentos < 500)
            {
                intentos++;
                var t = faker.Generate();
                NormalizarParaCrear(t);
                if (EsValida(t)) tareasValidas.Add(t);
            }

            context.Tareas.AddRange(tareasValidas);
            context.SaveChanges();
        }

        private static void NormalizarParaCrear(Tarea t)
        {
            t.Nombre = (t.Nombre ?? string.Empty).Trim();
            if (!string.Equals(t.Estado, "pendiente", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(t.Estado, "en progreso", StringComparison.OrdinalIgnoreCase))
            {
                t.Estado = "pendiente";
            }
            t.Estado = (t.Estado ?? "pendiente").ToLowerInvariant();
        }

        private static bool EsValida(Tarea t)
        {
            var resultados = new List<DA.ValidationResult>();
            var contexto = new DA.ValidationContext(t, serviceProvider: null, items: null);
            return DA.Validator.TryValidateObject(t, contexto, resultados, validateAllProperties: true);
        }
    }
}
