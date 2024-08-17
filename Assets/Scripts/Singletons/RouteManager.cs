using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteManager : Singleton<RouteManager>
{
    [SerializeField]
    public List<Route> Routes = new List<Route>();

    void Start()
    {
    }

    void Update()
    {

    }
}
