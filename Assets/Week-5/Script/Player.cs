using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Week6
{
    public class Player : MonoBehaviour
    {
        private CharacterController controller;

        [SerializeField]
        float speed = 5.0f;
        [SerializeField]
        float rotation = 5.0f;
        [SerializeField]
        int maxHealth = 100;

        int currentHealth;

        private float lastMouseX = 0f;
        private float mouseDeltaX = 0f;
        private int rotDir = 0;

        [SerializeField] float trapDetectionDistance = 2f;
        [SerializeField] int trapDamage = 10;
        [SerializeField] float doorDetectionDistance = 2f;

        public Text healthText;
        public int keyCount = 0;
        public Text keyCountText;
        public Text coinCountText;
        public int coinCount = 0;

        public GameObject doorObject;
        private bool doorOpened = false;


        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
            lastMouseX = Input.mousePosition.x;
            UpdateUI();
        }

        private void Update()
        {
            HandleMovement();
            HandleRotation();
            CheckForTraps();
            CheckForDoor();
        }

        void HandleMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 input = (horizontalInput * transform.right) + (transform.forward * verticalInput);
            controller.Move(input * speed * Time.deltaTime);
        }

        void HandleRotation()
        {
            mouseDeltaX = Input.mousePosition.x - lastMouseX;

            if (mouseDeltaX != 0)
            {
                rotDir = mouseDeltaX > 0 ? 1 : -1;
                lastMouseX = Input.mousePosition.x;

                transform.eulerAngles += new Vector3(0, rotation * Time.deltaTime * rotDir, 0);
            }
        }

        void CheckForTraps()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, trapDetectionDistance))
            {
                if (hit.collider.CompareTag("Trap"))
                {
                    TakeDamage(trapDamage);
                }
            }
        }
        void CheckForDoor()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, doorDetectionDistance))
            {
                if (hit.collider.CompareTag("Door") && HasKey())
                {
                    RotateDoor();
                    doorOpened = true;
                }
                
                else if (hit.collider.CompareTag("Door"))
                {
                    Debug.Log("You need a key.");
                }
            }
        }
        void RotateDoor()
        {
            doorObject.transform.Rotate(0, 90, 0);
        }
        bool HasKey()
        {
            return keyCount > 0;
        }

        void TakeDamage(int damage)
        {
            healthText.text = "Health:" + (maxHealth -= damage);
        }

        void AddKey()
        {
            keyCount++;
            UpdateUI();
        }

        void RemoveKey()
        {
            keyCount--;
            UpdateUI();
        }

        private void UpdateUI()
        {
            healthText.text = "Health:" + currentHealth;
            keyCountText.text = "Key:" + keyCount;
            coinCountText.text = "Coin:" + coinCount;
        }

    }
}
