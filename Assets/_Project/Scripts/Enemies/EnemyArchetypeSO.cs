using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Archetype", fileName = "EnemyArchetype")]
public class EnemyArchetypeSO : ScriptableObject
{
    public int BaseDamage = 10;
    public float BaseMoveSpeed = 3.5f;
    public float AttackRange = 1.8f;
    public float AttackCooldown = 1.2f;
    public float TurnSpeed = 540f;

    public float WindupTime = 0.20f;
    public float HitRadius = 0.75f;
    public float AttackAngleTolerance = 60f;

    public AnimationCurve DamageMultByLevel = AnimationCurve.Linear(1, 1f, 3, 1.5f);
    public AnimationCurve SpeedMultByLevel = AnimationCurve.Linear(1, 1f, 3, 1.2f);
    public AnimationCurve CooldownMultByLevel = AnimationCurve.Linear(1, 1f, 3, 0.85f);

    public void EvaluateAtLevel(int level, out int damage, out float moveSpeed, out float cooldown)
    {
        float dmg = Mathf.Max(0.01f, DamageMultByLevel.Evaluate(level));
        float spd = Mathf.Max(0.01f, SpeedMultByLevel.Evaluate(level));
        float cd = Mathf.Max(0.01f, CooldownMultByLevel.Evaluate(level));

        damage = Mathf.Max(1, Mathf.RoundToInt(BaseDamage * dmg));
        moveSpeed = Mathf.Max(0.1f, BaseMoveSpeed * spd);
        cooldown = Mathf.Max(0.1f, AttackCooldown * cd);

    }

}
