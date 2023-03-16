namespace CleanMicroserviceSystem.Astra.Infrastructure.BaGet.Core.Entities
{
    // See NuGetGallery.Core's: https://github.com/NuGet/NuGetGallery/blob/master/src/NuGetGallery.Core/Entities/PackageType.cs
    public class PackageType
    {
        public int Key { get; set; }

        public string Name { get; set; }
        public string Version { get; set; }

        public virtual Package Package { get; set; }
    }
}
