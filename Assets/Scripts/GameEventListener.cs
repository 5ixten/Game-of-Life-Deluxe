using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameEventListener : MonoBehaviour {
    public GameEvent gameEvent;
    public UnityEvent onEventTriggered;

    void OnEnable() {
        gameEvent.AddListener(this);
    }

    void OnDisable() {
        gameEvent.RemoveListener(this);
    }

    public void OnEventTriggered() {
        onEventTriggered.Invoke();
    }
}