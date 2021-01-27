// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Packaging;
using NuGet.Services.Entities;
using NuGetGallery;

namespace GalleryTools.Commands
{
    public sealed class BackfillTfmMetadataCommand : BackfillCommand<string>
    {
        protected override string MetadataFileName => "tfmMetadata.txt";
        protected override MetadataSourceType SourceType => MetadataSourceType.Entities;

        public static void Configure(CommandLineApplication config)
        {
            Configure<BackfillTfmMetadataCommand>(config);
        }

        protected override string ReadMetadata(Package package)
        {
            if (package.SupportedFrameworks == null)
            {
                return string.Empty;
            }

            return string.Join(",", package.SupportedFrameworks);
        }

        protected override bool ShouldWriteMetadata(string metadata) => true;

        protected override void ConfigureClassMap(PackageMetadataClassMap map)
        {
            map.Map(x => x.Metadata).Index(3);
        }

        protected override void UpdatePackage(Package package, string metadata)
        {
            if (string.IsNullOrEmpty(metadata))
            {
                return;
            }

            package.SupportedFrameworks = metadata.Split(',')
                .Select(f => new PackageFramework {Package = package, TargetFramework = f}).ToArray();
        }
    }
}
