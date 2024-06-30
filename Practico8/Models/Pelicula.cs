using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Practico8.Models;


public partial class Pelicula
{

    
    public long Id { get; set; }

    public string Titulo { get; set; } = null!;

    public int Anio { get; set; }

    [Range(1, 10, ErrorMessage = "La clasificacion debe estar entrar entre 1 y 10.")]
    public int Calificacion { get; set; }

    public virtual ICollection<Copia> Copia { get; set; } = new List<Copia>();
}
