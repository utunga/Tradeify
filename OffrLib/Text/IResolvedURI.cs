using System;


namespace Offr.Text
{
    public interface IResolvedURI
    {
        string CanonicalURI { get; }
        string OriginalURL { get; }
        Uri URL { get; }
    }
}
