using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The animations used for the character contains built in messages sent to these functions, 
/// these can be used for those to do something
/// </summary>
public class MovementSounds : MonoBehaviour
{
    public UnityEvent onLand;
    public UnityEvent onFootstep;
    void OnLand()
    {
        onLand.Invoke();
    }

    void OnFootstep()
    {
        onFootstep.Invoke();
    }
}
