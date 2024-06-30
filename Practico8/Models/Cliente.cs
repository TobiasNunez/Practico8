using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practico8.Models;

public partial class Cliente
{
    public long Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public string DocumentoIdentidad { get; set; } = null!;

    public string? Correo { get; set; }

    public string Telefono { get; set; } = null!;

    public virtual ICollection<Alquilere> Alquileres { get; set; } = new List<Alquilere>();

    public string DatosCliente()
    {
        return $"{this.Id} - {this.Nombre} {this.Apellido}";
    }

    [NotMapped]
    public string DatosCompletos => DatosCliente();
}
