var FETCH_KEY = "vs_fetch";
var SUBMIT_KEY = "vs_submit";
var JOIN_CHAR = ":";

function isFetchEvent(eventData)
{
     return eventData.includes(FETCH_KEY);
}

function isSubmitEvent(eventData)
{
     return eventData.includes(SUBMIT_KEY);
}
