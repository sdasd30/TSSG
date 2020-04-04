using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentPlayerSettings : MonoBehaviour
{
    private static CurrentPlayerSettings m_instance;

    private bool AutoFindPlayer;
    private GameObject currentPlayer;

    public static CurrentPlayerSettings Instance
    {
        get { return m_instance; }
        set { m_instance = value; }
    }
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (AutoFindPlayer && currentPlayer != null)
        {
            SetCurrentPlayer();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayer != null)
        {

        } else if (AutoFindPlayer)
        {
            SetCurrentPlayer();
        }
    }

    public static void SetCurrentPlayer()
    {
        m_instance.currentPlayer = FindCurrentPlayer();
    }
    public static void SetCurrentPlayer(GameObject g)
    {
        m_instance.currentPlayer = g;
    }
    public static GameObject GetCurrentPlayer()
    {
        if (m_instance.currentPlayer == null)
            SetCurrentPlayer();
        return m_instance.currentPlayer;
    }
    public static GameObject FindCurrentPlayer()
    {
        MovementBase [] mbList = FindObjectsOfType<MovementBase>();
        foreach (MovementBase m in mbList)
        {
            if (m.IsPlayerControl)
                return m.gameObject;
        }
        return null;
    }
}
