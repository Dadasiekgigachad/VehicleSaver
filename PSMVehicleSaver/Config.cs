using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PSMVehicleSaver
{
    public class Config : IRocketPluginConfiguration
    {
        public List<ushort> zakazanepojazdy = new List<ushort>();
        public bool CanGroupMemberTakeVehicle;
        public bool czymozezostacpojazddopokiownergrupaniewyjdzie;
        public  List<SavedVehicle> SavedVehicles = new List<SavedVehicle>();

        public void LoadDefaults()
        {
            CanGroupMemberTakeVehicle = true;
            czymozezostacpojazddopokiownergrupaniewyjdzie = true;
        }
    }
}
