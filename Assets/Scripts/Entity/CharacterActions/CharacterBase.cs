﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent (typeof (Orientation))]
public class CharacterBase : MonoBehaviour
{
    public bool IsAutonomous { get { return m_isAutonomous; } private set { m_isAutonomous = value; } }
    private bool m_isAutonomous = true;

    public bool AutoOrientSprite = true;
    public string IdleAnimation = "idle";
    public string WalkAnimation = "walk";
    public string HurtAnimation = "hit";
    public string AirAnimation = "air";
    public string JumpAnimation = "jump";
    public string LandAnimation = "land";
    public string SprintAnimation = "sprint";

    CharacterController m_controller;

    private AnimatorSprite m_anim;
    private Attackable m_attackable;
    private Orientation m_orient;
    private ActionInfo m_currentAction = null;
  

    private float m_animationSpeed = 2f;

    private Dictionary<HitboxInfo, float> m_queuedHitboxes = new Dictionary<HitboxInfo, float>();
    private Dictionary<ProjectileInfo, float> m_queuedProjectiles = new Dictionary<ProjectileInfo, float>();
    private bool m_pauseAnim = false;

    private bool m_haveMovedOnGround = false;
    private bool m_hitStateIsGuard = false;

    private string m_actionAnim = "";
    //private List<UIBarInfo> m_uiBars;

    //private Dictionary<ProjectileInfo, float> m_queuedProjectiles = new Dictionary<ProjectileInfo, float>();

    [HideInInspector]
    public AudioClip AttackSound;

    [HideInInspector]
    public float StunTime = 0.0f;

    //public Vector3 movement;
    // Start is called before the first frame update
    internal void Awake()
    {
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }
    
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_attackable = GetComponent<Attackable>();
        m_anim = GetComponent<AnimatorSprite>(); 
    }

    // Update is called once per frame
    internal void Update()
    {
        activateStunIfDead();
        if (progressStun())
            return;
        progressAnimation();
        if (progressAction())
            return;
        if (canAct()) {
            
        }
    }
    public void progressAnimation()
    {
        if (m_actionAnim != "" && m_anim != null)
            m_anim.Play(m_actionAnim);
        else if (!m_pauseAnim && m_anim != null)
            standardAnimation();

    }
    public void standardAnimation()
    {
        if (!m_controller.isGrounded)
        {
            m_haveMovedOnGround = false;
            if (GetComponent<MovementBase>().TrueAverageVelocity.z > 0.1f)
            {
                if (GetComponent<MovementBase>().TrueAverageVelocity.y > 0f) {
                    m_anim.Play(new string[] { JumpAnimation + "_back", AirAnimation + "_back", JumpAnimation, AirAnimation, IdleAnimation });
                } else {
                    m_anim.Play(new string[] { AirAnimation + "_back", AirAnimation, IdleAnimation });
                }
            }
            else
            {
                if (GetComponent<MovementBase>().TrueAverageVelocity.y > 0f)
                {
                    m_anim.Play(new string[] { JumpAnimation, AirAnimation, IdleAnimation });
                }
                else
                {
                    m_anim.Play(new string[] { AirAnimation, IdleAnimation });
                }
            }
            
        }
        else
        {
            if (GetComponent<MovementBase>().IsAttemptingMovement() && GetComponent<MovementBase>().TrueAverageVelocity.magnitude > 0.01f)
            {
                if (GetComponent<MovementBase>().IsSprinting)
                {
                    m_anim.Play(new string[] { SprintAnimation, WalkAnimation }, false);
                } else
                {
                    if (GetComponent<MovementBase>().TrueAverageVelocity.z > 0.1f)
                    {
                        //m_anim.Play(WalkAnimation + "_back");
                        m_anim.Play(new string[] { WalkAnimation + "_back", WalkAnimation }, false);
                    }  else
                    {
                        m_anim.Play(WalkAnimation);
                    }
                }
                m_haveMovedOnGround = true;
            }
            else
            {
                 m_anim.Play(IdleAnimation);
            }
        }
    }

    // -- STUN --


    public void RegisterStun(float st, bool defaultStun, HitInfo hi, bool guard = false)
    {

        if (m_currentAction != null)
        {
            m_currentAction.OnInterrupt(StunTime, defaultStun, hi);
        }
        if (defaultStun)
        {
            startStunState(st, guard);
        }
    }

    private bool progressStun()
    {
        if (StunTime <= 0.0f)
            return false;
        if (m_anim != null)
        {
            if (m_hitStateIsGuard)
            {
                m_anim.Play(new string[] { HurtAnimation }, AutoOrientSprite);
            }
            else
            {
                m_anim.Play(HurtAnimation, AutoOrientSprite);
            }
        }
        StunTime -= ScaledTime.deltaTime;
        if (m_currentAction != null)
            m_currentAction = null;
        if (StunTime <= 0.0f && m_attackable != null && m_attackable.Alive)
            EndStun();
        return true;
    }

    private void activateStunIfDead()
    {
        if (m_attackable == null || m_attackable.Alive)
            return;
        startStunState(3.0f);
    }
    public void EndStun()
    {
        if (m_attackable == null || m_attackable.Alive)
        {
            setAutonomous(true);
            StunTime = 0.0f;
        }
    }
    public void SetPause(bool p)
    {
        m_pauseAnim = p;
    }
    private void startStunState(float st, bool guard = false)
    {
        //Debug.Log ("Starting Hit State with Stun: "+ st);
        EndAction();
        StunTime = st;
        m_hitStateIsGuard = guard;
        setAutonomous(false);
    }

    // --- ACTIONS

    
    private bool progressAction()
    {
        if (m_currentAction == default(ActionInfo))
            return false;
        m_currentAction.Progress();
        if (m_currentAction == null)
            return false;
        OnActionProgressed(m_currentAction.CurrentState());
        return true;
    }

    public void OnActionProgressed(AttackState state)
    {
        switch (state)
        {
            case AttackState.STARTUP:
                OnActionStart();
                break;
            case AttackState.RECOVERY:
                OnActionRecover();
                break;
            case AttackState.INACTIVE:
                EndAction();
                break;
        }
    }

    private void OnActionStart()
    {
        /*if (m_currentAction.m_SoundInfo.AttackFX)
			AddEffect(m_currentAction.m_SoundInfo.AttackFX, m_currentAction.m_AttackAnimInfo.RecoveryTime + 0.2f);*/
        //m_anim.Play(m_currentAction.m_AttackAnimInfo.StartUpAnimation, true);
        m_actionAnim = m_currentAction.m_AttackAnimInfo.StartUpAnimation;
        if (m_anim != null)
            m_anim.SetSpeed(m_currentAction.m_AttackAnimInfo.AnimSpeed * m_animationSpeed);
        setAutonomous(false);
        //Debug.Log("On action start");
    }

    private void OnActionRecover()
    {
        //m_anim.Play(m_currentAction.m_AttackAnimInfo.RecoveryAnimation, true);
        m_actionAnim = m_currentAction.m_AttackAnimInfo.RecoveryAnimation;
        //Debug.Log("On action recover");
    }

    public void SkipActionToEnd()
    {
        if (m_currentAction != null)
        {
            m_currentAction.OnInterrupt(0, true, new HitInfo());
        }
        EndAction();
    }

    public void EndAction()
    {
        setAutonomous(true);
        m_currentAction = null;
        if (m_anim != null)
        {
            m_anim.SetSpeed(1.0f);
        }
        
        m_actionAnim = "";
        GetComponent<MovementBase>().DecelerateInAir = true;
    }

    bool canAct()
    {
        return (StunTime <= 0 && m_isAutonomous);
    }

    private void setAutonomous(bool autonomous)
    {
        if (GetComponent<MovementBase>() != null)
            GetComponent<MovementBase>().SetCanMove( autonomous);
        m_isAutonomous = autonomous;
    }
    //public void AddUIBar(UIBarInfo uib)
    //{
    //    if (m_uiBars == null)
    //    {
    //        m_uiBars = new List<UIBarInfo>();
    //    }
    //    m_uiBars.Add(uib);
    //}
    //public void ClearUIBars()
    //{
    //    if (m_uiBars != null)
    //    {
    //        m_uiBars.Clear();
    //    }
    //}
    //public void RemoveUIBar(string id)
    //{
    //    if (m_uiBars == null)
    //        m_uiBars = new List<UIBarInfo>();
    //    List<UIBarInfo> m_newList = new List<UIBarInfo>();
    //    foreach (UIBarInfo uib in m_uiBars)
    //    {
    //        if (uib.id != id)
    //            m_newList.Add(uib);

    //    }
    //    m_uiBars = m_newList;
    //}

    //public void DrawAllUIBars(GUIManager guiManager)
    //{
    //    if (m_uiBars == null)
    //        m_uiBars = new List<UIBarInfo>();
    //    List<UIBarInfo> m_newList = new List<UIBarInfo>();
    //    foreach (UIBarInfo uib in m_uiBars)
    //    {
    //        guiManager.AddUIBar(uib);
    //    }
    //    if (GetComponent<InventoryContainer>() != null)
    //    {
    //        Debug.Log("Adding a inventory container preview");
    //        foreach( Vector2 v in GetComponent<InventoryContainer>().eqpSlotInfo.Keys)
    //        {
    //            Debug.Log("Adding container vector: " + v);
    //            guiManager.AddEquipmentPreviewIcon(GetComponent<InventoryContainer>(), v);
    //        }
    //    }
    //}
    //public int GetNumUIBars()
    //{
    //    if (m_uiBars == null)
    //        return 0;
    //    return m_uiBars.Count;
    //}

    public void RegisterHit(GameObject otherObj, HitInfo hi, HitResult hr)
    {
        //Debug.Log ("Collision: " + this + " " + otherObj);
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnHitConfirm(hi, otherObj, hr));
        //Debug.Log ("Registering hit with: " + otherObj);
        //if (otherObj.GetComponent<Attackable>() != null)
        //{
        //    m_hitTargets[otherObj.GetComponent<Attackable>()] = hi;
        //}
        if (m_currentAction != null)
            m_currentAction.OnHitConfirm(otherObj, hi, hr);
    }
    public void TryAction(string actionInfoName, OnRegisterHit hitCallback = null, InputKey holdInput = InputKey.Fire)
    {
        foreach (ActionInfo ai in GetComponents<ActionInfo>())
        {
            if (ai.name == actionInfoName)
            {
                TryAction(ai, hitCallback, holdInput);
            }
        }
    }
    public void TryAction(ActionInfo ai, OnRegisterHit hitCallback = null,InputKey holdInput = InputKey.Fire)
    {
        if (ai == null)
            return;
        if (!canAct())
            return;
        m_currentAction = ai;
        m_currentAction.m_useButton = holdInput;
        m_currentAction.ClearCallback();
        m_currentAction.AddCallBack(hitCallback);
        m_currentAction.AddCallBack(RegisterHit);
        setAutonomous(ai.m_MovementInfo.CanMove);
        if (ai.m_MovementInfo.CanMove)
        {
            float time = Mathf.Max(0.1f, ai.m_AttackAnimInfo.StartUpTime + ai.m_AttackAnimInfo.RecoveryTime);
            GetComponent<MovementBase>().AddModifier(ai.AttackName, ai.m_MovementInfo.MovementModifier, time);
        }
            
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnAttack(m_currentAction));
        GetComponent<MovementBase>().DecelerateInAir = false;
        if (m_currentAction != null)
        {
            m_currentAction.ResetAndProgress();
        }
    }

    public List<ActionInfo> GetValidActions(Vector3 otherPos)
    {
        List<ActionInfo> allAttacks = new List<ActionInfo>();
        
        foreach (ActionInfo ainfo in GetAllActions())
        {
            float dir = GetComponent<Orientation>().FacingLeft ? -1f : 1f;
            float xDiff = Mathf.Abs(transform.position.x + (dir * ainfo.m_AIInfo.AIPredictionOffset.x) - otherPos.x);
            float yDiff = Mathf.Abs(transform.position.y + ainfo.m_AIInfo.AIPredictionOffset.y - otherPos.y);
            if (ainfo.IsInActiveZone(otherPos))
            {
                allAttacks.Add(ainfo);
            }
        }
        return allAttacks;
    }

    public List<ActionInfo> GetAllActions()
    {
        List<ActionInfo> allAttacks = new List<ActionInfo>();

        foreach (ActionInfo ai in GetComponents<ActionInfo>())
        {
            allAttacks.Add(ai);
        }
        foreach (ActionInfo ai in GetComponentsInChildren<ActionInfo>())
        {
            allAttacks.Add(ai);
        }
        return allAttacks;
    }

    private void storeData(CharData d)
    {
        d.SetBool("IsAutonomous",IsAutonomous);
    }

    private void loadData(CharData d)
    {
        IsAutonomous = d.GetBool("IsAutonomous",true);
    }

    private void broadCastActionStart()
    {
        GetComponent<Observable>()?.BroadcastToObserver(new AIEVObservedAction(gameObject,m_currentAction));
    }
}
