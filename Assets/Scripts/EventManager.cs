using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public event EventHandler<OnSpacePressedEventsArgs> OnspacePressed;
    public class OnSpacePressedEventsArgs : EventArgs {
        public string text;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            OnspacePressed?.Invoke(this, new OnSpacePressedEventsArgs {text = "bite"});
        }
    }
}
