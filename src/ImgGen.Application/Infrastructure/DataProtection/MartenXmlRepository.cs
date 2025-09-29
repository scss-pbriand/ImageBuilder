using Marten;
using Microsoft.AspNetCore.DataProtection.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ImgGen.Application.Infrastructure.DataProtection;

public class MartenXmlRepository(IDocumentStore store) : IXmlRepository
{
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        using var session = store.QuerySession();
        return session.Query<DataProtectionKey>()
            .ToList()
            .Select(k => XElement.Parse(k.Xml))
            .ToList();
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element), "XML element cannot be null.");
        }

        // Convert element to string safely
        var xmlString = element.ToString(SaveOptions.DisableFormatting);

        var key = new DataProtectionKey { Xml = xmlString };
        using var session = store.LightweightSession();
        session.Store(key);
        Task.Run(() => session.SaveChangesAsync()).Wait();
    }
}
