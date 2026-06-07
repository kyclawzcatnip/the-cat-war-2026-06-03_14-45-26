using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace CatWar
{
    public class CameraController : MonoBehaviour
    {
        [Header("Panning Limits")]
        [SerializeField] private float minX = -8f;
        [SerializeField] private float maxX = 8f;

        [Header("Keyboard Controls")]
        [SerializeField] private float keyboardSpeed = 12f;

        [Header("Drag Controls")]
        [SerializeField] private float dragSpeed = 0.8f;

        private Vector3 dragOrigin;
        private bool isDragging;

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
                return;

            HandleKeyboardInput();
            HandleMouseDragInput();
        }

        private void HandleKeyboardInput()
        {
            float horizontalInput = 0f;

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                {
                    horizontalInput = -1f;
                }
                else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                {
                    horizontalInput = 1f;
                }
            }
#else
            horizontalInput = Input.GetAxisRaw("Horizontal");
#endif

            if (Mathf.Abs(horizontalInput) > 0.05f)
            {
                Vector3 pos = transform.position;
                pos.x += horizontalInput * keyboardSpeed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                transform.position = pos;
            }
        }

        private void HandleMouseDragInput()
        {
            Vector3 currentMousePos = Vector3.zero;
            bool clickDown = false;
            bool clickHeld = false;

#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                currentMousePos = Mouse.current.position.ReadValue();
                clickDown = Mouse.current.leftButton.wasPressedThisFrame;
                clickHeld = Mouse.current.leftButton.isPressed;
            }
#else
            currentMousePos = Input.mousePosition;
            clickDown = Input.GetMouseButtonDown(0);
            clickHeld = Input.GetMouseButton(0);
#endif

            if (clickDown)
            {
                dragOrigin = currentMousePos;
                isDragging = true;
                return;
            }

            if (clickHeld && isDragging)
            {
                // Calculate move difference
                Vector3 difference = currentMousePos - dragOrigin;
                dragOrigin = currentMousePos; // Reset drag origin

                Vector3 pos = transform.position;
                pos.x -= difference.x * dragSpeed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                transform.position = pos;
            }
            else
            {
                isDragging = false;
            }
        }
    }
}
