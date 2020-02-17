using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[System.Serializable]
public class FootstepInfo
{
    public bool PlayFootsteps = false;
    public AudioClip FootStepClip = null;
    public float FootStepVolume = 0.3f;
    public float FootStepPitchVariation = 0.25f;
    public float FootStepVolumeVariation = 0.0f;
    public float FootstepInterval = 0.75f;
}

public class MovementBase : MonoBehaviour
{
    public bool IsPlayerControl = false;
    private bool m_canMove = true;
    [HideInInspector]
    public bool CanJump = true;
    [HideInInspector]
    public int MidAirJumps = 0;

    private BasicPhysics m_physics;
    private Orientation m_orient;

    [SerializeField]
    private float m_moveSpeed = 8.0f;
    public float MoveSpeed { get { return m_moveSpeed; } private set { m_moveSpeed = value; } }

    private Dictionary<string, float> m_movementModifier = new Dictionary<string,float>();
    private Dictionary<string, float> m_modifierDuration = new Dictionary<string, float>();

    //[SerializeField]
    private float m_jumpHeight = 4.0f;
    //[SerializeField]
    private bool VariableJumpHeight = false;
    public float JumpHeight { get { return m_jumpHeight; } private set { m_jumpHeight = value; } }
    //[SerializeField]
    private float m_sprintRatio = 1.5f;
    //public float SprintRatio { get { return m_sprintRatio; } private set { m_sprintRatio = value; } }
    //[SerializeField]
    private bool m_currentlySprinting = false;
    public bool IsSprinting { get { return m_currentlySprinting; } private set { m_currentlySprinting = value; } }

    public Vector3 InputMove { get { return m_inputMove; } private set { m_inputMove = value; } }
    [HideInInspector]
    public bool DecelerateInAir = true;

    // Movement tracking
    protected bool m_jumpHold;
    protected bool m_jumpDown;
    protected Vector3 m_inputMove;
    private Vector3 m_jumpVector;
    private float m_jumpVelocity;
    private Vector3 m_velocity;
    private int m_currentAirJumps;

    private float m_lastJump = 0.0f;
    private bool m_variableJumpApplied = false;
    private float m_sinceStep = 0f;
    private float m_accelerationTimeX = .1f;
    private float m_accelerationTimeZ = .1f;


    private float LastCalculatedTime = 0;
    private Vector3 lastPos;
    private Vector3 m_trueAverageVelocity;
    public Vector3 TrueAverageVelocity { get { return m_trueAverageVelocity; } private set { m_trueAverageVelocity = value; } }

    private InputPacket m_extraInputs;

    private const float VEL_CALC_INTERVAL = 0.1f;
    private const float MIN_JUMP_INTERVAL = 0.2f;
    private const float SMOOTH_TIME = .1f;

    private List<InputHandler> m_InputHandlers;
    private List<AIBase> m_aibase;

    public InputPacket LastInput { get { return m_lastInput; } private set { m_lastInput = value; } }
    private InputPacket m_lastInput;
    internal void Awake()
    {
        m_physics = GetComponent<BasicPhysics>();
        m_lastInput = new InputPacket();
        m_orient = GetComponent<Orientation>();
        m_trueAverageVelocity = new Vector3();
        m_aibase = new List<AIBase>(GetComponents<AIBase>());
        m_InputHandlers = new List<InputHandler>(GetComponents<InputHandler>());
        lastPos = transform.position;
        if (CanJump)
            SetJumpHeight(JumpHeight);
        if (GetComponent<PersistentItem>() != null)
            GetComponent<PersistentItem>().InitializeSaveLoadFuncs(storeData, loadData);
    }

    // Start is called before the first frame update
    void Start()
    {
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnControllableChange(IsPlayerControl));
    }

    internal void Update()
    {
        InputPacket ip = new InputPacket();
        if (m_canMove)
        {
            if (IsPlayerControl)
            {
                ip.MatchPlayerInput();
            }
            else
            {
                ip = npcMovement();
            }
        }
        
        moveSmoothly();
        currentPlayerControl(ip);
        foreach (InputHandler ot in m_InputHandlers)
            ot.HandleInput(ip);

        /*if (m_FootStepInfo.PlayFootsteps)
            playStepSounds(); */

        if (m_inputMove != m_lastInput.movementInput) {
            updateAverageVelocity();
        }
        m_lastInput = ip;

        resetJumps();
        updateAverageVelocity();
    }
    internal InputPacket npcMovement()
    {
        InputPacket ip = new InputPacket();
        foreach (AIBase aib in m_aibase)
            ip.Combine(aib.AITemplate());
        return ip;
    }
     
    public void updateAverageVelocity()
    {
        if (Time.timeSinceLevelLoad - LastCalculatedTime > VEL_CALC_INTERVAL)
        {
            Vector3 diff = transform.position - lastPos;
            m_trueAverageVelocity = diff / (Time.timeSinceLevelLoad - LastCalculatedTime);
            lastPos = transform.position;
            LastCalculatedTime = Time.timeSinceLevelLoad;
        }
    }
    //public void SetTargetPoint(Vector3 target, float tolerance = 0.5f)
    //{
    //    m_currentControl.SetTarget(target, tolerance);
    //}
    //public void FacePoint2D(Vector3 target)
    //{
    //    m_currentControl.FacePoint(target);
    //    //GetComponent<Orientation>().OrientToPoint2D(target);
    //}
    public void SetJumpHeight(float jumpHeight)
    {
        m_jumpVector.y = (-m_physics.GravityForce * (12f * Mathf.Sqrt(jumpHeight))) + 3f;
        m_jumpHeight = jumpHeight;
    }

    /*private void playStepSounds()
    {
        if (m_inputMove.x != 0f && m_physics.OnGround())
        {
            m_sinceStep += Time.deltaTime;
            if (m_sinceStep > m_FootStepInfo.FootstepInterval)
            {
                m_sinceStep = 0f;
                FindObjectOfType<AudioManager>().PlayClipAtPos(
                    (m_FootStepInfo.FootStepClip == null) ? FXBody.Instance.SFXFootstep : m_FootStepInfo.FootStepClip, transform.position,
                    m_FootStepInfo.FootStepVolume, m_FootStepInfo.FootStepVolumeVariation, m_FootStepInfo.FootStepPitchVariation);
            }
}
        else
        {
            m_sinceStep = m_FootStepInfo.FootstepInterval;
        }
    }*/

    private void resetJumps()
    {
        if (m_physics.OnGround())
            m_currentAirJumps = MidAirJumps;
    }

    private void moveSmoothly()
    {
        Vector2 input = new Vector2(m_inputMove.x, m_inputMove.z);
        float realMoveSpeed = MoveSpeed;
        foreach (float f in m_modifierDuration.Values)
            realMoveSpeed *= f;
        Vector3 targetVel = new Vector3(input.normalized.x * realMoveSpeed, 0, input.normalized.y * realMoveSpeed);
        if (!m_physics.OnGround() && !DecelerateInAir)
        {
            if (m_velocity.x > 0.1f && m_inputMove.x >= 0f)
            {
                targetVel.x = Mathf.Sign(m_velocity.x) * realMoveSpeed;
            } else if (m_velocity.x < -0.1f && m_inputMove.x <= 0f)
            {
                targetVel.x = Mathf.Sign(m_velocity.x) * realMoveSpeed;
            }
        }
        if (m_currentlySprinting)
            m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVel.x * m_sprintRatio, ref m_accelerationTimeX, SMOOTH_TIME);
        else
            m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVel.x, ref m_accelerationTimeX, SMOOTH_TIME);
        m_velocity.z = Mathf.SmoothDamp(m_velocity.z, targetVel.z, ref m_accelerationTimeZ, SMOOTH_TIME);
        m_physics.InputMove(m_velocity, m_inputMove);
    }
    
    private void currentPlayerControl(InputPacket ip)
    {        
        if (ip.movementInput.magnitude > 0.01f && m_orient != null)
        {
             m_orient.FaceVector(ip.movementInput);
            //m_orient.SetDirection(d);
        }
        if (ip.leftMouse || ip.leftMousePress ||
            ip.rightMouse || ip.rightMousePress ||
            ip.middleMouse || ip.middleMousePress)
        {
            m_orient.FacePoint(ip.MousePointWorld);
        }
        m_jumpDown = ip.jumpDown;
        m_jumpHold = ip.jump;
        if (CanJump)
            JumpMovement();
        //if (ip.inventory)
        //{
        //    m_eqp.ToggleDisplay();
        //}
        //if (ip.interact) {
        //    m_eqp.CloseContainer();
        //    //GetComponent<Interactor>().OnAttemptInteract();
        //}
        m_inputMove = ip.movementInput;
    }

    protected void JumpMovement()
    {
        if (m_jumpDown)
        {
            AttemptJump();
        }
        float dt = (Time.timeSinceLevelLoad - m_lastJump);
        if (VariableJumpHeight && dt > 0.1f && dt < 0.2f && m_jumpHold && !m_variableJumpApplied)
        {
            m_variableJumpApplied = true;
            ApplyJumpVector(new Vector2(1f, 0.35f));
        }
    }
    private void AttemptJump()
    {
        //ExecuteEvents.Execute<ICustomMessageTarget>(gameObject, null, (x, y) => x.OnJump());
        if (!CanBasicJump())
            return;
        
        if (VariableJumpHeight)
        {
            ApplyJumpVector(new Vector2(1f, 0.8f));
        }
        else
        {
            ApplyJumpVector(new Vector2(1f, 1f));
        }

        //FindObjectOfType<AudioManager>().PlayClipAtPos(FXBody.Instance.SFXJump, transform.position, 0.3f, 0f, 0.25f);
        m_lastJump = Time.timeSinceLevelLoad;
        m_variableJumpApplied = false;

        //m_physics.AddArtificialVelocity(new Vector3(0f, -0.75f * m_physics.Velocity().y,0f));
        if (!m_physics.OnGround())
            m_currentAirJumps -= 1;
    }
    public bool CanBasicJump()
    {
        if ((Time.timeSinceLevelLoad - m_lastJump) < MIN_JUMP_INTERVAL)
            return false;

        if (!m_physics.OnGround() && m_currentAirJumps <= 0)
            return false;

        return true;
    }
    public bool IsAttemptingMovement()
    {
        return m_inputMove.x != 0f || m_inputMove.z != 0f;
    }
    public void ApplyJumpVector(Vector2 scale)
    {
        float y = m_jumpVector.y;
        Vector2 jv = new Vector2(m_jumpVector.x, y); //- Mathf.Max (0, m_physics.TrueVelocity.y / Time.deltaTime));
        jv.x *= scale.x;
        jv.y *= scale.y;
        m_physics.AddSelfForce(jv, 0f);
    }

    public void AddModifier(string key, float Modifier,float duration)
    {
        if (m_modifierDuration.ContainsKey(key))
        {
            m_movementModifier.Remove(key);
            m_modifierDuration.Remove(key);
        }
        m_movementModifier.Add(key, Modifier);
        m_modifierDuration.Add(key, Time.timeSinceLevelLoad + duration);
    }

    private void processModifiers()
    {
        float time = Time.timeSinceLevelLoad;
        List<string> toRemove = new List<string>();
        foreach (string s in m_modifierDuration.Keys)
        {
            if (time <= m_modifierDuration[s])
                toRemove.Add(s);
        }
        foreach (string s in toRemove)
        {
            m_modifierDuration.Remove(s);
            m_movementModifier.Remove(s);
        }
    }
    private void storeData(CharData d)
    {
        d.SetBool("IsPlayerControl", IsPlayerControl);
        d.SetBool("CanJump", CanJump);
        d.SetInt("MidAirJumps", MidAirJumps);
    }

    private void loadData(CharData d)
    {
        IsPlayerControl = d.GetBool("IsPlayerControl");
        CanJump = d.GetBool("CanJump");
        MidAirJumps = d.GetInt("MidAirJumps");
    }
    public void SetCanMove (bool canMove)
    { m_canMove = canMove; }


}