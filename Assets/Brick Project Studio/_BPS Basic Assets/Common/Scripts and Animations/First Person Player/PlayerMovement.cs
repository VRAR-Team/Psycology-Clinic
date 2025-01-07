using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SojaExiles
{
    public class PlayerMovement : MonoBehaviour
    {
        public CharacterController controller;

        public float speed = 5f;
        public float gravity = -15f;

        Vector3 velocity;

        bool isGrounded;
        bool isMoving = true; // 控制是否允许移动

        // Update is called once per frame
        void Update()
        {
            // 检查是否按下鼠标右键
            if (Input.GetMouseButtonDown(1))
            {
                isMoving = false;
                Cursor.lockState = CursorLockMode.None; // 解锁鼠标指针
                Cursor.visible = true; // 显示鼠标指针
            }

            // 检查是否按下 C 键
            if (Input.GetKeyDown(KeyCode.C))
            {
                isMoving = true;
                Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标指针
                Cursor.visible = false; // 隐藏鼠标指针
            }

            if (isMoving)
            {
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");

                Vector3 move = transform.right * x + transform.forward * z;

                controller.Move(move * speed * Time.deltaTime);

                velocity.y += gravity * Time.deltaTime;

                controller.Move(velocity * Time.deltaTime);
            }
        }
    }
}