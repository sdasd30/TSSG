using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class TKFollowPath : Task
{
    public PathCreator Path;

    private bool FoundPath = false;
    private Vector3 nextSpot;
    private string state = "getting_to_path";
    private string pathName;
    public float moveProportionSpeed = 1.0f;

    private const float Path_TOLERANCE = 0.5f;
    private float distance_on_path;
    public override void  OnTransition()
    {
        if (pathName != "" && Path  == null)
        {
            GameObject g = GameObject.Find(pathName);
            if (g != null)
                Path = g.GetComponent<PathCreator>();
        } else if (Path != null)
        {
            pathName = Path.gameObject.name;
        }
        state = "getting_to_path";
    }
    // Update is called once per frame
    public override void OnActiveUpdate()
    {
        if (Path == null)
            return;
        nextSpot = Path.path.GetClosestPointOnPath(MasterAI.transform.position);
        if (MasterAI.get2DDistanceToPoint(nextSpot) > Path_TOLERANCE)
        {
            MasterAI.MoveToPoint(nextSpot, Path_TOLERANCE * 0.5f);
            state = "getting_to_path";
        } else
        {
            if (state.Equals("getting_to_path"))
            {
                distance_on_path = Path.path.GetClosestDistanceAlongPath(MasterAI.transform.position);
                state = "following_path";
            } else
            {
                distance_on_path += (moveProportionSpeed * 0.1f * MasterAI.GetComponent<MovementBase>().MaxMoveSpeed * Time.fixedDeltaTime);
                nextSpot = Path.path.GetPointAtDistance(distance_on_path);
                MasterAI.FacePoint(nextSpot);
                MasterAI.MoveToPoint(nextSpot, Path_TOLERANCE * 0.75f, moveProportionSpeed);
            }
        }

    }

    public override void OnLoad(Goal g)
    {
        if (g.ContainsKey("pathName", this))
            pathName = g.GetVariable("pathName", this);
        if (g.ContainsKey("moveProportionSpeed", this))
            moveProportionSpeed = float.Parse(g.GetVariable("moveProportionSpeed", this));
    }

    public override void OnSave(Goal g)
    {
        g.SetVariable("pathName", pathName, this);
        g.SetVariable("moveProportionSpeed", moveProportionSpeed.ToString(), this);
    }

    public override string debugExtraInfo()
    {
        string F = (FoundPath) ? "T" : "F";
        string s = "Found: " + F + " Tgt: " + nextSpot + " d: " + MasterAI.get2DDistanceToPoint(nextSpot) + "\n";
        s += "state: " + state + "\n";
        return s;
    }
}
