using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    [SerializeField] private float integrity = 1f;

    //TODO temporary (use Damage method instead?)
    public float Integrity
    {
        get => integrity;
        set
        {
            integrity = value;
            if (integrity <= 0f)
            {
                RaiseGameOver();
            }
        }
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(false);
    }
}
