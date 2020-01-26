using UnityEngine;
using System.Collections.Generic;

//Class allowing basic self-propelled movement for objects in 2D plane.
[RequireComponent (typeof (BoxCollider2D))]
public class PhysicsSS : MonoBehaviour {

    //Collision Detection
	//public LayerMask collisionMask;
    public List<LayerMask> collisionMaskList;
    public Vector2 m_accumulatedVelocity = Vector2.zero;

    public CollisionInfo m_collisions;

    private const float SKIN_WIDTH = .015f;
	private int horizontalRayCount = 4;
	private int verticalRayCount = 4;
    private float horizontalRaySpacing;
    private float verticalRaySpacing;
    private BoxCollider2D bCollider;
    private RaycastOrigins raycastOrigins;

	

    private const float VELOCITY_MINIMUM_THRESHOLD = 0.3f;

    //Persistent Stats
    public bool Floating = true;
    public float GravityForce = -1.0f;
    public bool facingLeft = false;
    public bool canMove = true;
    public bool AttemptingMovement = false;
    public float DecelerationRatio = 1.0f;
    public Vector3 TrueVelocity;
    private float TerminalVelocity = -0.5f;
    private float maxClimbAngle = 80;

    // Tracking movement
    public bool onGround = true;
    public float dropThruTime = 0.0f;

    private Vector2 m_velocity;
    private List<Vector2> m_forces = new List<Vector2>();
    private List<float> timeForces = new List<float>();
    private float m_initialOffsetX;
    private Vector3 m_lastPosition;
    private float m_gravityCancelTime = 0f;
   

    Vector2 m_inputedForce = new Vector2();
    // Tracking inputed movement
    public Vector2 SelfInput = Vector2.zero;
    

    //floating
    private float m_oldFloatingTime = 0f;
    private bool m_oldFloating = false;

    void Start() {
		bCollider = GetComponent<BoxCollider2D> ();
		float newBOffY = bCollider.offset.y + SKIN_WIDTH;
		m_initialOffsetX = bCollider.offset.x;
		bCollider.offset = new Vector2(m_initialOffsetX,newBOffY);
		CalculateRaySpacing ();
		canMove = true;
		setFacingLeft (facingLeft);
	}

	public void setGravityForce(float gravScale) {
		GravityForce = gravScale;
	}

	void FixedUpdate() {
		dropThruTime = Mathf.Max(0f,dropThruTime - Time.fixedDeltaTime);
        DecelerateAutomatically(VELOCITY_MINIMUM_THRESHOLD);
        //      if (Mathf.Abs(m_accumulatedVelocity.x) > 0.3f) {
        //	if (m_collisions.below) {
        //		m_accumulatedVelocity.x *= (1.0f - Time.fixedDeltaTime * DecelerationRatio * 2.0f);
        //	} else {
        //		m_accumulatedVelocity.x *= (1.0f - Time.fixedDeltaTime * DecelerationRatio * 3.0f);
        //	}
        //} else {
        //	m_accumulatedVelocity.x = 0f;
        //}
        m_updateTrueVelocity();
        m_processMovement();
        m_updateFloating();
    }
    private void DecelerateAutomatically(float threshold)
    {
        if (m_accumulatedVelocity.sqrMagnitude > threshold)
            m_accumulatedVelocity *= (1.0f - Time.fixedDeltaTime * DecelerationRatio * 3.0f);
        else
            m_accumulatedVelocity = Vector2.zero;
    }
    private void m_updateTrueVelocity()
    {
        TrueVelocity = transform.position - m_lastPosition;
        m_lastPosition = transform.position;
    }
    private void m_updateFloating()
    {
        if (m_oldFloatingTime > 0f)
        {
            m_oldFloatingTime -= Time.fixedDeltaTime;
            if (m_oldFloatingTime <= 0f)
                m_continueFromFreeze();
        }
    }
    public void m_continueFromFreeze()
    {
        Floating = m_oldFloating;
    }
    public void m_freezeInAir(float time)
    {
        if (time > 0f)
        {
            m_oldFloating = Floating;
            m_oldFloatingTime = time;
            Floating = true;
            //CancelVerticalMomentum();
            m_accumulatedVelocity = Vector2.zero;
            //m_artificialVelocity = Vector2.zero;
        }
        else
        {
            m_continueFromFreeze();
        }
    }
    private void m_processMovement()
    {
        m_checkCanMove();
        ApplyForcesToVelocity();
        //processArtificialVelocity();
        UpdateRaycastOrigins();
        m_movePlayer();
        m_collisions.Reset();
    }
    private void m_checkCanMove()
    {
        if (canMove)
            return;
        m_inputedForce = new Vector2(0, 0);
    }

    private void ApplyForcesToVelocity()
	{
        m_inputedForce = m_inputedForce * Time.fixedDeltaTime;
        m_velocity.x = m_inputedForce.x;
        if (Floating)
            m_velocity.y = m_inputedForce.y;
        bool Yf = false;

        for (int i = m_forces.Count - 1; i >= 0; i--)
        {
            Vector2 selfVec = m_forces[i];
            if (selfVec.y != 0f && !Floating)
            {
                m_velocity.y = 0f;
                Yf = true;
            }
            if (timeForces[i] < Time.fixedDeltaTime)
            {
                m_velocity += (selfVec * Time.fixedDeltaTime);
            }
            else
            {
                m_velocity += (selfVec * Time.fixedDeltaTime);
            }
            timeForces[i] = timeForces[i] - Time.fixedDeltaTime;
            if (timeForces[i] < 0f)
            {
                m_forces.RemoveAt(i);
                timeForces.RemoveAt(i);
            }
            if (!Floating && m_velocity.y > TerminalVelocity)
            {
                if (Yf || !m_collisions.below)
                {
                    m_velocity.y += GravityForce * Time.fixedDeltaTime;
                }
                else if (m_collisions.below)
                { //force player to stick to slopes
                    m_velocity.y += GravityForce * Time.fixedDeltaTime * 6f;
                }
            }

        }
        m_velocity.x += (m_accumulatedVelocity.x * Time.fixedDeltaTime);
        if (Floating)
            m_velocity.y += (m_accumulatedVelocity.y * Time.fixedDeltaTime);
        if (m_velocity.y > TerminalVelocity && !Floating)
        {
            if (!m_collisions.below && m_gravityCancelTime <= 0f)
            {
                m_velocity.y += GravityForce * Time.fixedDeltaTime;
            }
            else if (m_collisions.below)
            {
                m_velocity.y += GravityForce * Time.fixedDeltaTime * 6f;
            }
        }
    }
    private void m_movePlayer() {
		if (!canMove) {
			SelfInput = Vector2.zero;
		}
		if (m_collisions.above || m_collisions.below){
			m_velocity.y = 0.0f;
		}  

		m_collisions.Reset ();
        Vector2 preCollideV = new Vector2(m_velocity.x, m_velocity.y);
        if (m_velocity.x != 0 || m_inputedForce.x != 0)
            Horizontalcollisions(ref m_velocity);
        if (m_velocity.y != 0 || m_inputedForce.y != 0)
            Verticalcollisions(ref m_velocity);
        transform.Translate(m_velocity, Space.World);
	}

	public void addToVelocity(Vector2 veloc )
	{
		m_accumulatedVelocity.x += veloc.x;
        if (Floating)
            m_accumulatedVelocity.y += veloc.y;
        else
		    addSelfForce (new Vector2 (0f, veloc.y), 0f);
	}
	public void addSelfForce(Vector2 force, float duration) {
		m_forces.Add (force);
		timeForces.Add (duration);
	}

	public void Move(Vector2 veloc, Vector2 input) {
		//NumKnivesLog ("Move Called with input: " + input);
		SelfInput = input;
		m_inputedForce = veloc;
		if (SelfInput.x != 0.0f)
			setFacingLeft (SelfInput.x < 0.0f);
	}
	bool handleStairs(RaycastHit2D hit,Vector2 vel) {
		if (hit.collider.gameObject.GetComponent<JumpThru> ()) {
			if (dropThruTime > 0f) {
				return true;
			}
			if (hit.collider.gameObject.GetComponent<EdgeCollider2D> ()) {
				EdgeCollider2D ec = hit.collider.gameObject.GetComponent<EdgeCollider2D> ();
				Vector2[] points = ec.points;
				Vector2 leftPoint = Vector2.zero;
				bool foundLeft = false;
				Vector2 rightPoint = Vector2.zero;
				bool foundRight = false;
				foreach (Vector2 p in points) {
					float xDiff = (ec.transform.position.x + p.x) - transform.position.x;
					if (xDiff < -0.01f) {
						if (foundRight) {
							float yDiff = p.y - rightPoint.y;
							if (yDiff > 0.01f) {
								if (vel.x > 0f) {
									return true;
								} else {
									return false;
								}
							} else if (yDiff < -0.01f) {
								if (vel.x > 0f) {
									return false;
								} else {
									return true;
								}
							}
						} else {
							if (Vector2.Equals(Vector2.zero,leftPoint)) {
								leftPoint = p;
								foundLeft = true;
							}
						}
					} else if (xDiff > 0.01f) {
						if (foundLeft) {
							float yDiff = p.y - leftPoint.y;
							if (yDiff > 0.01f) {
								if (vel.x < 0f) {
									return true;
								} else {
									return false;
								}
							} else if (yDiff < -0.01f) {
								if (vel.x < 0f) {
									return false;
								} else {
									return true;
								}
							}
						} else {
							if (Vector2.Equals(Vector2.zero,rightPoint)) {
								rightPoint = p;
								foundRight = true;
							}
						}
					}
				}
			} else {
				return true;
			}
		}
		return false;
	}
	void Horizontalcollisions(ref Vector2 velocity) {
		float directionX;
		//Debug.Log ("Horizontal m_collisions:" + SelfInput + " : " + velocity);
		if (velocity.x == 0) {
			directionX = Mathf.Sign (SelfInput.x);
		} else {
			directionX = Mathf.Sign (velocity.x);
		}
		float rayLength = Mathf.Max(0.05f,Mathf.Abs (velocity.x) + SKIN_WIDTH);

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			if (i == horizontalRayCount - 1) {
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			} else {
				rayOrigin += Vector2.up * (horizontalRaySpacing/2f * i);
			}
            foreach (LayerMask collisionMask in collisionMaskList)
            {
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                if (hit)
                {
                    //Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);
                }
                else
                {
                    //Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.green);
                }

                if (hit && !hit.collider.isTrigger)
                {

                    if (handleStairs(hit, velocity))
                    {

                    }
                    else
                    {
                        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                        if (i == 0 && slopeAngle <= maxClimbAngle)
                        {
                            float distanceToSlopeStart = 0;
                            if (slopeAngle != m_collisions.slopeAngleOld)
                            {
                                distanceToSlopeStart = hit.distance - SKIN_WIDTH;
                                //velocity.x -= distanceToSlopeStart * directionX;
                                velocity.x = (Mathf.Abs(velocity.x) + distanceToSlopeStart) * directionX;
                            }
                            ClimbSlope(ref velocity, slopeAngle);
                            //velocity.x += distanceToSlopeStart * directionX;
                            velocity.x = (Mathf.Abs(velocity.x) + distanceToSlopeStart) * directionX;
                        }

                        if (!m_collisions.climbingSlope || slopeAngle > maxClimbAngle)
                        {
                            velocity.x = (hit.distance - SKIN_WIDTH) * directionX;
                            rayLength = hit.distance;

                            if (m_collisions.climbingSlope)
                            {
                                velocity.y = Mathf.Tan(m_collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                            }

                            m_collisions.left = directionX == -1;
                            m_collisions.right = directionX == 1;
                            break;
                        }
                    }
                }
            }
		}

	}
	
	void Verticalcollisions(ref Vector2 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + SKIN_WIDTH;

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            foreach (LayerMask collisionMask in collisionMaskList)
            {
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                if (hit && !hit.collider.isTrigger && hit.collider.gameObject != gameObject)
                {
                    if (hit.collider.gameObject.GetComponent<JumpThru>() && (velocity.y > 0 || dropThruTime > 0f))
                    { //|| handleStairs(hit,velocity))){
                    }
                    else
                    {
                        velocity.y = (hit.distance - SKIN_WIDTH) * directionY;
                        rayLength = hit.distance;
                        if (m_collisions.climbingSlope)
                        {
                            velocity.x = velocity.y / Mathf.Tan(m_collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                        }

                        m_collisions.below = directionY == -1;
                        m_collisions.above = directionY == 1;
                        break;
                    }
                }
            }
		}
		//m_falling = "none";
		onGround = false;
		rayLength = rayLength + 0.1f;
		if (true) {
			//bool collide = false;
			//bool started = false;
			rayLength = 0.3f;
			for (int i = 0; i < verticalRayCount; i++) {
				Vector2 rayOrigin = raycastOrigins.bottomLeft; //true ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
                foreach (LayerMask collisionMask in collisionMaskList)
                {
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * -1f, rayLength, collisionMask);
                    if (hit && hit.collider.gameObject.GetComponent<JumpThru>() && (dropThruTime > 0f))
                    {
                    }
                    else
                    {
                        if (hit && !hit.collider.isTrigger && hit.collider.gameObject != gameObject)
                        {
                            ////Debug.Log (hit.collider.gameObject);
                            //Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);
                            onGround = true;
                            //collide = true;
                        }
                        else
                        {
                            //Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.green);
                        }
                    }
                    //started = true;
                }
			}
		}
	}
	public void setDropTime(float tm) {
		dropThruTime = tm;
	}
	void ClimbSlope(ref Vector2 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			m_collisions.below = true;
			m_collisions.climbingSlope = true;
			m_collisions.slopeAngle = slopeAngle;
		}
	}

	void UpdateRaycastOrigins() {
		Bounds bounds = bCollider.bounds;
	
		bounds.Expand (SKIN_WIDTH );

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x , bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = bCollider.bounds;
		bounds.Expand (SKIN_WIDTH );

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public float slopeAngle, slopeAngleOld;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
	public void setFacingLeft(bool left) {
		facingLeft = left;
        if (GetComponent<Orientation>() != null)
            GetComponent<Orientation>().SetDirection(left);
	}
	public void TurnToTransform(Transform t) {
		if (t.position.x > transform.position.x) {
			setFacingLeft (false);
		} else if (t.position.x < transform.position.x) {
			setFacingLeft (true);
		}
	}

}
