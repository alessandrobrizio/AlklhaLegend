using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : ScriptableObject, ICollectable
{
    /// <summary>
    /// Apply PowerUp effect
    /// </summary>
    /// <param name="collector">The entity picking up the PowerUp</param>
    /// <inheritdoc/>
    public abstract bool Collect(GameObject collector);
}
