using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;


public class CameraController : MonoBehaviour
{

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.visible = true;
        }
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            transform.Rotate(new Vector3(-y, x, 0));
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.DOBlendableMoveBy(transform.forward, .1f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.DOBlendableMoveBy(-transform.forward, .1f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.DOBlendableMoveBy(-transform.right, .1f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.DOBlendableMoveBy(transform.right, .1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.DOBlendableMoveBy(transform.up, .1f);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.DOBlendableMoveBy(-transform.up, .1f);
        }
    }

}

