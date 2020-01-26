using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof (PhysicsSS))]
public class BasicMovement : MonoBehaviour {

	// m_physics 
	public bool IsCurrentPlayer = false;
	public float JumpHeight = 4.0f;
	public float TimeToJumpApex = .4f;

	float m_accelerationTimeAirborne = .2f;
	float m_accelerationTimeGrounded = .1f;
	public float MoveSpeed = 8.0f;

	float m_gravity;
	float m_jumpVelocity;
	Vector2 velocity;
	Vector2 m_jumpVector;
	float m_velocityXSmoothing;
    float m_velocityYSmoothing;
	//-------------------
	PhysicsSS m_physics;

	public bool canDoubleJump = false;
    public bool doubleJumpAvailable = true;

	float inputX = 0.0f;
	float inputY = 0.0f;

	public bool autonomy = true;

	bool targetSet = false;
	bool targetObj = false;
	Vector3 targetPoint;
	public float minDistance = 1.0f;
	public float abandonDistance = 10.0f;
	public PhysicsSS followObj;
    private List<AIBase> m_aibase;
    private List<OffensiveTemplate> m_offensiveTemplates;
	internal void Start()  {
		m_physics = GetComponent<PhysicsSS> ();
		m_gravity = -(2 * JumpHeight) / Mathf.Pow (TimeToJumpApex, 2);
		m_physics.setGravityForce(m_gravity * (1.0f/60f));
		m_jumpVelocity = Mathf.Abs(m_gravity) * TimeToJumpApex;
		m_jumpVector = new Vector2 (0f, m_jumpVelocity);

        m_aibase = new List<AIBase>(GetComponents<AIBase>());
        m_offensiveTemplates = new List<OffensiveTemplate>(GetComponents<OffensiveTemplate>());
    }
		
	public void onHitConfirm(GameObject otherObj) {
	}

	void Update() {
        InputPacket ip = new InputPacket();
		if (IsCurrentPlayer) {
            ip = playerMovement();
		} else {
			ip = npcMovement ();
		}
        baseMovement(ip);
        foreach (OffensiveTemplate ot in m_offensiveTemplates)
            ot.HandleInput(ip);

        #region powers
        if (poweredUp)
        {
            powerUpCooldown -= Time.deltaTime;
            if (powerUpCooldown <= 0)
            {
                poweredUp = false;
                doubleJumpAvailable = false;
                canDoubleJump = false;
                //FindObjectOfType<PowerUpUI>().DestroyJump();
            }
        }
        #endregion
    }

    internal InputPacket playerMovement() {
        InputPacket ip = new InputPacket();
        ip.movementInput.x = Input.GetAxis("Horizontal");
        ip.movementInput.y = Input.GetAxis("Vertical");
        ip.jump = Input.GetButtonDown("Jump");
        ip.fire1 = Input.GetButton("Fire1");
        ip.fire1Press = Input.GetButtonDown("Fire1");
        Vector3 mousePos = Input.mousePosition;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        ip.MousePointWorld = mousePos;
        return ip;
	}

    //=== NPC movement ====
    internal InputPacket npcMovement()
    {
        InputPacket ip = new InputPacket();
        foreach (AIBase aib in m_aibase)
            ip.Combine( aib.AITemplate());
        return ip;
    }

 //   void oldMovement () {
	//	if (targetSet) {
	//		if (targetObj) {
	//			if (followObj == null) {
	//				endTarget ();
	//				return;
	//			}
	//			targetPoint = followObj.transform.position;
	//		}
	//		moveToPoint (targetPoint);
	//	}
	//}


    internal void baseMovement(InputPacket ip)
    {
        if (m_physics.onGround && doubleJumpAvailable) { canDoubleJump = true; }
        inputX = 0.0f;
        inputY = 0.0f;
        if (!autonomy && m_physics.canMove && targetSet)
        {
            if (targetObj)
            {
                if (followObj == null)
                {
                    endTarget();
                    return;
                }
                targetPoint = followObj.transform.position;
            }
            //moveToPoint(targetPoint);
        }
        else if (m_physics.canMove && autonomy)
        {
            inputY = ip.movementInput.y;
            inputX = ip.movementInput.x ;
            if (ip.jump)
            {
                if (inputY < -0.9f)
                {
                    GetComponent<PhysicsSS>().setDropTime(1.0f);
                }
                else if (m_physics.onGround)
                {
                    m_physics.addSelfForce(m_jumpVector, 0f);
                }
                else if (canDoubleJump)
                {
                    velocity.y = m_jumpVelocity;
                    m_physics.addSelfForce(m_jumpVector, 0f);
                    canDoubleJump = false;
                }
            }
        }
        //m_physics logic
        float targetVelocityX = inputX * MoveSpeed;
        float targetVelocityY = inputY * MoveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref m_velocityXSmoothing, (m_physics.m_collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);
        if (m_physics.Floating)
            velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref m_velocityYSmoothing, (m_physics.m_collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);
        Vector2 input = new Vector2(inputX, inputY);
        m_physics.Move(velocity, input);
        m_physics.AttemptingMovement = (inputX != 0.0f || inputY != 0.0f);
        if (inputX < 0)
            m_physics.setFacingLeft(true);
        else if (inputX > 0)
            m_physics.setFacingLeft(false);
    }

 //   public void moveToPoint(Vector3 point) {
	//	inputX = 0.0f;
	//	inputY = 0.0f;

	//	float dist = Vector3.Distance (transform.position, point);
	//	if (dist > abandonDistance || dist < minDistance) {
	//		endTarget ();
	//	} else {
	//		if (m_physics.canMove) {
	//			if (point.x > transform.position.x) {
	//				if (dist > minDistance)
	//					inputX = 1.0f;
	//				m_physics.setFacingLeft (false);

	//			} else {
	//				if (dist > minDistance)
	//					inputX = -1.0f;
	//				m_physics.setFacingLeft (true);
	//			}
	//		}
	//	}
	//	float targetVelocityX = inputX * MoveSpeed;
	//	velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref m_velocityXSmoothing, (m_physics.collisions.below)?m_accelerationTimeGrounded:m_accelerationTimeAirborne);
	//	Vector2 input = new Vector2 (inputX, inputY);

	//	if (m_physics.canMove && (m_physics.falling == "left" || m_physics.falling == "right") && m_physics.collisions.below) {
	//		m_physics.addSelfForce (new Vector2 (0f, m_jumpVelocity), 0f);
	//	}
	//	m_physics.Move (velocity, input);
	//	m_physics.AttemptingMovement = (inputX != 0.0f);
	//}
	public void setTargetPoint(Vector3 point, float proximity) {
		setTargetPoint (point, proximity, float.MaxValue);
	}
	public void setTargetPoint(Vector3 point, float proximity,float max) {
		targetPoint = point;
		minDistance = proximity;
		abandonDistance = max;
		targetSet = true;
	}

	void setTarget(PhysicsSS target) {
		targetObj = true;
		targetSet = true;
		followObj = target;
	}
	public void endTarget() {
		targetSet = false;
		targetObj = false;
		followObj = null;
		minDistance = 0.2f;
	}

    #region Jump Power Scripts
    private bool poweredUp;
    private float powerUpCooldown;

    public void EnableDoubleJumps(float seconds)
    {
        poweredUp = true;
        powerUpCooldown = seconds;
        doubleJumpAvailable = true;
        //if (FindObjectOfType<PowerUpUI>() != null)
        //    FindObjectOfType<PowerUpUI>().CreateJump();
    }

    public float returnPowerUpCooling()
    {
        return powerUpCooldown;
    }
    //IEnumerator TemporaryDoubleJumps(float seconds)
    //{
    //    doubleJumpAvailable = true;
    //    yield return new WaitForSeconds(seconds);
    //    doubleJumpAvailable = false;
    //    canDoubleJump = false;
    //}
    #endregion
}