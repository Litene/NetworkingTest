using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent {
    MonoBehaviour Source { get; set; }
    bool IsServerOwner { get; set; }
    public void Initialize<T>(T source, bool isOwner) where T : MonoBehaviour { } // do we want owner here? is it subject to change?

    public void Tick(float deltaTime) { }

    public void Execute() { }
}