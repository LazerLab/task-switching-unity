/*
 * Author(s): Isaiah Mann
 * Description: Controls the overall flow and logic of experiments
 * Usage: [no notes]
 */

namespace VolunteerScience
{
    using System;

    using UnityEngine;

    public class ExperimentController : Singleton<ExperimentController>
    {
		// Keys and functions used by the JavaScript of the Volunteer Science Unity Integration
        const string COMPLETE_EXPERIMENT_FUNC = "completeExperiment";
		const string SET_ROUND_FUNC = "setRound";
        const string ROUND_KEY = "vs_round";
		const string SEED_KEY = "vs_seed";

		// Delegate function to run when Volunteer Science sends the initialize event
		Action onInitialize;

		public void Initialize()
		{
			if(onInitialize != null)
			{
				onInitialize();
			}
		}

		// Any function with a void() signature can be subscribed to run when Volunteer Science initializes
		public void SubscribeToInitialize(Action callback)
		{
			onInitialize += callback;
		}

		// Unsubscribes functions from running on Initialize
		public void UnsubscribeFromInitialize(Action callback)
		{
			onInitialize -= callback;
		}

		// Sends message to Volunteer Science that experiment has been completed
        public void CompleteExperiment()
        {
            Application.ExternalCall(COMPLETE_EXPERIMENT_FUNC);
        }

		// Sets the round number within Volunteer Science
		public void SetRound(int roundId)
		{
			Application.ExternalCall(SET_ROUND_FUNC, roundId);
		}

		// Gets the current round number from Volunteer Science
        public IntFetchAction GetRound(Action<int> callback)
        {
            return VariableFetcher.Get.GetInt(ROUND_KEY, callback);
        }

		// Gets a random seed from Volunteer Science
		public IntFetchAction GetSeed(Action<int> callback)
		{
			return VariableFetcher.Get.GetInt(SEED_KEY, callback);
		}

    }

}
