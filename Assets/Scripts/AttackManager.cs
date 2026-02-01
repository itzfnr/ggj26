using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DateTimeOffset = System.DateTimeOffset; // Only grab the time tool

// AttackManager aims:
// - Create "do" functions to trigger a specific attack 
//
// Attacks are from the following:
// - Water: Heals player between 10-20 points, has 25% chance of removing any debuffs from player (eg. fire, lightning).
// - Earth (Nature): high damage (3-5pt) - grants player "Stone Shield" as a shield against enemy attacks for 5-8 seconds (RNG)
// - Fire: low damage but applied every second to enemy for 3-5 seconds (RNG)
// - Lightning: medium damage (1-3pt) - has no other effect.

public enum AttackTarget
{
    Player,
    Enemy
}

public abstract class Attack
{
    // health win state manager passthrough.
    public HealthWinStateManager healthWinStateManager;

    // display name for attack
    public string attackName { get; set; }
    public bool attackActive { get; set; }
    public AttackTarget attackTarget { get; set; }

    // set to true if attack is "one-shot" (ie. it should only be done once)
    public bool isAttackOneshot { get; set; }
    
    // if not oneshot then what is the refresh time?
    public bool attackRefreshTime { get; set; }

    // if true, deemed as a "passive attack" which should reduce RNG rate for enemy to get
    // you don't want the enemy constantly healing themselves
    public bool isPassiveAttack { get; set; }

    // if true, attack deemed over for GC.
    public bool isAttackOver { get; set; }

    // attack manager reference
    public AttackManager attackManager { get; set; }

    public Attack(AttackManager attackManagerObj, string name, AttackTarget target, HealthWinStateManager healthWinStateManagerObj, bool activeOnStart)
    {
        healthWinStateManager = healthWinStateManagerObj;
        attackManager = attackManagerObj;

        attackName = name;
        attackActive = activeOnStart;
        attackTarget = target;

        isAttackOver = false;

        // If attack to be activated on start trigger activation.
        if (attackActive)
        {
            OnAttackStart();
        }
    }

    // Abstract classes for the updates
    public abstract void OnAttackStart();
    public abstract void OnAttackUpdate();

    // Function to deal damage based on target
    public int DoDamage(AttackTarget target, int damagePoints)
    {
        if (target == AttackTarget.Player)
        {
            return this.healthWinStateManager.DealDamageToPlayer(damagePoints);
        }
        else
        {
            return this.healthWinStateManager.DealDamageToEnemy(damagePoints);
        }
    }

    // Function to heal based on target
    public int DoHealing(AttackTarget target, int healPoints)
    {
        if (target == AttackTarget.Player)
        {
            return this.healthWinStateManager.HealPlayer(healPoints);
        }
        else
        {
            return this.healthWinStateManager.HealEnemy(healPoints);
        }
    }
}

public class WaterAttack : Attack
{
    public WaterAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active)
        :base(attackManager, "Healing Water Swirl", target, healthWinStateManagerObj, active)
    {
        this.isAttackOneshot = true;
        this.isPassiveAttack = true;
    }

    public override void OnAttackStart()
    {
        if (this.attackTarget == AttackTarget.Player)
        {
            this.healthWinStateManager.HealPlayer(Random.Range(3, 8));
        } else
        {
            this.healthWinStateManager.HealEnemy(Random.Range(3, 8));
        }

        this.isAttackOver = true;
    }

    public override void OnAttackUpdate()
    {
        // This is a one-shot attack so doesn't have any special recurring effect.
        return;
    }
}

public class FireAttack : Attack
{
    public FireAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active)
        : base(attackManager, "Hellspawn's Fire", target, healthWinStateManagerObj, active)
    {
        this.isAttackOneshot = false;
        this.isPassiveAttack = false;
        
    }

    private int attackLength;
    private long attackStartTime;

    public override void OnAttackStart()
    {
        // TODO: give whoever their fire effect.
        attackLength = Random.Range(3000, 5000);
        attackStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long timeSinceLastFireHit = 0;

    public override void OnAttackUpdate()
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timeSinceLastFireHit >= Random.Range(1000,2000))
        {
            timeSinceLastFireHit = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Deal damage.
            this.DoDamage(this.attackTarget, 1);
        }

        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - attackStartTime >= attackLength)
        {
            // Attack done.
            this.isAttackOver = true;
        }
    }
}

public class EarthAttack : Attack
{
    public EarthAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active)
        : base(attackManager, "Nature's Wrath", target, healthWinStateManagerObj, active)
    {
        this.isAttackOneshot = false;
        this.isPassiveAttack = false;

    }

    private int attackLength;
    private long attackStartTime;

    public override void OnAttackStart()
    {
        // TODO: give whoever their fire effect.
        attackLength = Random.Range(5000, 8000);
        attackStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // do immediete damage between 3-5pt.
        this.DoDamage(this.attackTarget, Random.Range(3,5));

        // freeze whoever was hit
        if (this.attackTarget == AttackTarget.Player)
        {
            // player is being attacked freeze them.
            if (this.attackManager.isNatureAttackActiveOnPlayer)
            {
                // don't freeze again
                this.isAttackOver = true;
            } else
            {
                this.attackManager.isNatureAttackActiveOnPlayer = true;
                this.attackManager.canPlayerAttack = false;
            }
            
        }
        else
        {
            if (this.attackManager.isNatureAttackActiveOnEnemy)
            {
                // don't freeze again
                this.isAttackOver = true;
            }
            else
            {
                this.attackManager.isNatureAttackActiveOnEnemy = true;
                this.attackManager.canEnemyAttack = false;
            }
        }
    }

    private long timeSinceLastFireHit = 0;

    public override void OnAttackUpdate()
    {
        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - attackStartTime >= attackLength)
        {
            if (this.attackTarget == AttackTarget.Player)
            {
                // player is being attacked unfreeze them.
                this.attackManager.canPlayerAttack = true;
            }
            else
            {
                this.attackManager.canEnemyAttack = true;
            }

            this.isAttackOver = true;
        }
    }
}

public class LightningAttack : Attack
{
    public LightningAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active)
        : base(attackManager, "Zeus' Revenge", target, healthWinStateManagerObj, active)
    {
        this.isAttackOneshot = true;
        this.isPassiveAttack = false;
    }

    public override void OnAttackStart()
    {
        this.DoDamage(this.attackTarget, Random.Range(1, 3));
        this.isAttackOver = true;
    }

    public override void OnAttackUpdate()
    {
        // This is a one-shot attack so doesn't have any special recurring effect.
        return;
    }
}

public class AttackManager : MonoBehaviour
{

    // Public variable for the HealthWinStateManager - so we can deal damage and such.
    public HealthWinStateManager healthWinStateManager;

    // active attacks list
    public List<Attack> activeAttacks = new List<Attack>();

    // if set to false, player cannot attack right now.
    public bool canPlayerAttack = true;
    public bool canEnemyAttack = true;

    public bool isNatureAttackActiveOnEnemy = false;
    public bool isNatureAttackActiveOnPlayer = false;

    private long enemyLastAttackTime = 0;
    private int timeEnemyShouldWaitBeforeAttack;

    void DoWaterDamage(AttackTarget target)
    {
        WaterAttack newAttack = new WaterAttack(this, healthWinStateManager, target, true);
        activeAttacks.Add(newAttack);
    }

    void DoEarthDamage()
    {

    }

    void DoFireDamage(AttackTarget target)
    {
        FireAttack newAttack = new FireAttack(this, healthWinStateManager, target, true);
        activeAttacks.Add(newAttack);
    }

    void DoLightningDamage()
    {

    }

    void Start()
    {
       
    }

    // Unity functions.
    void Update()
    {
        //Debug.Log(activeAttacks.Count);
        for (int i = 0; i < activeAttacks.Count; i++)
        {
            // only allow each type of attack player -> enemy etc. if the player/enemy is allowed to attack on this frame.
            if (activeAttacks[i].attackTarget == AttackTarget.Enemy && canPlayerAttack)
            {
                activeAttacks[i].OnAttackUpdate();
            } else if (activeAttacks[i].attackTarget == AttackTarget.Player && canEnemyAttack)
            {
                activeAttacks[i].OnAttackUpdate();
            }
            
        }

        // Loop again for clearing the oneshots and finished attacks.
        for (int i = 0; i < activeAttacks.Count; i++)
        {
            if (activeAttacks[i].isAttackOneshot || activeAttacks[i].isAttackOver)
            {
                // remove.
                activeAttacks.Remove(activeAttacks[i]);
            }
        }
    }

    
    public void OnAttack(string spriteName)
    {
        Debug.Log(spriteName);
        switch(spriteName)
        {
            case "red_mask":
                if (!canPlayerAttack) { Debug.Log("Player can't attack right now!"); return; }
                activeAttacks.Add(new FireAttack(this, healthWinStateManager, AttackTarget.Enemy, true));
                break;
            case "water_mask":
                // player can heal irrespective of being able to attack.
                activeAttacks.Add(new WaterAttack(this, healthWinStateManager, AttackTarget.Player, true));
                break;
            case "earth_mask":
                if (!canPlayerAttack) { Debug.Log("Player can't attack right now!"); return; }
                activeAttacks.Add(new EarthAttack(this, healthWinStateManager, AttackTarget.Enemy, true));
                break;
            case "lightning_mask":
                if (!canPlayerAttack) { Debug.Log("Player can't attack right now!"); return; }
                activeAttacks.Add(new LightningAttack(this, healthWinStateManager, AttackTarget.Enemy, true));
                break;
        }
    }
}
