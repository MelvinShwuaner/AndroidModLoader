using NeoModLoader.api;
using RSG;

class SteamClient //an IQ too high?????
{
    public class SteamId
    {
        public class AccountId
        {
            public static string ToString()
            {
                return "A steam account type shi";
            }
        }
    }
}
namespace NeoModLoader.services
{
    public class MobileWorkShopService : IPlatformSpecificModWorkshopService 
    {
        public void UploadModLoader(string changelog)
        {
            throw new NotSupportedException("workshop is not supported on mobile");
        }

        public Promise UploadMod(string name, string description, string previewImagePath, string workshopPath, string changelog,
            bool verified)
        {
            throw new NotSupportedException("workshop is not supported on mobile");
        }

        public Promise EditMod(ulong fileID, string previewImagePath, string workshopPath, string changelog)
        {
            throw new NotSupportedException("workshop is not supported on mobile");
        }

        public void FindSubscribedMods()
        {
            throw new NotSupportedException("workshop is not supported on mobile");
        }

        public ModDeclare GetNextModFromWorkshopItem()
        {
            throw new NotSupportedException("workshop is not supported on mobile");
        }
    }
}