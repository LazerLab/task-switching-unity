/**
 * @author: Isaiah Mann
 * @desc: Used to communicate between Unity iFrame and Volunteer Science host page (player-side)
 * @requires: vs-unity-globals.js, both scripts should be added as a script to the index.html of the WebGL build
 */

// Constants related to send messages to Unity
var MESSAGE_RECEIVER_GAME_OBJECT = "VSReceiver";
var INIT_FUNC = "Initialize";
var RECEIVER_TIME_OUT = 10000; // In milliseconds
var RECEIVER_WAIT_TIME = 100;

// A dictionary of callbacks to run when a requested variable is returned by Volunteer Science
var unityFetchCallbacks = {};
var fetchCount = 0;

// Values used to indicate the status of the Message Receiver object within Unity
var receiverIsReady = false;
var recieverAccessTimer = 0;

// Class to store the necessary info for a callback to Unity
class UnityCallback
{
     // Uses the `SendMessage` function in Unity Web GL to call a function on the GameObject by name
     constructor(gameObject, callbackFunction)
     {
         this.gameObject = gameObject;
         this.callbackFunction = callbackFunction;
     }
}

// Event handler for messages sent by the Volunteer Science host
function receiveEvent(event)
{
     if(isFetchEvent(event.data))
     {
          handleFetchEventCallback(event.data);
     }
     else if(isInitEvent(event.data))
     {
          initialize();
     }
}

// Sends requested data back to Unity fomr a fetch call
function handleFetchEventCallback(eventData)
{
     // Should return: ["vs_fetch", key, id, value]
     var message = eventData.split(JOIN_CHAR);
     var key = message[1];
     var id = message[2];
     var value = message[3];
     var callback = unityFetchCallbacks[key + JOIN_CHAR + id];
     // Unity defined function:
     SendMessage(callback.gameObject, callback.callbackFunction, value);
}

// Sends message to Volunteer Science via "submit" call
function submit(data)
{
     parent.window.postMessage(SUBMIT_KEY + JOIN_CHAR + data, "*");
}

// Retrieves a variable corresponding to a particular key from Volunteer Science
function fetch(key, gameObject, callbackFunction)
{
     var callbackKey = getCallbackKey(key);
     var args;
     if(key.includes(JOIN_CHAR))
     {
          args = key.split(JOIN_CHAR);
          args = args.slice(1, args.length);
          args = args.join(JOIN_CHAR);
     }
     else
     {
          args = "";
     }
     unityFetchCallbacks[callbackKey] = new UnityCallback(gameObject, callbackFunction);
     parent.window.postMessage(FETCH_KEY + JOIN_CHAR + callbackKey + JOIN_CHAR + args, "*");
}

function getCallbackKey(fetchKey)
{
     var key = fetchKey.split(JOIN_CHAR)[0] + JOIN_CHAR + fetchCount;
     fetchCount++;
     return key;
}

// To be called when the player completes an experiment in Volunteer Science
function completeExperiment()
{
     parent.window.postMessage(COMPLETE_KEY, "*");
}

// Sets the round of the experiment to a specified integer
function setRound(roundId)
{
     parent.window.postMessage(SET_KEY + JOIN_CHAR + ROUND_KEY + JOIN_CHAR + roundId, "*");
}

// Sets a consumable used
function setConsumables(consumablesClass, consumablesSet, consumableChoice)
{
     parent.window.postMessage(
          formatArguments([SET_CONSUMABLES_KEY,
               consumablesClass, consumablesSet, consumableChoice]), "*");
}

// Calls Initialize() within Unity upon receiving a message from Volunteer Science
function initialize()
{
     // Times out after a specified period
     // Timeout is for the edge case in which there is no VSReceiver prefab inside the Unity game
     if(recieverAccessTimer < RECEIVER_TIME_OUT)
     {
          // Waits until the VSReceiver object within Unity has been initialized
          if(receiverIsReady)
          {
               // Calls the Initialize() function on the VSReceiver GameObject
               SendMessage(MESSAGE_RECEIVER_GAME_OBJECT, INIT_FUNC);
          }
          else
          {
               // If the receiver is not yet ready, try this call again after a designated number of milliseconds
               window.setTimeout(initialize, RECEIVER_WAIT_TIME);
               // Update how long this function has been running in order to trigger time out if necessary
               recieverAccessTimer += RECEIVER_WAIT_TIME;
          }
     }
}

// To be called from within Unity (automatically by the VSReceiver prefab) to indicate the Receiver object exists
function receiverReady()
{
     receiverIsReady = true;
}

// Sets up the event handler for when the Unity Player page receives messages via postMessage
window.addEventListener("message", receiveEvent, false);
