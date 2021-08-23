using System;

namespace AssetInformationListener.Infrastructure.Exceptions
{
    public class AssetNotFoundException : EntityNotFoundException
    {
        public AssetNotFoundException(Guid id)
            : base("Asset", id)
        { }
    }
}
