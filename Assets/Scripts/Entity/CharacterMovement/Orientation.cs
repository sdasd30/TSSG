using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { UP, RIGHT, DOWN, LEFT , NEUTRAL}

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class Orientation : MonoBehaviour
{

    // Tracking m_sprite orientation (flipping if left)...
    //private SpriteRenderer m_sprite;
    [HideInInspector]
    public bool FacingLeft = false;
    [HideInInspector]
    public Direction CurrentDirection = Direction.DOWN;

    public float LastZ = 0f;
    // Use this for initialization
    internal void Awake()
    {
    }

    public void SetDirection(Direction d)
    {
        CurrentDirection = d;
        
        if (d == Direction.RIGHT)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90f, 0f, 0f));
        }
        if (d == Direction.UP)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90f, -90f, 0f));
        }
        if (d == Direction.LEFT)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90f, 180f, 0f));
        }
        if (d == Direction.DOWN)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90f, 90f, 0f));
        }
        LastZ = transform.rotation.y;
    }
    public void SetDirection(Vector3 directionVector)
    {
        float angle = Mathf.Atan2(directionVector.z, directionVector.x);
        transform.rotation = Quaternion.Euler(new Vector3(90f, angle, 0f));
        LastZ = -angle;
    }
    public void FacePoint(Vector3 facePoint)
    {
        float angle = Mathf.Atan2(transform.position.z - facePoint.z, facePoint.x - transform.position.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(90f, angle, 0f));
        LastZ = -angle;
    }
    public void FaceVector(Vector3 targetVector)
    {
        FaceVector(new Vector2(targetVector.x, targetVector.z));
    }
    public void FaceVector(Vector2 targetVector)
    {
        float angle = Mathf.Atan2(-targetVector.y, targetVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(90f, angle, 0f));
        LastZ = -angle;
    }
    public void SetDirection (bool facingLeft)
    {
        if (facingLeft)
            SetDirection(Direction.LEFT);
        else
            SetDirection(Direction.RIGHT);
    }
    public Direction VectorToDirection(Vector3 v)
    {
        if (Mathf.Abs(v.z) > Mathf.Abs(v.x))
        {
            if (v.z > 0)
            {
                return Direction.UP;
            }
            else
            {
                return Direction.DOWN;
            }

            
        } else
        {
            if (v.x > 0)
            {
                return Direction.RIGHT;
            }
            else
            {
                return Direction.LEFT;
            }
        }
        
    }
    public Vector3 OrientVectorToDirection2D(Vector3 v, bool negativesAllowed = true)
    {
        Vector3 newV = new Vector3(v.x, v.y, v.z);
        if (FacingLeft)
        {
            newV.x = -v.x;
        }
        return newV;
    }
    public Vector2 OrientVectorToDirection2D(Vector2 v, bool negativesAllowed = true)
    {
        Vector2 newV = new Vector2(v.x, v.y);
        if (FacingLeft)
        {
            newV.x = -v.x;
        }
        return newV;
    }
    public Vector3 OrientVectorToDirection(Vector3 v,bool negativesAllowed = true)
    {
        return OrientVectorToDirection(CurrentDirection, v, negativesAllowed);
    }

    public Vector3 OrientVectorToDirection(Direction d, Vector3 v, bool negativesAllowed = true)
    {
        Vector3 newV = new Vector3(v.x, v.y, v.z);
        if (d == Direction.UP)
        {
            newV.x = -v.z;
            newV.z = v.x;
        }
        else if (d == Direction.LEFT)
        {
            newV.x = -v.x;
            newV.z = -v.z;
        }
        else if (d == Direction.DOWN)
        {
            newV.x = v.z;
            newV.z = -v.x;
        }
        if (!negativesAllowed)
        {
            newV.x = Mathf.Abs(newV.x);
            newV.z = Mathf.Abs(newV.z);
        }
        return newV;
    }
    public Direction DirectionToPoint(Vector3 point)
    {
        Vector3 me = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 you = new Vector3(point.x, 0f, point.z);
        float angleToPoint = Vector3.SignedAngle(Vector3.right, you - me, Vector3.up);
        Direction testDirection = Direction.UP;
        if (angleToPoint <= -45f && angleToPoint >= -135f)
            testDirection = Direction.UP;
        if (angleToPoint < -135f || angleToPoint > 135f)
            testDirection = Direction.LEFT;
        if (angleToPoint < 135f && angleToPoint > 45f)
            testDirection = Direction.DOWN;
        if (angleToPoint < 45f && angleToPoint > -45f)
            testDirection = Direction.RIGHT;
        return testDirection;
    }

    public float AngleToPoint(Vector3 targetPoint)
    {
        Vector3 me = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 you = new Vector3(targetPoint.x, 0f, targetPoint.z);
        float ang = Vector3.Angle(me, you);
        float sang = -Vector3.SignedAngle(Vector3.right, you - me, Vector3.up);
        return -Vector3.SignedAngle(Vector3.right, you - me, Vector3.up);
    }
    public bool DirectionToPoint2D(Vector3 point)
    {
        return (point.x < transform.position.x);
    }
    public void OrientToPoint2D(Vector3 point)
    {
        SetDirection(point.x < transform.position.x);
    }
    public bool FacingPoint(Vector3 point)
    {
        return FacingPoint(point,CurrentDirection);
    }
    public bool FacingPoint(Vector3 point, Direction d)
    {
        return DirectionToPoint(point) == CurrentDirection;
    }
    public bool FacingPoint2D(Vector3 point)
    {
        return FacingPoint2D(point, FacingLeft);
    }
    public bool FacingPoint2D(Vector3 point, bool left)
    {
        return (point.x < transform.position.x == left);
    }

    public bool FacingPoint(Vector3 point, float tolerance)
    {
        float angle = AngleToPoint(point);
        float myCurrentAngle = LastZ;
        if (myCurrentAngle > 180)
            myCurrentAngle = -(360 - myCurrentAngle);
        return (Mathf.Abs(angle - myCurrentAngle) < tolerance ||
            Mathf.Abs(angle = (myCurrentAngle + 360f)) < tolerance);
    }
    public static Vector3 OrientToVectorZ(Vector3 baseVector, float ZRotation)
    {
        float x = baseVector.x * Mathf.Cos(ZRotation) + baseVector.z * Mathf.Sin(ZRotation);
        float y = baseVector.y;
        float z = baseVector.x * -Mathf.Sin(ZRotation) + baseVector.z * Mathf.Cos(ZRotation);
        return new Vector3(x,y,z);
    }
}

