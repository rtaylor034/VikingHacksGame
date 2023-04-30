using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Translate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * _speed * Time.deltaTime);

        transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
    }
}
