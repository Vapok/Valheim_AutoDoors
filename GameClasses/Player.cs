using HarmonyLib;
using System;
using UnityEngine;

namespace AutoDoors.GameClasses
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    public static class Player_Update_Patch
    {
        private static void Postfix(Player __instance)
        {
            var timeNow = DateTime.UtcNow;
            bool updateIntervalPassed = (timeNow - AutoDoorPlugin.Instance.LastUpdate).TotalSeconds >= AutoDoorPlugin.Instance.Cfg.UpdateInterval;
            
            if (!AutoDoorPlugin.IsRunning || !updateIntervalPassed)
                return;

            var player = Player.m_localPlayer;
            if (player != __instance)
                return;

            bool validPlayer = player != null && !player.IsDead();

            if (AutoDoorPlugin.Instance.IsActive && validPlayer)
            {
                AutoDoorPlugin.Instance.LastUpdate = timeNow;

                var radius = player.m_maxInteractDistance;
                var rsq = radius * radius;

                // remove any invalid doors
                AutoDoorPlugin.Instance.TrackedDoors.RemoveAll(td => !td.IsValid);

                // check doors in range
                foreach (var td in AutoDoorPlugin.Instance.TrackedDoors)
                {
                    if (!td.Update())
                        continue;
                    var door = Door_UpdateState_Patch.DoorCache[td.Id];
                    if (door != null)
                    {
                        var prevInAutoRange = td.InAutoRange;
                        var dsq = Vector3.SqrMagnitude(door.transform.position - player.transform.position);
                        td.InAutoRange = dsq <= rsq;
                        if (td.InAutoRange)
                        {
                            if (!td.IsManual)
                            {
                                if (td.State == 0)
                                {
                                    if (!td.IsAutoOpened)
                                    {
                                        door.Interact(player, false, false);
                                    }
                                    else
                                    {
                                        td.IsManual = true;
                                    }
                                }
                                else
                                {
                                    if (!prevInAutoRange)
                                    {
                                        td.IsManual = true;
                                    }
                                    else
                                    {
                                        td.IsAutoOpened = true;
                                    }
                                }
                            }
                        }
                        else if (prevInAutoRange)
                        {
                            if (!td.IsManual)
                            {
                                td.SetState(0);
                            }
                            else
                            {
                                td.IsManual = false;
                            }
                            td.IsAutoOpened = false;
                        }
                    }
                }

            }

        }

    }
}
