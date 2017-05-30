/*
 * Author(s): Isaiah Mann
 * Description: Retrieves information about the players
 * Usage: [no notes]
 */

namespace VolunteerScience
{
	using System;
	using System.Collections;

	public class PlayerController : Singleton<PlayerController>
	{
		// Keys used by the JavaScript code in the Volunteer Science Unity Integration
		const string PLAYER_ID_KEY = "vs_player_id";
		const string PLAYER_NAME_KEY = "vs_player_name";
		const string PLAYER_COUNT_KEY = "vs_player_count";

		// Returns the number of players in the experiment 
		public IntFetchAction GetPlayerCount(Action<int> callback)
		{
			return VariableFetcher.Get.GetInt(PLAYER_COUNT_KEY, callback);
		}

		// Gets the ID number of the current player
		public IntFetchAction GetMyID(Action<int> callback)
		{
			return VariableFetcher.Get.GetInt(PLAYER_ID_KEY, callback);
		}
			
		// Gets the name of the current player
		// Requires two callbacks, because we need to know the player's ID to get their name
		// Cannot return an action because the action is not created until the first callback runs
		public void GetName(Action<string> callback)
		{
			GetMyID(
				delegate(int playerId)
				{
					GetName(playerId, callback);
				}
			);
		}

		// Gets the name of a player corresponding to an ID number
		public StringFetchAction GetName(int playerID, Action<string> callback)
		{
			return VariableFetcher.Get.GetString(getPlayerNameKey(playerID), callback);
		}

		// Formats the player ID as a key to be passed to the JavaScript
		string getPlayerNameKey(int playerID)
		{
			return VariableFetcher.Get.FormatFetchCall(PLAYER_NAME_KEY, playerID.ToString());
		}

	}

}
