/*
 * Author(s): Isaiah Mann
 * Description: Receives messages from the Volunteer Science platform
 * Usage: [no notes]
 */

namespace VolunteerScience
{	
	using UnityEngine;

	public class MessageReceiver : Singleton<MessageReceiver>
	{
		const string READY_FUNC = "receiverReady";

		protected override void Awake()
		{
			base.Awake();
			Application.ExternalCall(READY_FUNC);
		}

		public void Initialize()
		{
			ExperimentController.Get.Initialize();
		}

	}

}
