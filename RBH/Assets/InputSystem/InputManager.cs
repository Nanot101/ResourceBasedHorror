using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : Singleton<InputManager>
{
    PlayerInputActions inputActions;

    #region Player
    public Vector2 MoveInput {  get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool Interact {  get; private set; }
    #endregion

    #region Inventory
    public bool QuickMove { get; private set; }
    public bool OpenInventory { get; private set; }
    public bool PickupItem { get; private set; }
    public bool InteractInventory { get; private set; }
    public bool OpenContextMenu { get; private set; }
    public bool RotateItem { get; private set; }
    #endregion
    
    #region UI
    public bool Cancel { get; private set; }
    public bool OpenNotebook { get; private set; }
    #endregion
    
    #region Weapon
    public bool Fire { get; private set; }
    public bool Reload { get; private set; }
    public bool SwitchWeapon { get; private set; }
    #endregion

    #region Dialogue
    public bool NextDialogue { get; private set; }
    #endregion
    private void Awake()
    {
        inputActions = new PlayerInputActions();
        
        #region Player
        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        inputActions.Player.Sprint.performed += ctx => IsSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => IsSprinting = false;

        inputActions.Player.Interact.performed += ctx => Interact = true;
        #endregion

        #region Inventory
        inputActions.Inventory.QuickMove.performed += ctx => QuickMove = true;
        inputActions.Inventory.QuickMove.canceled += ctx => QuickMove = false;

        inputActions.Inventory.OpenInventory.performed += ctx => OpenInventory = true;

        inputActions.Inventory.PickupItem.performed += ctx => PickupItem = true;

        inputActions.Inventory.Interact.performed += ctx => InteractInventory = true;

        inputActions.Inventory.OpenContextMenu.performed += ctx => OpenContextMenu = true;

        inputActions.Inventory.RotateItem.performed += ctx => RotateItem = true;
        #endregion
        
        #region UI
        inputActions.UI.Cancel.performed += ctx => Cancel = true;

        inputActions.UI.OpenNotebook.performed += ctx => OpenNotebook = true;
        #endregion

        #region Weapon
        inputActions.Weapon.Fire.performed += ctx => Fire = true;

        inputActions.Weapon.Reload.performed += ctx => Reload = true;

        inputActions.Weapon.SwitchWeapon.performed += ctx => SwitchWeapon = true;
        #endregion

        #region Dialogue
        inputActions.Dialogue.NextDialogue.performed += ctx => NextDialogue = true;
        #endregion
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    //Reset one frame inputs
    private void LateUpdate()
    {
        Interact = false;
        Fire = false;
        Reload = false;

        OpenInventory = false;
        PickupItem = false;
        InteractInventory = false;
        OpenContextMenu = false;
        RotateItem = false;

        Cancel = false;
        OpenNotebook = false;

        Fire = false;
        Reload = false;
        SwitchWeapon = false;

        NextDialogue = false;
    }


}
