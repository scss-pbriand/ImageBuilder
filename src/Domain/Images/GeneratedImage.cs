using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Images
{
    public class GeneratedImage
    {
        public Guid Id { get; set; }
        public Guid ImageTypeId { get; set; } // Reference to the ImageType
        public List<GeneratedLayer> Layers { get; set; } = new(); // Layers in the generated image
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of generation
    }

    public class GeneratedLayer
    {
        public Guid CategoryId { get; set; } // Reference to the ImageCategory
        public Guid AssetId { get; set; } // Reference to the ImageAsset
        public int SortOrder { get; set; } // Order in which the layer is applied
    }
}
