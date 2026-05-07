using NeoModLoader.General;
using NeoModLoader.utils;
using UnityEngine;

namespace NeoModLoader.ui;

internal static class UIManager
{
    public static void init()
    {
        InformationWindow.CreateWindow("Information", "Information Title");
        ModListWindow.CreateAndInit("NeoModList");
        if(!Config.isAndroid){
            NewModListWindow.CreateAndInit("NMLMenu");
            WorkshopModListWindow.CreateAndInit("WorkshopMods");
            ModUploadWindow.CreateAndInit("ModUpload");
            ModUploadingProgressWindow.CreateAndInit("ModUploadingProgress");
            PowerButtonCreator.AddButtonToTab(
            PowerButtonCreator.CreateWindowButton("NewNML_ModsList", "NMLMenu",
                                                  InternalResourcesGetter.GetIcon()),
            PowerButtonCreator.GetTab(PowerTabNames.Main),
            23);
        }
        ModUploadAuthenticationWindow.CreateAndInit("ModUploadAuthentication");
        ModConfigureWindow.CreateAndInit("ModConfigure");
        PowerButtonCreator.AddButtonToTab(
            PowerButtonCreator.CreateWindowButton("NML_ModsList", "NeoModList",
                                                  InternalResourcesGetter.GetIcon()),
            PowerButtonCreator.GetTab(PowerTabNames.Main),
          22);
    }
}