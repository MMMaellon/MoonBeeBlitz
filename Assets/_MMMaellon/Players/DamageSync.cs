
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace MMMaellon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DamageSync : UdonSharpBehaviour
    {
        [System.NonSerialized]
        public MoonTechnician _localPlayerObject = null;
        [UdonSynced]
        public int damage;
        [UdonSynced]
        public int receiver;
        [UdonSynced]
        public uint event_id = 0;
        void Start()
        {

        }
        public void SendDamage(int amount, int receiver_id){
            if(!Networking.LocalPlayer.IsOwner(gameObject)){
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            receiver = receiver_id;
            damage = amount;
            event_id++;
            RequestSerialization();
        }

        public override void OnDeserialization(){
            if(receiver == _localPlayerObject.transform.GetSiblingIndex()){
                _localPlayerObject.ReceiveDamage(damage);
            }
        }
    }
}
