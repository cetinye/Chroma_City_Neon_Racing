using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RightPressed()
    {
        Debug.LogWarning("Right");
        player.SwitchLane(true);
    }

    public void LeftPressed()
    {
        Debug.LogWarning("Left");
        player.SwitchLane(false);
    }
}
