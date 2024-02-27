
using UdonSharp;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;

namespace MMMaellon
{
    public class MoonTechnicianPoolListener : Cyan.PlayerObjectPool.CyanPlayerObjectPoolEventListener
    {
        public override void _OnLocalPlayerAssigned()
        {

        }

        public override void _OnPlayerAssigned(VRCPlayerApi player, int poolIndex, UdonBehaviour poolObject)
        {
            if(!player.IsValid() || !player.isLocal){
                return;
            }
            MoonTechnician local_tech = poolObject.GetComponent<MoonTechnician>();
            foreach (MoonTechnician tech in transform.GetComponentsInChildren<MoonTechnician>(true))
            {
                tech.SetLocalPlayerObject(local_tech);
            }
            local_tech.ResetPlayer();
        }

        public override void _OnPlayerUnassigned(VRCPlayerApi player, int poolIndex, UdonBehaviour poolObject)
        {

        }
    }
}
