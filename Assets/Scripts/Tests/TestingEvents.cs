using System;
using UnityEngine;

public class TestingEvents : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventManager eventManager = GetComponent<EventManager>();
        eventManager.OnspacePressed += EventManage_OnSpacepressed;
        
    }

    private void EventManage_OnSpacepressed(object sender, EventManager.OnSpacePressedEventsArgs e) {
        Debug.Log(e.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
