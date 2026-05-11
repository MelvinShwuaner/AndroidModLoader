using NeoModLoader.api;
using RSG;

namespace NeoModLoader.services;

public class ModWorkshopServiceMobile : IPlatformSpecificModWorkshopService 
{
    public void UploadModLoader(string changelog)
    {
        throw new PlatformNotSupportedException("workshop is not supported on mobile");
    }

    public Promise UploadMod(string name, string description, string previewImagePath, string workshopPath, string changelog,
        bool verified)
    {
        throw new PlatformNotSupportedException("workshop is not supported on mobile");
    }

    public Promise EditMod(ulong fileID, string previewImagePath, string workshopPath, string changelog)
    {
        throw new PlatformNotSupportedException("workshop is not supported on mobile");
    }

    public void FindSubscribedMods()
    {
        throw new PlatformNotSupportedException("workshop is not supported on mobile");
    }

    public ModDeclare GetNextModFromWorkshopItem()
    {
        throw new PlatformNotSupportedException("workshop is not supported on mobile");
    }
}