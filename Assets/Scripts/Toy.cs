using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToyType {
    Ball,
    Plane,
}

public class Toy : MonoBehaviour {
    [SerializeField]
    public ToyType Type;
}
