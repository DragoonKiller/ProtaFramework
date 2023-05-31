using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;

namespace Prota.Unity
{
    [ESystemAllowDuplicate]
    public sealed class NodeSystem : ESystem
    {
        protected override void OnValidate()
        {
            base.OnValidate();
            this.updateMode = UpdateMode.None;
        }
    }
}
