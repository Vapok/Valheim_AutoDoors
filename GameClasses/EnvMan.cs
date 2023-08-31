using HarmonyLib;

namespace AutoDoors.GameClasses
{
    [HarmonyPatch(typeof(EnvMan), "SetForceEnvironment")]
    public static class EnvMan_SetForceEnvironment_Patch
    {
        private static void Postfix(string ___m_forceEnv)
        {
            if (!AutoDoorPlugin.IsRunning)
                return;

            AutoDoorPlugin.Instance.IsCrypt = ___m_forceEnv.Contains("Crypt");
        }
    }
}
