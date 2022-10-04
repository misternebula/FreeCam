using HarmonyLib;

namespace FreeCam.Patches;

[HarmonyPatch(typeof(NomaiRemoteCamera))]
internal static class NomaiRemoteCameraPatches
{
    // Fixes bug where using the FreeCam from a NomaiRemoteCameraPlatform keeps you stuck in the None input mode

    private static InputMode _oldInputMode;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NomaiRemoteCamera.Activate))]
    public static void NomaiRemoteCamera_Activate() => _oldInputMode = OWInput.GetInputMode();

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NomaiRemoteCamera.Deactivate))]
    public static void NomaiRemoteCamera_Deactivate() => OWInput.ChangeInputMode(_oldInputMode);
}
