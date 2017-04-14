/**
 * @author: Isaiah Mann
 * @desc: Used to communicate between Unity iFrame and Volunteer Science host page (player-side)
 * @requires: vs-unity-globals.js, both scripts should be added as a script to the index.html of the WebGL build
 */

 unityFetchCallbacks = {};

class UnityCallback
{
     constructor(gameObject, callbackFunction)
     {
         this.gameObject = gameObject;
         this.callbackFunction = callbackFunction;
     }
}

 function receiveEvent(event)
 {
      if(isFetchEvent(event.data))
      {
           handleFetchEventCallback(event.data);
      }
 }

function handleFetchEventCallback(eventData)
{
     // Should return: ["vs_fetch", key, value]
     var message = eventData.split(JOIN_CHAR);
     var key = message[1];
     var value = message[2];
     var callback = unityFetchCallbacks[key];
     // Unity defined function:
     SendMessage(callback.gameObject, callback.callbackFunction, value);
}

function submit(data)
{
     parent.window.postMessage(SUBMIT_KEY + JOIN_CHAR + data, "*");
}

function fetch(key, gameObject, callbackFunction)
{
     unityFetchCallbacks[key] = new UnityCallback(gameObject, callbackFunction);
     parent.window.postMessage(FETCH_KEY + JOIN_CHAR + key, "*");
}

window.addEventListener("message", receiveEvent, false);
