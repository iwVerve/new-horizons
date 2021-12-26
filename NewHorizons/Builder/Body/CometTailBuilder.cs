﻿using NewHorizons.External;
using OWML.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NewHorizons.Builder.Body
{
    public static class CometTailBuilder
    {
        public static void Make(GameObject go, BaseModule module, AstroObject primary)
        {
            var cometTail = GameObject.Instantiate(GameObject.Find("Comet_Body/Sector_CO/Effects_CO/Effects_CO_TailMeshes"), go.transform);
            cometTail.transform.localPosition = Vector3.zero;
            cometTail.name = "CometTail";
            cometTail.transform.localScale = Vector3.one * 110 / module.SurfaceSize;
            cometTail.transform.localRotation = Quaternion.Euler(0, 180, 90);
            /*
            var alignment = cometTail.AddComponent<AlignWithTargetBody>();
            alignment.SetTargetBody(primary.GetAttachedOWRigidbody());
            alignment.SetValue("_usePhysicsToRotate", true);
            */
        }
    }
}