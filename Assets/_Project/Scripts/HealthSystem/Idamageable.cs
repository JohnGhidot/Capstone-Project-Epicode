using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    int CurrentHP { get; }
    int MaxhHP { get; }
    bool IsDead { get; }
    void TakeDamage(int amount, GameObject source);
}