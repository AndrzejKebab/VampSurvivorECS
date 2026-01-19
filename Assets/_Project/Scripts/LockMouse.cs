using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AndrzejKebab
{
    public class LockMouse : MonoBehaviour
    {
        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            }
        }
    }
}
