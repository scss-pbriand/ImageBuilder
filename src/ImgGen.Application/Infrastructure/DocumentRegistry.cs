using Domain.Identity;
using Domain.Identity.Models;
using Domain.Images;
using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgGen.Application.Infrastructure;

public class DocumentRegistry : MartenRegistry
{
    public DocumentRegistry()
    {
        For<ImageType>().Identity(x => x.Id);
        For<GeneratedImage>().Identity(x => x.Id).ForeignKey<ImageType>(x => x.ImageTypeId);

        RegisterIdentity();
    }

    private void RegisterIdentity()
    {
        // These documents to not implement DocumentAggregate, so require special configuration.;

        For<ApplicationUser>()
            .Identity(x => x.AggregateId)
            .Index(x => x.Id.Value, c => c.IsUnique = true)
            .Duplicate(u => u.Email)
            .Duplicate(u => u.EmailConfirmed)
            .Duplicate(u => u.PhoneNumber)
            .Duplicate(u => u.PhoneNumberConfirmed)
            .Duplicate(u => u.Name.FirstName)
            .Duplicate(u => u.Name.LastName)
            .Duplicate(u => u.Name.MiddleName)
            .Duplicate(u => u.Name.FullName)
            .UseOptimisticConcurrency(true)
            .UseNumericRevisions(true)
            .SoftDeleted()
            .Index(
                x => x.NormalizedUserName!, i =>
                {
                    i.IsUnique = true;
                    i.Predicate = "mt_deleted = false";
                }
            )
            .Index(
                x => x.NormalizedEmail!, i =>
                {
                    i.IsUnique = true;
                    i.Predicate = "mt_deleted = false";
                }
            )
            .Metadata(m =>
            {
                m.IsSoftDeleted.MapTo(x => x.IsDeleted);
                m.SoftDeletedAt.MapTo(x => x.DeletedAt);
            }
            );

        For<Role>()
            .Identity(x => x.AggregateId)
            .Index(x => x.Id, c => c.IsUnique = true)
            .UseOptimisticConcurrency(true)
            .UseNumericRevisions(true)
            .Index(x => x.NormalizedName, x => x.IsUnique = true);

    }
}
