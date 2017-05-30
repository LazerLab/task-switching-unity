/*
 * Author(s): Isaiah Mann
 * Description: Used for the callback phase when Volunteer Science returns a variable to Unity
 * Usage: VariableFetchAction attaches an instance of this script to a randomely created GameObject with a unique name
 * Usage (CONT): This leverages the SendMessage(gameObject, functionName, arguments); JavaScript call in WebGL
 */

namespace VolunteerScience
{
    using UnityEngine;

	// Attached to a randomely created GameObject
    public class VariableReceiveHandler : MonoBehaviour
    {
        VariableFetchAction fetcher;

		// Should be called when this script is created
        public void Set(VariableFetchAction fetcher)
        {
            this.fetcher = fetcher;
        }
            
        public void Receive(object value)
        {
			// Passes the received value back to the FetchAction
            fetcher.RunCallback(value);   
            // Destroys this object:
            fetcher.Complete();
        }

    }

}
