using UnityEngine;
using System.Collections;

public class Tune_AttackBackwards : Tune_Attack
{
    /// <summary>
    /// Gets the position where the tune object will spawn at.
    /// </summary>
    /// <returns>The position where the tune object will spawn at.</returns>
    protected override Vector3 GetSpawnPosition ()
    {
        return ownerTransform.position + -ownerTransform.forward * 2f;
    }
}