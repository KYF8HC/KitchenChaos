using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance{ get; private set; }
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounter selectedCounter;
    }

    [SerializeField]private float moveSpeed = 7f;
    [SerializeField]private GameInput gameInput;
    [SerializeField]private LayerMask countersLayermask;
    [SerializeField]private Transform kitchenObjectHoldPoint;

    private bool isWalking = false;
    private Vector3 lastInteractDirection;
    private ClearCounter selectedCounter;
    private KitchenObject kitchenObject;


    public bool IsWalking()
    {
        return isWalking;
    }

    private void Awake() 
    {
        if(Instance != null)
        {
            Debug.LogError("There is more than one Player instance");
        }
        Instance = this;
    }
    private void Start() 
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
    }
    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if(selectedCounter != null)
        {
            selectedCounter.Interact(this);
        }
    }
    private void Update()
    {
        HandelMovement();
        HandelInteractions();
    }  
    private void HandelInteractions()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if(moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        float interactDistance = 2f;
        if(Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayermask))
        {
            if(raycastHit.transform.TryGetComponent(out ClearCounter clearCounter))
            {
                if(clearCounter != selectedCounter)
                {
                    SetSelectedCounter(clearCounter);
                }
            } else
            {
                SetSelectedCounter(null);
            } 
        } else
        {
            SetSelectedCounter(null);
        } 
    }
    private void HandelMovement()
    {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, 
                                            transform.position + Vector3.up * playerHeight,
                                            playerRadius, 
                                            moveDir, 
                                            moveDistance);
        if(!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, 
                                            transform.position + Vector3.up * playerHeight,
                                            playerRadius, 
                                            moveDirX, 
                                            moveDistance);
            if(canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, 
                                            transform.position + Vector3.up * playerHeight,
                                            playerRadius, 
                                            moveDirZ, 
                                            moveDistance);
                if(canMove)
                {
                    moveDir = moveDirZ; 
                }
            }
        }
        if(canMove)
        {
            transform.position += moveDir * moveDistance;
        }
        isWalking = moveDir != Vector3.zero;
        float rotateSpeed = 11f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, 
                                        Time.deltaTime * rotateSpeed);
    }
    private void SetSelectedCounter(ClearCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() 
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) 
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject() 
    {
        return kitchenObject;
    }

    public void ClearKitchenObject() 
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject() 
    {
        return kitchenObject != null;
    }
}