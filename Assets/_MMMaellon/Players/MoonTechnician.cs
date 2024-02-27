
using MMMaellon.PlateTectonics;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace MMMaellon
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MoonTechnician : UdonSharpBehaviour
    {
        public PlateTectonics.PlayerSync plate;
        public CapsuleCollider capsule_collider;
        [System.NonSerialized, UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(health))]
        public int _health = 100;
        [System.NonSerialized, UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(maxHealth))]
        public int _maxHealth = 100;
        public int maxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                _SetHealthBar();
                if (IsOwnerLocal())
                {
                    RequestSerialization();
                }
            }
        }
        public int health
        {
            get => _health;
            set
            {
                if (value > _health)
                {
                    if (_health >= maxHealth)
                    {
                        //prevent repeat events
                        return;
                    }
                    _health = value;
                    if (value >= maxHealth)
                    {
                        _health = maxHealth;
                        _OnIncreaseHealth();
                        _OnMaxHealth();
                    }
                    else
                    {
                        _OnIncreaseHealth();
                    }
                }
                else if (value < _health)
                {
                    if (_health <= 0)
                    {
                        //prevent repeat events
                        return;
                    }
                    _health = value;
                    if (value <= 0)
                    {
                        _health = 0;
                        _OnDecreaseHealth();
                        _OnMinHealth();
                    }
                    else
                    {
                        _OnDecreaseHealth();
                    }
                }
                _SetHealthBar();
                if (IsOwnerLocal())
                {
                    RequestSerialization();
                }
            }
        }
        [Tooltip("Will automatically set event parameters on this animator for the following events: \"OnIncreaseHealth\", \"OnDecreaseHealth\", \"OnMaxHealth\", \"OnMinHealth\", \"OnIncreaseShield\", \"OnDecreaseShield\", \"OnMaxShield\", and \"OnMinShield\". Similar parameters will also be set for your resources, but with the resource name in place of \"Health\" and \"Shield\"")]
        public Animator eventAnimator = null;
        public virtual void _OnIncreaseHealth()
        {
            if (Utilities.IsValid(eventAnimator))
            {
                eventAnimator.SetTrigger("OnIncreaseHealth");
            }
        }

        public virtual void _OnDecreaseHealth()
        {
            if (Utilities.IsValid(eventAnimator))
            {
                eventAnimator.SetTrigger("OnDecreaseHealth");
            }
        }
        public virtual void _OnMaxHealth()
        {
            if (Utilities.IsValid(eventAnimator))
            {
                eventAnimator.SetTrigger("OnMaxHealth");
            }
        }

        public virtual void _OnMinHealth()
        {
            if (Utilities.IsValid(eventAnimator))
            {
                eventAnimator.SetTrigger("OnMinHealth");
            }
        }
        [System.NonSerialized, UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(team))]
        public int _team = -1001;
        public int team
        {
            get => _team;
            set
            {
                _team = value;
                if (Utilities.IsValid(statsAnimator))
                {
                    statsAnimator.SetInteger("team", value);
                }
                if (IsOwnerLocal())
                {
                    RequestSerialization();
                }
            }
        }
        [System.NonSerialized, UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(state))]
        public int _state = STATE_SPECTATING;
        public int state
        {
            get => _state;
            set
            {
                _state = value;
                if (Utilities.IsValid(statsAnimator))
                {
                    statsAnimator.SetInteger("state", value);
                }
                if (IsOwnerLocal())
                {
                    RequestSerialization();
                }
            }
        }
        [System.NonSerialized]
        public const int STATE_SPECTATING = -1001;//can't deal or take damage
        [System.NonSerialized]
        public const int STATE_NORMAL = 0;//can do everything
        [System.NonSerialized]
        public const int STATE_DISABLED = 1;//can't deal damage
        [System.NonSerialized]
        public const int STATE_INVINCIBLE = 2;//can't take damage
        [System.NonSerialized]
        public const int STATE_DOWNED = 3;//can't do anything, but can still be revived
        [System.NonSerialized]
        public const int STATE_DEAD = 3;//can't do anything, waiting for respawn
        [System.NonSerialized]
        public const int STATE_FROZEN = 5;//can't do anything and can't move, while resurrecting another player and stuff like that
        [Tooltip("Will automatically set \"health\" and \"shield\" float parameters and a \"team\" integer parameter on this animator")]
        [FieldChangeCallback(nameof(statsAnimator))]
        public Animator _statsAnimator = null;
        public Animator statsAnimator
        {
            get => _statsAnimator;
            set
            {
                _statsAnimator = value;
                _SetAnimatorValues();
            }
        }
        public void _SetHealthBar()
        {
            if (Utilities.IsValid(statsAnimator))
            {
                statsAnimator.SetFloat("health", Mathf.Clamp01(health / (float)maxHealth));
            }
        }
        public void _SetAnimatorValues(){
            if (Utilities.IsValid(statsAnimator))
            {
                statsAnimator.SetInteger("team", team);
                statsAnimator.SetInteger("state", state);
                statsAnimator.SetFloat("health", Mathf.Clamp01(health / (float)maxHealth));
            }
        }

        public bool IsOwnerLocal(){
            return plate.local;
        }
        P_Shooters.P_Shooter otherPShooter;
        public virtual void OnParticleCollision(GameObject other)
        {
            if (!Utilities.IsValid(other))
            {
                return;
            }
            otherPShooter = other.GetComponent<P_Shooters.P_Shooter>();
            if (!Utilities.IsValid(otherPShooter))
            {
                otherPShooter = other.GetComponentInParent<P_Shooters.P_Shooter>();
            }
            if (Utilities.IsValid(otherPShooter) && otherPShooter.damageOnParticleCollision)
            {
                OnPShooterHit(otherPShooter);
            }
        }
        [System.NonSerialized]
        public MoonTechnician _localPlayerObject = null;
        public void SetLocalPlayerObject(MoonTechnician local_tech){
            _localPlayerObject = local_tech;

            foreach (DamageSync sync in damage_syncs){
                sync._localPlayerObject = local_tech;
            }
        }
        public virtual void OnPShooterHit(P_Shooters.P_Shooter otherShooter)
        {
            if (!Utilities.IsValid(otherShooter))
            {
                return;
            }
            if (otherShooter.sync.owner == plate.Owner && !otherShooter.selfDamage)
            {
                return;
            }

            if (!otherShooter.sync.IsLocalOwner())
            {
                return;
            }

            otherShooter.OnHitPlayerFX();

            _localPlayerObject.SendDamage(otherShooter.CalcDamage(), plate.transform.GetSiblingIndex());
        }

        public DamageSync[] damage_syncs;
        int sync_id;
        public virtual void SendDamage(int damage, int receiver_id){
            damage_syncs[sync_id].SendDamage(receiver_id, damage);
            sync_id = (sync_id + 1) % damage_syncs.Length;
        }
        public virtual void ReceiveDamage(int damage)
        {
            if (damage == 0 || !IsOwnerLocal() ||!CanTakeDamage())
            {
                return;
            }
            health -= damage;
        }
        public virtual bool CanTakeDamage()
        {
            return state == STATE_NORMAL || state == STATE_FROZEN;
        }
        public virtual bool CanDealDamage()
        {
            return state == STATE_NORMAL || state == STATE_INVINCIBLE;
        }

        public void ResetPlayer(){
            maxHealth = 100;
            health = maxHealth;
            team = -1001;
            state = STATE_SPECTATING;
        }

    }
}
