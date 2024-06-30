using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Practico8.Models;

public partial class Alquilere
{
    public long Id { get; set; }

    public long CopiaId { get; set; }

    public long ClienteId { get; set; }

   
    [Display(Name = "Fecha de Alquiler")]
   
    [DataType(DataType.Date)]

    public DateTime FechaAlquiler { get; set; }

    public DateTime FechaTope { get; set; }

    public DateTime? FechaEntregada { get; set; }

    public virtual Cliente? Cliente { get; set; }
    public virtual Copia? Copia { get; set; }

    [NotMapped]
    public string? PeliculaTitulo { get; private set; }

    public async Task SetPeliculaTituloAsync(Practico8Context context)
    {
        if (Copia != null && Copia.PeliculaId != 0)
        {
            var pelicula = await context.Peliculas.FindAsync(Copia.PeliculaId);
            PeliculaTitulo = pelicula?.Titulo ?? "Sin título";
        }
        else
        {
            PeliculaTitulo = "Sin título";
        }
    }

 
}
