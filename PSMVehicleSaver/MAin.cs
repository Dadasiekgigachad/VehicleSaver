using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace PSMVehicleSaver
{
    public class Main : RocketPlugin<Config>
    {

        protected override void Load()
        {
            Logger.LogWarning("Załadowano Plugin Vehiclesaver!");
            U.Events.OnPlayerDisconnected += OnPlayerDisconnect;
            U.Events.OnPlayerConnected += OnPlayerConnect;
            VehicleManager.onDamageVehicleRequested += Vehicledamage;
        }

        public void OnPlayerConnect(UnturnedPlayer player)
        {

            foreach (var veh in Configuration.Instance.SavedVehicles.ToList()) // Iteracja po kopii listy
            {
                if (veh.Owner == player.CSteamID || (veh.Group == player.SteamGroupID && Configuration.Instance.CanGroupMemberTakeVehicle))
                {
                    RestoreVehicle(veh);
                }
            }
        }

        private void RestoreVehicle(SavedVehicle veh)
        {
            var vehinfo = VehicleManager.findVehicleByNetInstanceID(veh.InstanceID);
            if (vehinfo != null)
            {
                vehinfo.transform.SetPositionAndRotation(veh.Position, veh.Rotation);
                Configuration.Instance.SavedVehicles.Remove(veh);
                Configuration.Save();
            }
        }

        public void OnPlayerDisconnect(UnturnedPlayer player)
        {
            foreach (var vehicle in VehicleManager.vehicles)
            {
                if (vehicle.isLocked)
                {
                    if (Configuration.Instance.zakazanepojazdy.Contains(vehicle.id)) continue;
                    bool wasOwner = vehicle.lockedOwner == player.CSteamID;
                    bool wasGroupMember = vehicle.lockedGroup == player.SteamGroupID;
                    if (!wasOwner && !wasGroupMember) continue;
                    bool areGroupMembersPresent = false;
                    if (vehicle.lockedGroup != CSteamID.Nil)
                    {
                        foreach (var plar in Provider.clients)
                        {
                            if (plar.playerID.steamID == player.CSteamID) continue;
                            if (plar.ToUnturnedPlayer().SteamGroupID == vehicle.lockedGroup)
                            {
                                areGroupMembersPresent = true;
                                break;
                            }
                        }
                    }
                    if (wasOwner && !areGroupMembersPresent)
                    {
                        vehicle.forceRemoveAllPlayers();
                        Configuration.Instance.SavedVehicles.Add(new SavedVehicle
                        {
                            InstanceID = vehicle.instanceID,
                            Position = vehicle.transform.position,
                            Group = player.SteamGroupID,
                            Owner = player.CSteamID,
                            Rotation = vehicle.transform.rotation
                        });
                        vehicle.transform.SetPositionAndRotation(new Vector3(300000f, 100f, 300000f), new Quaternion(0f, 0f, 0f, 0f));
                        Configuration.Save();
                    }
                }
            }
        }
        public void Vehicledamage(CSteamID instigatorSteamID, InteractableVehicle vehicle, ref ushort pendingTotalDamage, ref bool canRepair, ref bool shouldAllow, EDamageOrigin damageOrigin)
        {
            if (Configuration.Instance.SavedVehicles.Any(savedVehicle => savedVehicle.InstanceID == vehicle.instanceID))
            {
                shouldAllow = false;
            }
        }
    }
}
