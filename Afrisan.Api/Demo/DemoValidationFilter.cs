using System.ComponentModel.DataAnnotations;

namespace Afrisan.Api.Demo;

// Minimal APIs de .NET 8 no aplican automáticamente los atributos de los DTO de MVC.
// Se reutilizan esas validaciones para que la demo rechace formularios inválidos.
public sealed class DemoValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var errors = new Dictionary<string, string[]>();
        foreach (var dto in context.Arguments.Where(x => x?.GetType().Namespace == "Afrisan.Api.DTOs"))
        {
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(dto!, new ValidationContext(dto!), results, validateAllProperties: true);
            foreach (var group in results.SelectMany(r => r.MemberNames.DefaultIfEmpty("body")
                .Select(name => (name, message: r.ErrorMessage ?? "Dato inválido"))).GroupBy(x => x.name))
                errors[group.Key] = group.Select(x => x.message).ToArray();
        }
        return errors.Count > 0 ? Results.ValidationProblem(errors) : await next(context);
    }
}
