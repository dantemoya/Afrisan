using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace Afrisan.Api.Demo;

// También mantiene en memoria las claves que el host de desarrollo puede precargar.
// Evita que la demo intente escribir el key ring en el perfil de Windows.
public sealed class DemoKeyRepository : IXmlRepository
{
    private readonly List<XElement> elements = [];
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        lock (elements) return elements.Select(x => new XElement(x)).ToArray();
    }
    public void StoreElement(XElement element, string friendlyName)
    {
        lock (elements) elements.Add(new XElement(element));
    }
}
