using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practico8.Models;

namespace Practico8.Controllers
{
    public class AlquileresController : Controller
    {
        private readonly Practico8Context _context;

        public AlquileresController(Practico8Context context)
        {
            _context = context;
        }

        // GET: Alquileres
        public async Task<IActionResult> Index()
        {
            var practico8Context = _context.Alquileres.Include(a => a.Cliente).Include(a => a.Copia);
            var alquileres = await practico8Context.ToListAsync();

            foreach (var alquiler in alquileres)
            {
                await alquiler.SetPeliculaTituloAsync(_context);
            }

            return View(await practico8Context.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AlquileresActivos()
        {
            var activos = await _context.Alquileres.Include(a => a.Cliente)
                                                   .Include(a => a.Copia)
                                                   .ThenInclude(c => c.Pelicula)
                                                   .Where(a=> a.FechaEntregada == null).ToListAsync(); 

            foreach (var activo in activos)
            {
                await activo.SetPeliculaTituloAsync(_context);
            }

            ViewBag.Activos = true;
            return View("index", activos);
        }


        [HttpPost]
        public async Task<IActionResult> FechaTopeVencida()
        {
            DateTime fechaActual = DateTime.Now;
            var vencidos = await _context.Alquileres.Include(a => a.Cliente)
                                                    .Include(a => a.Copia)
                                                    .ThenInclude(c => c.Pelicula)
                                                    .Where(a => a.FechaTope < fechaActual && a.FechaEntregada == null).ToListAsync();

            foreach (var vencido in vencidos)
            {
                await vencido.SetPeliculaTituloAsync(_context);
            }

            ViewBag.Vencidos = true;
            return View("index", vencidos);
        }

        [HttpPost]
        public async Task<IActionResult> FiltrarPorUsuario(long usuarioId)
        {
            var alquileres = await _context.Alquileres
                .Include(a => a.Cliente)
                .Include(a => a.Copia)
                .ThenInclude(c => c.Pelicula)
                .Where(a => a.ClienteId == usuarioId)
                .OrderBy(a => a.FechaEntregada == null ? 0 : 1) // Ordena los activos primero
                .ThenBy(a => a.FechaAlquiler) // Luego ordena por fecha de alquiler
                .ToListAsync();

            foreach (var alquiler in alquileres)
            {
                await alquiler.SetPeliculaTituloAsync(_context);
            }

            ViewBag.IsFiltered = true;
            return View("Index", alquileres);
        }

        // GET: Alquileres/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquilere = await _context.Alquileres
                .Include(a => a.Cliente)
                .Include(a => a.Copia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alquilere == null)
            {
                return NotFound();
            }

            return View(alquilere);
        }

        // GET: Alquileres/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "DatosCompletos");

            var copiasDisponibles = Copia.ObtenerCopiasDisponibles(_context.Copias);
            ViewData["CopiaId"] = new SelectList(copiasDisponibles, "Id", "Id");

            // Inicializa el modelo con la fecha actual
            var model = new Alquilere
            {
                FechaAlquiler = DateTime.Now
            };

            return View(model);
        }
        // POST: Alquileres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CopiaId,ClienteId,FechaAlquiler,FechaEntregada")] Alquilere alquilere)
        {
           
         
                if (ModelState.IsValid)
                {
                    alquilere.FechaTope = alquilere.FechaAlquiler.AddDays(3);
                    _context.Add(alquilere);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", alquilere.ClienteId);
            ViewData["CopiaId"] = new SelectList(_context.Copias, "Id", "Id", alquilere.CopiaId);
            return View(alquilere);
        }


        // GET: Alquileres/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquilere = await _context.Alquileres.FindAsync(id);
            if (alquilere == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", alquilere.ClienteId);
            ViewData["CopiaId"] = new SelectList(_context.Copias, "Id", "Id", alquilere.CopiaId);
            return View(alquilere);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,CopiaId,ClienteId,FechaAlquiler,FechaTope,FechaEntregada")] Alquilere alquilere)
        {
            if (id != alquilere.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alquilere);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlquilereExists(alquilere.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", alquilere.ClienteId);
            ViewData["CopiaId"] = new SelectList(_context.Copias, "Id", "Id", alquilere.CopiaId);
            return View(alquilere);
        }

        // GET: Alquileres/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alquilere = await _context.Alquileres
                .Include(a => a.Cliente)
                .Include(a => a.Copia)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alquilere == null)
            {
                return NotFound();
            }

            return View(alquilere);
        }

        // POST: Alquileres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var alquilere = await _context.Alquileres.FindAsync(id);
            if (alquilere != null)
            {
                _context.Alquileres.Remove(alquilere);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlquilereExists(long id)
        {
            return _context.Alquileres.Any(e => e.Id == id);
        }
    }

}
