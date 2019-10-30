using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simplerotate : MonoBehaviour {

    private RectTransform rectComponent;
    private Image imageComp;
    public float rotateSpeed = 200f;
    private float currentvalue;
    public Vector3 RotateAxis;

    void Start()
    {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
    }

    void Update()
    {
        rectComponent.transform.Rotate(RotateAxis * rotateSpeed * Time.deltaTime);
    }
}