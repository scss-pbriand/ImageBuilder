using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Images;

public class ImagePart
{
    public Guid CategoryId { get; set; }
    public Guid? ImageAssetId { get; set; } // null if category not included
}
