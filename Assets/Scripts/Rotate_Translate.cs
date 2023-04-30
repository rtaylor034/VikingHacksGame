using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Translate : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);

        transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
    }
}
