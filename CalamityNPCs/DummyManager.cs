using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using UnityEngine;
using MEC;
using CalamityNPCs;
using LabApi.Features.Wrappers;

namespace CalamityNPCs
{
    public class DummyManager
    {
        public static List<DummyController> Dummies = new();


        public static void SpawnDummy(Vector3 position) { /* ... */ }
        public static void RemoveAllDummies() { /* ... */ }
    }
}
