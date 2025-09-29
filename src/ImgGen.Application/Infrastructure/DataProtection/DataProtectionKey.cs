using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgGen.Application.Infrastructure.DataProtection;

public class DataProtectionKey
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Xml { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}