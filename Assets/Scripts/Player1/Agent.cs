using System.Collections.Generic;
using Blackout.Inventory;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;
using UnityEngine.UIElements;


public class Agent : NetworkBehaviour, IInteractor, IDamageable
{

    private AgentInteractionHandler handler = new AgentInteractionHandler();

    public bool isInLocker;

    
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);
    
    public float spawnTimer = 20f;
    public bool shouldSpawnEntity = false;
    
    public bool canGrapple;
    public int batteryCount = 5;
    
    private SphereCollider myCheckTrigger;
    [SerializeField] private float interactionRange;

    LinkedList<BaseInteractable> interactablesInRange = new LinkedList<BaseInteractable>();
    LinkedListNode<BaseInteractable> currentSelectedInteractable;
    [SerializeField] private float speed;

    [SerializeField] private float jumpForce = 5f;

    [SerializeField] public float currentSpeed = 0;
    [SerializeField] private Transform groundCheck;

    private const float BASE_SPEED = 3f;
    private const float MAX_SPEED = BASE_SPEED * 1.5f;
    private const float ACCELERATION = 1.5f;
    [SerializeField] private bool isAirborne = true;
    [SerializeField] private Transform playerCameraPivotTransform;
    [SerializeField] public Transform playerCamera;

    [SerializeField] private Transform playerBody;
    [SerializeField] private float xMouseSensitivity = 2f;
    [SerializeField] private float yMouseSensitivity = 2f;
    
    [SerializeField] private Animator animator;
    private Rigidbody playerRigidbody;
    private float xInput;
    private float yInput;
    private float xMouseInput;
    private float yMouseInput;

    private bool shiftPressed;
    private bool isMoving;
    private float xRotation = 0f;
    private float yRotation = 0f;
    [SerializeField] private GameObject playerRightHandSlot;    
    public bool hasKey = false;

    [SerializeField] private ItemLibrary ItemLibrary;

    private static Item[] inventory = new Item[6];
    public bool isItemEquipped = false;
    private int activeInventorySlot = 0;
    
    private GameObject currentlyEquippedItem;
    public GameObject currentlyEquippedItempublic;

    public bool freeze = false;
    private bool enableMovementOnNextTouch;

    public bool activeGrapple;

    private CursorLockMode cursorState;
    public GameObject inventoryCanvas;
    private InventoryController invController;


    

    // For Health and energy bars
    public GameObject PlayerHUDPrefab;
    public UIDocument PlayerHUD;
    public VisualElement PlayerHUDui;
    public VisualElement BarsContainer;
    public VisualElement HealthBar;
    public VisualElement EnergyBar;
    public float Health = 100;
    public int Energy = 25;

    public GameObject GameOverScreenPrefab;
    private UIDocument GameOverScreen;
    public bool isGameOverScreenActive = false;

    public NetworkVariable<bool> isGameWon = new NetworkVariable<bool>(false);
    public static int nbOfDocumentCollected = 0;

    public override void OnNetworkSpawn()
    {

        myCheckTrigger = gameObject.AddComponent<SphereCollider>();
        myCheckTrigger.isTrigger = true;
        myCheckTrigger.radius = interactionRange;

        if (!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }

        PlayerHUD = playerCamera.GetComponentInChildren<UIDocument>();
        if (PlayerHUD == null)
        {
            Debug.LogError("PlayerHUD not found in children of playerCamera.");
            return;
        }
        PlayerHUDui = PlayerHUD.rootVisualElement.Q<VisualElement>("Container");
        PlayerHUDui.style.display = DisplayStyle.Flex;
        PlayerHUDui.pickingMode = PickingMode.Ignore;
        BarsContainer = PlayerHUDui.Q<VisualElement>("BarsContainer");
        HealthBar = BarsContainer.Q<VisualElement>("HealthBar").Q<VisualElement>("BarBG").Q<VisualElement>("BarFill");
        EnergyBar = BarsContainer.Q<VisualElement>("EnergyBar").Q<VisualElement>("BarBG").Q<VisualElement>("BarFill");


        playerRigidbody = GetComponent<Rigidbody>();
        cursorState = CursorLockMode.Locked;
        invController = InventoryController.Instance.GetComponent<InventoryController>();


        TutorialManager.Instance.StartTutorial("player1");
    }

    public static void AddItemToAgentInventory(Item item)
    {
        for(int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == item) return;
            if(inventory[i] == null)
            {
                inventory[i] = item;
                break;
            }
        }
    }

// Removed commented-out implementation of EquipItem to improve code readability and maintainability.
   private void EquipItem()
{
    Debug.Log("EquipItem " + InventoryController.activeInventorySlotId);

    // Déséquipe l'objet actuel si besoin
    if (isItemEquipped)
    {
        CallUnequipItemServerRpc();
        isItemEquipped = false;
        return;
    }

    // Récupère l'objet à équiper depuis l'InventoryController
    Item itemToEquip = invController.GetItemInSlot(InventoryController.activeInventorySlotId);

    if (itemToEquip == null)
    {
        Debug.LogWarning("Aucun objet à équiper dans ce slot.");
        return;
    }

    Debug.Log(itemToEquip.Name);
    Debug.Log("slot " + InventoryController.activeInventorySlotId);
    CallEquipItemServerRpc(itemToEquip.Id);
    isItemEquipped = true;
}

    [ServerRpc]
    private void CallEquipItemServerRpc(int prefabId)
    {
        EquipItemLocalClientRpc(prefabId);
    }
    [ClientRpc]
    private void EquipItemLocalClientRpc(int prefabId)
    {
        GameObject prefab = ItemManager.Instance.GetPrefabById(prefabId);
        if (prefab == null)
        {
            Debug.LogError("Item not found, update ItemManager from editor");
            return;
        }
        GameObject item = Instantiate(prefab);
        item.transform.SetParent(playerRightHandSlot.transform);
        if (item.TryGetComponent<Flashlight>(out var flashlight))
        {
            flashlight.followTarget = playerRightHandSlot.transform;
        }
        else if (item.TryGetComponent<Keycard>(out var keycard))
        {
            keycard.followTarget = playerRightHandSlot.transform;
        }
        currentlyEquippedItem = item;
    }

    [ServerRpc]
    public void CallUnequipItemServerRpc()
    {
        UnEquipItemLocalClientRpc();
    }
    [ClientRpc]
    private void UnEquipItemLocalClientRpc()
    {
        Destroy(currentlyEquippedItem);
        
        currentlyEquippedItem = null;
        isItemEquipped = false;
    }

    [ServerRpc]
    private void CallDestroyCollectibleServerRpc()
    {
        DestroyCollectibleClientRpc();
    }
    [ClientRpc]
    private void DestroyCollectibleClientRpc()
    {
        if (currentSelectedInteractable != null && currentSelectedInteractable.Value != null)
        {
            currentSelectedInteractable.Value.gameObject.SetActive(false);
            interactablesInRange.Remove(currentSelectedInteractable);
            currentSelectedInteractable = currentSelectedInteractable.Next ?? currentSelectedInteractable.List?.First;
        }
    }


    void Update()
    {
        if ((Health <= 0 || isDead.Value) && !isGameOverScreenActive)
        {
            isDead.Value = true;
            var instantiatedGameOverScreen = Instantiate(GameOverScreenPrefab);
            isGameOverScreenActive = true;
            cursorState = CursorLockMode.None;
            GameOverScreen = instantiatedGameOverScreen.GetComponent<UIDocument>();
            GameOverScreen.sortingOrder = 99999;
        }
        
         if (isGameWon.Value && !isGameOverScreenActive)
        {
            var instantiatedGameOverScreen = Instantiate(GameOverScreenPrefab);
            isGameOverScreenActive = true;
            cursorState = CursorLockMode.None;
            GameOverScreen = instantiatedGameOverScreen.GetComponent<UIDocument>();
            GameOverScreen.rootVisualElement.Q<Label>("Score").text += nbOfDocumentCollected + " Document(s) collected";
            GameOverScreen.rootVisualElement.Q<Label>("Text").text = "Mission Completed!";
            GameOverScreen.sortingOrder = 99999;

        }

        
        // Only count down when not waiting to spawn the next entity
        if (!shouldSpawnEntity)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0)
            {
                shouldSpawnEntity = true;
            }
        }
        // Once door clears the flag, restart the timer
        else if (shouldSpawnEntity == false && spawnTimer <= 0)
        {
            spawnTimer = 120f;
        }
        
        if (!KeyPad.IsAnyKeyPadOpen)
        {
            UnityEngine.Cursor.lockState = cursorState;
        }
        activeInventorySlot = InventoryController.activeInventorySlotId;
        SwitchCurrentInteractable();
        CheckAirborne();

        if(!IsOwner){
            return;
        }

        HealthBar.style.width = Length.Percent(Health);
        EnergyBar.style.width = Length.Percent(Energy);
        
        CheckIfCanGrapple();
        shiftPressed = Input.GetKey(KeyCode.LeftShift);
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        xMouseInput = Input.GetAxis("Mouse X");
        yMouseInput = -Input.GetAxis("Mouse Y");

        if(Input.GetKeyDown(KeyCode.Space) && !isAirborne && !freeze){
            animator.SetBool("IsJumping", true);
            Jump();
        }

        // Maybe ???
        if (currentlyEquippedItem != null) {
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                currentlyEquippedItem.GetComponent<IActionItem>().PrimaryAction(this);
            }
        }

        if(currentSelectedInteractable is not null)
        {
            if(currentSelectedInteractable.Value.gameObject.TryGetComponent<Outline>(out var outline))
            {
                outline.OutlineColor = Color.red;
            }            

            
            if(Input.GetKeyDown(KeyCode.E))
            {
                InteractWith(currentSelectedInteractable.Value);
            }
        }


        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            EquipItem();
        }

        if (Input.GetKeyDown(KeyCode.Tab) && cursorState == CursorLockMode.Locked) {
            cursorState = CursorLockMode.None;
            invController.toggleInventory();
        } else if (Input.GetKeyDown(KeyCode.Tab) && cursorState == CursorLockMode.None) {
            cursorState = CursorLockMode.Locked;
            invController.toggleInventory();

        }

        currentlyEquippedItempublic = currentlyEquippedItem;
    }

    void CheckIfCanGrapple()
    {
        canGrapple = currentlyEquippedItem is not null && currentlyEquippedItem.TryGetComponent<GrapplingHook>(out var hook); //CODE AFFREUX

    }

    public void ResetResctrictions(){
        activeGrapple = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(enableMovementOnNextTouch){
            enableMovementOnNextTouch = false;
            ResetResctrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractable>(out IInteractable myInteractable))
        {
            if(!interactablesInRange.Contains((BaseInteractable)myInteractable))
            {
                interactablesInRange.AddLast((BaseInteractable)myInteractable);
                currentSelectedInteractable = interactablesInRange.First;
            }
        }   
    }

    
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent<IInteractable>(out IInteractable myInteractable))
        {
            if(interactablesInRange.Contains((BaseInteractable)myInteractable))
            {
                interactablesInRange.Remove((BaseInteractable)myInteractable);
            }
            if(interactablesInRange.Count == 0)
            {
                currentSelectedInteractable = null;
            }
        }   
    }
    private void SwitchCurrentInteractable()

    {
        if (currentSelectedInteractable == null)
        {
           // Debug.LogWarning("currentSelectInteractble null");
            return;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            currentSelectedInteractable.Value.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            currentSelectedInteractable = currentSelectedInteractable.Next ?? currentSelectedInteractable.List.First;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            currentSelectedInteractable.Value.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
            currentSelectedInteractable = currentSelectedInteractable.Previous ?? currentSelectedInteractable.List.Last;
        }
    }


    void FixedUpdate()
    {
        if(!IsOwner){
            return;
        }
        if(activeGrapple){
            playerRigidbody.linearDamping = 0;
        }
        else{
            playerRigidbody.linearDamping = 1;
        }
        ControlCamera();

        

        if(!freeze)
        {
            isMoving = xInput != 0 || yInput != 0;
            Animate();
            
            if(xInput != 0 || yInput != 0)
            {
                Move();
            }
            else
            {
                currentSpeed = 0f;
            }
        }
        else
        {
            currentSpeed = 0;
            playerRigidbody.linearVelocity = Vector3.zero;
            
        }
        
    }

    void CheckAirborne()
    {
        RaycastHit hit;
        if(Physics.Raycast(groundCheck.position, Vector3.down, out hit, 3f * transform.localScale.y)){
            isAirborne = false;
        }
        else{
            isAirborne = true;
        }
    }

    void ControlCamera()
    {
        xRotation += yMouseInput * yMouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += xMouseInput * xMouseSensitivity;

        Quaternion verticalRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        playerCameraPivotTransform.localRotation = verticalRotation;

        // Synchroniser la rotation du corps avec la caméra (axe Y uniquement)
        playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }


    void Move()
    {
        if(activeGrapple) return;
        Vector3 myForward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized;
        Vector3 myRight  = new Vector3(playerCamera.right.x, 0, playerCamera.right.z).normalized;

        if(shiftPressed){
            speed = BASE_SPEED * 1.5f;
        }
        else{
            speed = BASE_SPEED;
        }

        currentSpeed = speed;
       

        Vector3 myMovement = (myForward * yInput + myRight * xInput).normalized * currentSpeed;
        Vector3 myVelocity = new Vector3(myMovement.x, playerRigidbody.linearVelocity.y, myMovement.z);
        // if (myMovement != Vector3.zero) 
        // {
        //     Quaternion targetRotation = Quaternion.LookRotation(myMovement, Vector3.up);
        //     playerBody.rotation = Quaternion.RotateTowards(playerBody.rotation, targetRotation, 1000f * Time.deltaTime);
        // }
        playerRigidbody.linearVelocity = myVelocity;
        
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight){

        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    void Animate()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsGrounded", !isAirborne);
        animator.SetFloat("Magnitude", currentSpeed / MAX_SPEED);
        if(playerRigidbody.linearVelocity.y < -0.5f) // i am falling
        {
            animator.SetBool("IsFalling", true);
            animator.SetBool("IsJumping", false);

        }
        else
        {
            animator.SetBool("IsFalling", false);
        }
    }

    void Jump()
    {
        if(!isAirborne)
        {
            playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isAirborne = true;
        }
    }

    public bool CanInteract(IInteractable interactable)
    {
        return true;
    }

    public void InteractWith(IInteractable interactable)
    {
        if(CanInteract(interactable)){
            interactable.AcceptInteraction(handler);
            Debug.Log("TEST");
        }
        if(interactable is CollectableItem){
            CallDestroyCollectibleServerRpc();
        }
    }

    private Vector3 velocityToSet;

    private void SetVelocity(){
        enableMovementOnNextTouch = true;
        playerRigidbody.linearVelocity = velocityToSet;
    }
    
    
    public void TakeDamage(float dmg, float knockback)
    {
        Health -= dmg;
    }

    public void GetDestroyed()
    {
        throw new System.NotImplementedException();
    }

    public class AgentInteractionHandler : IInteractionHandler

    {
        public void InteractWith(BaseInteractable item)
        {
            Debug.Log("tested");
        }
        public void InteractWith(CollectableItem item)
        {
            Debug.Log("collected");
            if (item.item.Name == "Document")
            {
                Debug.Log("itemName");
                DocumentManager.Instance.CollectDocument(item.item.Id);
                nbOfDocumentCollected++;
            }
            InventoryController.Instance.AddItemToInventory(item.item);
            Agent.AddItemToAgentInventory(item.item);
        }

        public void InteractWith(InteractableButton button)
        {
            Debug.Log("Clicked 22");
        }


    }
}
