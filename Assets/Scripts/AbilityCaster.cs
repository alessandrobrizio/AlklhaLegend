using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AbilityCaster<Caster>
    where Caster : MonoBehaviour
{
    [System.Serializable]
    public class AbilityInfo
    {
        public Ability<Caster> ability;
        [ShowOnly] public float cooldown;
        public delegate bool CustomIsReady();
        public CustomIsReady customIsReady;
        [SerializeField] [ShowOnly] private bool isReady;
        public bool IsReady
        {
            get
            {
                CheckIsReady();
                return isReady;
            }
            set
            {
                if (isReady == value || ability == null) return;
                isReady = value;
                if (isReady)
                    OnReady.Invoke();
                else
                    OnCast.Invoke();
            }
        }
        public bool needProgress;
        public delegate float CustomProgress();
        public CustomProgress customProgress;
        private float lastProgress;

        public UnityEvent<float> OnProgress;
        public UnityEvent OnReady;
        public UnityEvent OnCast;

        public void CheckIsReady()
        {
            if (isReady || ability == null) return;
            if (needProgress)
            {
                float progress;
                if (customProgress == null)
                {
                    progress = 1 - cooldown / ability.Cooldown;
                }
                else
                {
                    progress = customProgress();
                }
                if (progress != lastProgress) OnProgress.Invoke(progress);
            }
            if (customIsReady == null)
            {
                isReady = cooldown <= 0f;
            }
            else
            {
                isReady = customIsReady();
            }
            if (isReady) OnReady.Invoke();
        }
    }

    [System.Serializable]
    public class Anchor
    {
        public string parameterName;
        public Transform transform;
        public bool IsActive(Animator animator) => animator.GetFloat(parameterName) > 0.5f;
    }

    public AbilityInfo[] abilitiesInfo;
    public Anchor[] anchors;

    private Caster caster = null;
    private Animator animator = null;
    [SerializeField] [ShowOnly] private Ability<Caster> currentAbility = null;
    private float attackAnimationDuration = 0f;
    private readonly List<GameObject> hitThisCast = new List<GameObject>();

    public Ability<Caster> CurrentAbility => currentAbility;
    public bool IsCasting => currentAbility != null;

    public void Awake(Caster caster, Animator animator)
    {
        this.caster = caster;
        this.animator = animator;
    }

    public void Update()
    {
        attackAnimationDuration -= Time.deltaTime;
        foreach (AbilityInfo abilityInfo in abilitiesInfo)
        {
            if (abilityInfo.ability != null)
            {
                abilityInfo.cooldown -= Time.deltaTime;
                abilityInfo.CheckIsReady();
            }
        }
        if (attackAnimationDuration <= 0f)
        {
            currentAbility = null;
        }
    }

    public void FixedUpdate()
    {
        if (currentAbility == null) return;

        foreach (Anchor anchor in anchors)
        {
            if (anchor.IsActive(animator))
            {
                Collider[] colliders = Physics.OverlapSphere(anchor.transform.position, currentAbility.Range);
                foreach (Collider collider in colliders)
                {
                    if (!hitThisCast.Contains(collider.gameObject))
                    {
                        currentAbility.Apply(caster, collider);
                        hitThisCast.Add(collider.gameObject);
                    }
                }
            }
        }
    }

    public bool TryCast(int index)
    {
        if (currentAbility != null
        || !abilitiesInfo[index].IsReady
        || abilitiesInfo[index].ability == null)
        {
            return false;
        }

        hitThisCast.Clear();
        currentAbility = abilitiesInfo[index].ability;
        currentAbility.Cast(caster);
        abilitiesInfo[index].cooldown = currentAbility.Cooldown;
        abilitiesInfo[index].IsReady = false;
        attackAnimationDuration = currentAbility.AttackDuration;
        return true;
    }

    public void OnDrawGizmos(Color color)
    {
        if (animator == null || currentAbility == null) return;

        Gizmos.color = color;
        foreach (var anchor in anchors)
        {
            if (anchor.IsActive(animator))
            {
                Gizmos.DrawSphere(anchor.transform.position, currentAbility.Range);
            }
        }
    }
}
