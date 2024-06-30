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
    public class CopiasController : Controller
    {
        private readonly Practico8Context _context;

        public CopiasController(Practico8Context context)
        {
            _context = context;
        }


        // GET: Copias
        public async Task<IActionResult> Index()
        {
            var practico8Context = _context.Copias.Include(c => c.Pelicula);
            ViewBag.IsFiltered = false; // No estamos filtrando
            return View(await practico8Context.ToListAsync());
        }

        // POST: Copias/IndexCopiasDisponibles
        [HttpPost]
        public async Task<IActionResult> IndexCopiasDisponibles()
        {
            var copiasDisponibles = await Copia.ObtenerCopiasDisponiblesLista(_context.Copias);
            ViewBag.IsFiltered = true; // Estamos filtrando
            return View("Index", copiasDisponibles);
        }



        // GET: Copias/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copia = await _context.Copias
                .Include(c => c.Pelicula)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (copia == null)
            {
                return NotFound();
            }

            return View(copia);
        }

        // GET: Copias/Create
        public IActionResult Create()
        {
            ViewData["PeliculaId"] = new SelectList(_context.Peliculas, "Id", "Titulo");
            return View();
        }

        // POST: Copias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PeliculaId,Deteriorada,Formato,PrecioAlquiler")] Copia copia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(copia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PeliculaId"] = new SelectList(_context.Peliculas, "Id", "Titulo", copia.PeliculaId);
            return View(copia);

        }

        // GET: Copias/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copia = await _context.Copias.FindAsync(id);
            if (copia == null)
            {
                return NotFound();
            }
            ViewData["PeliculaId"] = new SelectList(_context.Peliculas, "Id", "Id", copia.PeliculaId);
            return View(copia);
        }

        // POST: Copias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,PeliculaId,Deteriorada,Formato,PrecioAlquiler")] Copia copia)
        {
            if (id != copia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(copia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CopiaExists(copia.Id))
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
            ViewData["PeliculaId"] = new SelectList(_context.Peliculas, "Id", "Id", copia.PeliculaId);
            return View(copia);
        }

        // GET: Copias/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var copia = await _context.Copias
                .Include(c => c.Pelicula)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (copia == null)
            {
                return NotFound();
            }

            return View(copia);
        }

        // POST: Copias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var copia = await _context.Copias.FindAsync(id);
            if (copia != null)
            {
                _context.Copias.Remove(copia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CopiaExists(long id)
        {
            return _context.Copias.Any(e => e.Id == id);
        }
    }
}
