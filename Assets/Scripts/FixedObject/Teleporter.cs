using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{

    public Vector3 TargetPos;
    public List<GameObject> TargetObject;
    public List<GameObject> m_collidedObjs = new List<GameObject>();
    public List<GameObject> m_overlappingControl = new List<GameObject>();

    private Dictionary<GameObject, int> m_timeRemaining = new Dictionary<GameObject, int>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //foreach(GameObject go in m_timeRemaining)
        //{

        //}
    }


    internal void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_collidedObjs.Contains(other.gameObject) && (other.GetComponent<Attackable>()!= null || other.GetComponent<Projectile>() != null))
        {
            m_collidedObjs.Add(other.gameObject);
            TeleportObject(other.gameObject);
        }
    }

    internal void OnTriggerExit2D(Collider2D other)
    {
        /*
		 * TODO: Delay removal of collided object to avoid stuttered collisions 
		 */
        /*
		if (other.gameObject.GetComponent<Attackable> () && collidedObjs.Contains(other.gameObject.GetComponent<Attackable>())) {
			collidedObjs.Remove (other.gameObject.GetComponent<Attackable> ());
		}
		*/
        if (m_collidedObjs.Contains(other.gameObject))
        {
            m_collidedObjs.Remove(other.gameObject);
        }
    }

    private void TeleportObject(GameObject go)
    {
        if (TargetObject.Count > 0)
        {
            GameObject target = TargetObject[Random.Range((int)0, TargetObject.Count)];
            if (target.GetComponent<Teleporter>())
            {
                target.GetComponent<Teleporter>().m_collidedObjs.Add(go);
            }
            go.transform.position = target.transform.position;
        } else
        {
            go.transform.position = TargetPos;

        }
        //if (m_overlappingControl.Contains(go.gameObject))
        //{
        //    m_overlappingControl.Remove(go.gameObject);
        //}
    }
}
