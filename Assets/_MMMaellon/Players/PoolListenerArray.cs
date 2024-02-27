
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PoolListenerArray : Cyan.PlayerObjectPool.CyanPlayerObjectPoolEventListener
{
    public Cyan.PlayerObjectPool.CyanPlayerObjectPoolEventListener[] listeners;
    public override void _OnLocalPlayerAssigned()
    {
        foreach(var listener in listeners){
            listener._OnLocalPlayerAssigned();
        }
    }

    public override void _OnPlayerAssigned(VRCPlayerApi player, int poolIndex, UdonBehaviour poolObject)
    {
        foreach(var listener in listeners){
            listener._OnPlayerAssigned(player, poolIndex, poolObject);
        }
    }

    public override void _OnPlayerUnassigned(VRCPlayerApi player, int poolIndex, UdonBehaviour poolObject)
    {
        foreach(var listener in listeners){
            listener._OnPlayerUnassigned(player, poolIndex, poolObject);
        }
    }

}
