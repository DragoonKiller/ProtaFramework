using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using UnityEngine;

public class ECCamera : EComponent
{
    Camera _camera;
    public new Camera camera => _camera ? _camera : _camera = GetComponent<Camera>();
}
