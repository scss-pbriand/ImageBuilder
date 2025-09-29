using JasperFx.Events;
using Marten;
using Marten.Schema.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgGen.Application.Infrastructure
{
    public class ImgGenStoreOptions : StoreOptions
    {
        public ImgGenStoreOptions()
        {
            Policies.ForAllDocuments(
            static m =>
            {
                m.Metadata.CreatedAt.Enabled = true;
                m.Metadata.LastModifiedBy.Enabled = true;
                m.Metadata.CorrelationId.Enabled = true;
                m.Metadata.CausationId.Enabled = true;
                m.Metadata.Headers.Enabled = true;

                if (m.IdType == typeof(Guid))
                {
                    m.IdStrategy = new SequentialGuidIdGeneration();
                }
            }
        );

        Events.MetadataConfig.HeadersEnabled = true;
        Events.MetadataConfig.CausationIdEnabled = true;
        Events.MetadataConfig.CorrelationIdEnabled = true;

        Schema.Include<DocumentRegistry>();

        Events.UseMandatoryStreamTypeDeclaration = true;

        Events.StreamIdentity = StreamIdentity.AsGuid;

        UseSystemTextJsonForSerialization(ApplicationJsonSerializer.Default);
        }
    }
}
