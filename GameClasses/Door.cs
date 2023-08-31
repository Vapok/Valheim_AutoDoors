using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace AutoDoors.GameClasses
{
    [HarmonyPatch(typeof(Door), nameof(Door.UpdateState))]
    public static class Door_UpdateState_Patch
    {
        public static Dictionary<int, Door> DoorCache = new();
        private static void Postfix(ref Door __instance)
        {
            if (!AutoDoorPlugin.IsRunning)
                return;

            if (AutoDoorPlugin.Instance.IsActive && __instance.m_keyItem == null)
            {
                var id = __instance.GetInstanceID();

                if (!AutoDoorPlugin.Instance.TrackedDoors.Any(td => td.Id == id))
                {
                    if (!DoorCache.ContainsKey(id))
                        DoorCache.Add(id,__instance);
                    
                    AutoDoorPlugin.Instance.TrackedDoors.Add(new TrackedDoor(id));

                }
            }
        }
    }
}
