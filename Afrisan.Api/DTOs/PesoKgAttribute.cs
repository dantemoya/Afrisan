using System.ComponentModel.DataAnnotations;

namespace Afrisan.Api.DTOs;

// numeric(18,3) conserva milésimas de kg. Rechazar una precisión mayor evita
// que PostgreSQL redondee el pesaje sin que el usuario lo advierta.
[AttributeUsage(AttributeTargets.Property)]
public sealed class PesoKgAttribute : ValidationAttribute
{
    public PesoKgAttribute() : base("El peso debe ser no negativo, caber en numeric(18,3) y tener hasta tres decimales.") { }

    public override bool IsValid(object? value)
        => value is null || value is decimal kg && kg >= 0 && kg < 1000000000000000m && decimal.Round(kg, 3) == kg;
}
