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

    // can the attacker attack?
    public bool canAttack { get; set; }

    public Attack(AttackManager attackManagerObj, string name, AttackTarget target, HealthWinStateManager healthWinStateManagerObj, bool activeOnStart, bool canAttackTarget)
    {
        healthWinStateManager = healthWinStateManagerObj;
        attackManager = attackManagerObj;

        attackName = name;
        attackActive = activeOnStart;
        attackTarget = target;

        isAttackOver = false;

        // assume yes.
        canAttack = canAttackTarget;

        // If attack to be activated on start trigger activation.
        if (attackActive)
        {
            OnAttackStart();
        }
    }

    // Abstract classes for the updates
    public abstract void OnAttackStart();
    public abstract void OnAttackUpdate(bool canAttackerAttack);

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
    public WaterAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active, bool canAttack)
        :base(attackManager, "Healing Water Swirl", target, healthWinStateManagerObj, active, canAttack)
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

    public override void OnAttackUpdate(bool canAttackerAttack)
    {
        // This is a one-shot attack so doesn't have any special recurring effect.
        return;
    }
}

public class FireAttack : Attack
{
    public FireAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active, bool canAttack)
        : base(attackManager, "Hellspawn's Fire", target, healthWinStateManagerObj, active, canAttack)
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

        if (attackTarget == AttackTarget.Enemy)
        {
            attackManager.isFireAttackActiveOnEnemy = true;
        } else
        {
            attackManager.isFireAttackActiveOnPlayer = true;
        }
    }

    private long timeSinceLastFireHit = 0;

    public override void OnAttackUpdate(bool canAttackerAttack)
    {
        this.canAttack = canAttackerAttack;
        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timeSinceLastFireHit >= Random.Range(1000,2000))
        {
            timeSinceLastFireHit = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Deal damage.
           if (this.canAttack)
            {
                this.DoDamage(this.attackTarget, 1);
            }
        }

        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - attackStartTime >= attackLength)
        {
            // Attack done.
            this.isAttackOver = true;

            if (attackTarget == AttackTarget.Enemy)
            {
                attackManager.isFireAttackActiveOnEnemy = false;
            }
            else
            {
                attackManager.isFireAttackActiveOnPlayer = false;
            }
        }
    }
}

public class EarthAttack : Attack
{
    public EarthAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active, bool canAttack)
        : base(attackManager, "Nature's Wrath", target, healthWinStateManagerObj, active, canAttack)
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
        if (this.canAttack)
        {
            this.DoDamage(this.attackTarget, Random.Range(3, 5));
        }

        // freeze whoever was hit
        if (this.attackTarget == AttackTarget.Player)
        {
            // player is being attacked freeze them.
            if (this.attackManager.isNatureAttackActiveOnEnemy)
            {
                // don't freeze again
                this.isAttackOver = true;
            } else
            {
                this.attackManager.isNatureAttackActiveOnEnemy = true;
                this.attackManager.canPlayerAttack = false;
            }
            
        }
        else
        {
            if (this.attackManager.isNatureAttackActiveOnPlayer)
            {
                // don't freeze again
                this.isAttackOver = true;
            }
            else
            {
                this.attackManager.isNatureAttackActiveOnPlayer = true;
                this.attackManager.canEnemyAttack = false;
            }
        }
    }

    private long timeSinceLastFireHit = 0;

    public override void OnAttackUpdate(bool canAttackerAttack)
    {
        this.canAttack = canAttackerAttack;
        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - attackStartTime >= attackLength)
        {
            if (this.attackTarget == AttackTarget.Player)
            {
                // player is being attacked unfreeze them.
                this.attackManager.canPlayerAttack = true;
                this.attackManager.isNatureAttackActiveOnEnemy = false;
            }
            else
            {
                this.attackManager.canEnemyAttack = true;
                this.attackManager.isNatureAttackActiveOnPlayer = false;
            }

            this.isAttackOver = true;
        }
    }
}

public class LightningAttack : Attack
{
    public LightningAttack(AttackManager attackManager, HealthWinStateManager healthWinStateManagerObj, AttackTarget target, bool active, bool canAttack)
        : base(attackManager, "Zeus' Revenge", target, healthWinStateManagerObj, active, canAttack)
    {
        this.isAttackOneshot = true;
        this.isPassiveAttack = false;
    }

    public override void OnAttackStart()
    {
        this.DoDamage(this.attackTarget, Random.Range(1, 3));
        this.isAttackOver = true;
    }

    public override void OnAttackUpdate(bool canAttackerAttack)
    {
        this.canAttack = canAttackerAttack;
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

    public bool isFireAttackActiveOnEnemy = false;
    public bool isFireAttackActiveOnPlayer = false;

    private long enemyLastAttackTime = 0;
    private int timeEnemyShouldWaitBeforeAttack;

    // audio clips
    public AudioClip fireAttackSound;
    public AudioClip waterAttackSound;
    public AudioClip lightningAttackSound;
    public AudioClip earthAttackSound;
    private AudioSource audioSource;

    // stone shields
    public GameObject emenyStoneShield;
    public GameObject playerStoneShield;

    // fire
    public GameObject enemyFire;
    public GameObject playerFire;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        enemyLastAttackTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        timeEnemyShouldWaitBeforeAttack = Random.Range(3000, 7500);
    }

    private string[] enemyAttacks = { "red_mask", "earth_mask", "lightning_mask" };

    // Unity functions.
    void Update()
    {
        //Debug.Log(activeAttacks.Count);
        for (int i = 0; i < activeAttacks.Count; i++)
        {
            // only allow each type of attack player -> enemy etc. if the player/enemy is allowed to attack on this frame.
            if (activeAttacks[i].attackTarget == AttackTarget.Enemy)
            {
                activeAttacks[i].OnAttackUpdate(canPlayerAttack);
            } else if (activeAttacks[i].attackTarget == AttackTarget.Player)
            {
                activeAttacks[i].OnAttackUpdate(canEnemyAttack);
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

        // set visibility of stones.
        emenyStoneShield.SetActive(this.isNatureAttackActiveOnEnemy);
        playerStoneShield.SetActive(this.isNatureAttackActiveOnPlayer);

        // set visibility of fire.
        enemyFire.SetActive(this.isFireAttackActiveOnEnemy);
        playerFire.SetActive(this.isFireAttackActiveOnPlayer);

        // check enemy attack.
        if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - enemyLastAttackTime >= timeEnemyShouldWaitBeforeAttack)
        {
            // find a attack
            string possibleAttack = enemyAttacks[Random.Range(0, enemyAttacks.Length)];
            switch (possibleAttack)
            {
                case "red_mask":
                    if (!canPlayerAttack) { Debug.Log("Enemy can't attack right now!"); return; }
                    activeAttacks.Add(new FireAttack(this, healthWinStateManager, AttackTarget.Player, true, canEnemyAttack));
                    audioSource.PlayOneShot(fireAttackSound, 1f);
                    break;
                case "water_mask":
                    // enemy can heal irrespective of being able to attack.
                    activeAttacks.Add(new WaterAttack(this, healthWinStateManager, AttackTarget.Enemy, true, true));
                    audioSource.PlayOneShot(waterAttackSound, 1f);
                    break;
                case "earth_mask":
                    activeAttacks.Add(new EarthAttack(this, healthWinStateManager, AttackTarget.Player, true, canEnemyAttack));
                    audioSource.PlayOneShot(earthAttackSound, 1f);
                    break;
                case "lightning_mask":
                    if (!canEnemyAttack) { Debug.Log("Enemy can't attack right now!"); return; }
                    activeAttacks.Add(new LightningAttack(this, healthWinStateManager, AttackTarget.Player, true, canEnemyAttack));
                    audioSource.PlayOneShot(lightningAttackSound, 1f);
                    break;
            }

            // get it ready for refresh.
            enemyLastAttackTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            timeEnemyShouldWaitBeforeAttack = Random.Range(3000, 7500);
        }

    }


    public void OnAttack(string spriteName)
    {
        Debug.Log(spriteName);
        switch(spriteName)
        {
            case "red_mask":
                if (!canPlayerAttack) { Debug.Log("Player can't attack right now!"); return; }
                activeAttacks.Add(new FireAttack(this, healthWinStateManager, AttackTarget.Enemy, true, canPlayerAttack));
                audioSource.PlayOneShot(fireAttackSound, 1f);
                break;
            case "water_mask":
                // player can heal irrespective of being able to attack.
                activeAttacks.Add(new WaterAttack(this, healthWinStateManager, AttackTarget.Player, true, true));
                audioSource.PlayOneShot(waterAttackSound, 1f);
                break;
            case "earth_mask":
                activeAttacks.Add(new EarthAttack(this, healthWinStateManager, AttackTarget.Enemy, true, canPlayerAttack));
                audioSource.PlayOneShot(earthAttackSound, 1f);
                break;
            case "lightning_mask":
                if (!canPlayerAttack) { Debug.Log("Player can't attack right now!"); return; }
                activeAttacks.Add(new LightningAttack(this, healthWinStateManager, AttackTarget.Enemy, true, canPlayerAttack));
                audioSource.PlayOneShot(lightningAttackSound, 1f);
                break;
        }
    }
}
