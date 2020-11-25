using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectable
{
    /// <summary>
    /// Resolve collect action
    /// </summary>
    /// <param name="collector">The entity picking up the collectable</param>
    /// <returns>Returns true if successfully collected</returns>
    bool Collect(GameObject collector);
}
