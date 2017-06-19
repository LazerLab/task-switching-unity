/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;
using UnityEngine.UI;

public class TSUIController : SingletonController<TSUIController>
{
	[SerializeField]
	GameObject completeMessage;

	[SerializeField]
	Text stimuli1Category1, stimuli1Category2, stimuli2Category1, stimuli2Category2;

	[SerializeField]
	Text[] stimuli1, stimuli2;

	public void SetLabels(TaskBatch batch)
	{
		stimuli1Category1.text = batch.FirstStimuliCategory1;
		stimuli1Category2.text = batch.FirstStimuliCategory2;
		stimuli2Category1.text = batch.SecondStimuliCategory1;
		stimuli2Category2.text = batch.SecondStimuliCategory2;
		foreach(Text t in stimuli1)
		{
			t.text = batch.FirstStimuli;
		}
		foreach(Text t in stimuli2)
		{
			t.text = batch.SecondStimuli;
		}
	}

	public void ShowComplete()
	{
		completeMessage.SetActive(true);
	}

}
