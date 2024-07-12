using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace PSMVehicleSaver
{
    public class SavedVehicle
    {
        public uint InstanceID;
        public Vector3 Position;
        public Quaternion Rotation;
        public CSteamID Owner;
        public CSteamID Group;
    }
}
