using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float verticalOffset;
	public float lookAheadDstX;
	public float lookSmoothTimeX;
	public float verticalSmoothTime;
	public Vector3 focusAreaSize;
	public bool UseCameraLimits;
	public Vector3 minVertex;
	public Vector3 maxVertex;
	Vector2 viewSize;

	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;
	const float DISTANCE_SNAP = 999f;

	bool lookAheadStopped;

	void Start() {
		initFunct ();
	}
    public void initFunct() {
        SetPlayerObj(target);
    }

    public void SetPlayerObj(Transform t)
    {
        if (t != null)
        {
            target = t;
            viewSize.y = GetComponent<Camera>().orthographicSize * 2f;
            viewSize.x = viewSize.y * GetComponent<Camera>().aspect;
            focusArea = new FocusArea(target.GetComponent<Collider>().bounds, focusAreaSize, viewSize, minVertex, maxVertex);
        }
    }
	void Update() {
		if (target != null) {
			focusArea.Update (target.GetComponent<Collider> ().bounds, minVertex, maxVertex, UseCameraLimits);
			if (Vector3.Distance (target.transform.position, transform.position) > DISTANCE_SNAP) {
				transform.position = target.transform.position;
			}
		} else {
			//GameObject pl = GameManager.Instance.CurrentPlayer;
			//if (pl == null)
			//	return;
			//target = pl.GetComponent<PhysicsSS> ();
		}
		Vector3 focusPosition = focusArea.centre + Vector3.up * verticalOffset;


		if (focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign (focusArea.velocity.x);
			//if (Mathf.Sign(target.InputedMove.x) == Mathf.Sign(focusArea.velocity.x) && target.InputedMove.x != 0) {
			//	lookAheadStopped = false;
			//	targetLookAheadX = lookAheadDirX * lookAheadDstX;
			//}
			//else {
			//	if (!lookAheadStopped) {
			//		lookAheadStopped = true;
			//		targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)/4f;
			//	}
			//}
		}

		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.z = Mathf.SmoothDamp (transform.position.z, focusPosition.z, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector3.right * currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.up * 10;
		//transform.position = new Vector3 ((int)transform.position.x, (int)transform.position.z, (int)transform.position.z);
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (0, 0, 1, .1f);
		Gizmos.DrawCube (focusArea.centre, focusAreaSize);
		Gizmos.color = new Color (0, 1, 0, .4f);
		Gizmos.DrawCube (focusArea.centre, viewSize);
	}

	struct FocusArea {
		public Vector3 centre;
		public Vector3 velocity;
		float left,right;
		float top,bottom;

		Vector3 camSize;


		public FocusArea(Bounds targetBounds, Vector3 size,Vector3 largeCam,Vector3 minVertex, Vector3 maxVertex) {
			Vector3 realCenter = new Vector3(targetBounds.center.x,0f,targetBounds.center.y);
			//Debug.Log("Min: " + (realCenter.y - largeCam.y/2f) + " Max vertex: " + minVertex.y);
			if (realCenter.x - largeCam.x/2 < minVertex.x + size.x/2f) {
				realCenter.x = (minVertex.x * 2f + largeCam.x)/2f;
			} else if (realCenter.x + largeCam.x/2 > maxVertex.x - size.x/2f) {
				realCenter.x = (maxVertex.x * 2f - largeCam.x)/2f;
			}

			if (realCenter.z - largeCam.z/2f < minVertex.z + size.z/2f) {
				realCenter.z = (minVertex.z * 2f + largeCam.z)/2f;
			} else if (realCenter.z + largeCam.z/2f > maxVertex.z - size.z/2f) {
				realCenter.z = (maxVertex.z * 2f - largeCam.z)/2f;
			}
			left = realCenter.x - size.x/2f;
			right = realCenter.x + size.x/2f;
			bottom = realCenter.z - size.z/2f;
			top = realCenter.z + size.z/2f;

			velocity = Vector3.zero;
			centre = new Vector3((left+right)/2,(top + bottom)/2);
			camSize = largeCam;
		}

		public void Update(Bounds targetBounds,Vector3 minVertex, Vector3 maxVertex,bool UseCameraLimits) {
			float shiftX = 0;

			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			bool shift = true;
			if (UseCameraLimits) {
				float extra = camSize.x;
				if (left - (extra / 2f) < minVertex.x && shiftX < 0f) {
					shift = false;
				}
				if (right + (extra/2f) > maxVertex.x && shiftX > 0f) {
					shift = false;
				}
			}
			if (shift) {
				left += shiftX;
				right += shiftX;
			}

			float shiftZ = 0;
			if (targetBounds.min.z < bottom) {
                shiftZ = targetBounds.min.z - bottom;
			} else if (targetBounds.max.z > top) {
                shiftZ = targetBounds.max.z - top;
			}
			shift = true;
			if (UseCameraLimits) {
				float extra = camSize.z;
				if (bottom - (extra/2f) < minVertex.z && shiftZ < 0f) {
					shift = false;
				}
				if (top + (extra/2f) > maxVertex.z && shiftZ > 0f) {
					shift = false;
				}
			}
			if (shift) {
				bottom += shiftZ;
				top += shiftZ;
			}
			centre = new Vector3((left+right)/2,0,(top +bottom)/2);
			velocity = new Vector3 (shiftX,0, shiftZ);
		}
	}

}
