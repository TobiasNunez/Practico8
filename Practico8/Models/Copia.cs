using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practico8.Models;

public partial class Copia
{
    public long Id { get; set; }

    public long PeliculaId { get; set; }

    public bool Deteriorada { get; set; }

    [RegularExpression("^(DVD|BluRay)$", ErrorMessage = "El tipo de medio debe ser 'DVD' o 'BluRay'.")]
    public string Formato { get; set; } = null!;


    public double PrecioAlquiler { get; set; }

    public virtual ICollection<Alquilere> Alquileres { get; set; } = new List<Alquilere>();

    public virtual Pelicula? Pelicula { get; set; }


    public static List<Copia> ObtenerCopiasDisponibles(DbSet<Copia> copias)
    {
        var copiasDisponibles = copias.Where(copia => !copia.Alquileres.Any() || copia.Alquileres.All(alquiler => alquiler.FechaEntregada != null)).ToList();

        return copiasDisponibles;
    }

    public static async Task<List<Copia>> ObtenerCopiasDisponiblesLista(DbSet<Copia> copias)
    {
        return await copias.Include(c => c.Pelicula)
                           .Include(c => c.Alquileres)
                           .Where(c => !c.Alquileres.Any() || c.Alquileres.All(a => a.FechaEntregada != null))
                           .ToListAsync();
    }
  
}