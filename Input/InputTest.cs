using UnityEngine;
using UnityEngine.InputSystem;

namespace Prota.Input
{
    public class InputTest : MonoBehaviour
    {
        GameInput input => GetComponent<GameInput>();
        
        void Awake()
        {
            input.AddCallback("123", e => {
                Debug.Log("111");
            });
            
            input.AddCallback("123", e => {
                Debug.Log("222");
            });
            
            input.AddCallback("Move", CallbackA);
            
            input.AddCallback("MouseMove", e => {
                Debug.LogWarning(e.ReadValue<Vector2>());
            });
            
            input.AddCallback("PrimaryAction", e => {
                Debug.Log(Mouse.current.position.ReadValue() + " | " + e.action.activeControl.GetType());
            });
        }
        
        void CallbackA(InputAction.CallbackContext cc)
        {
            Debug.Log(cc.ReadValue<Vector2>());
        }
        
        void OnDestroy()
        {
            
        }
        
        
        
    }
}