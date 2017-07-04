/*
 * Author(s): Isaiah Mann
 * Description: Samples from two different batches to generate stimuli
 * Usage: [no notes]
 */

using UnityEngine;

public class HybridTaskBatch : TaskBatch
{
	public override string FirstStimuli
	{
		get
		{
			return batch1.FirstStimuli;
		}
	}

	public override string SecondStimuli
	{
		get
		{
			return batch2.SecondStimuli;
		}
	}

	public override string FirstStimuliCategory1
	{
		get
		{
			return batch1.FirstStimuliCategory1;
		}
	}

	public override string FirstStimuliCategory2
	{
		get
		{
			return batch1.FirstStimuliCategory2;
		}
	}

	public override string SecondStimuliCategory1
	{
		get
		{
			return batch2.SecondStimuliCategory1;
		}
	}

	public override string SecondStimuliCategory2
	{
		get 
		{
			return batch2.SecondStimuliCategory2;
		}
	}

	ImageTaskBatch batch1Img
	{
		get
		{
			return batch1 as ImageTaskBatch;
		}
	}

	ImageTaskBatch batch2Img
	{
		get
		{
			return batch2 as ImageTaskBatch;
		}
	}

	TaskBatch batch1, batch2;

	public HybridTaskBatch(TaskBatch batch1, TaskBatch batch2)
	{
		this.batch1 = batch1;
		this.batch2 = batch2;
	}

	public override StimuliSet GetSet()
	{
		ImageStimuliSet set = new ImageStimuliSet();
		StimuliSet sample1 = batch1.GetSet();
		StimuliSet sample2 = batch2.GetSet();
		if(sample1 is ImageStimuliSet && (sample1 as ImageStimuliSet).HasImage1)
		{
			set.Stimuli1Img = (sample1 as ImageStimuliSet).Stimuli1Img;
		}
		else
		{
			set.Stimuli1 = sample1.Stimuli1;
		}
		if(sample2 is ImageStimuliSet && (sample2 as ImageStimuliSet).HasImage2)
		{
			set.Stimuli2Img = (sample2 as ImageStimuliSet).Stimuli2Img;
		}
		else
		{
			set.Stimuli1 = sample2.Stimuli2;
		}
		return set;
	}

	public override int GetStimuli1Index(string stimuli)
	{
		return batch1.GetStimuli1Index(stimuli);
	}

	public override int GetStimuli2Index(string stimuli)
	{
		return batch2.GetStimuli2Index(stimuli);
	}

	public override bool IsValidStimuli1Category1(string stimuli)
	{
		return batch1.IsValidStimuli1Category1(stimuli);
	}

	public override bool IsValidStimuli1Category2(string stimuli)
	{
		return batch1.IsValidStimuli1Category2(stimuli);
	}

	public override bool IsValidStimuli2Category1(string stimuli)
	{
		return batch2.IsValidStimuli2Category1(stimuli);
	}

	public override bool IsValidStimuli2Category2(string stimuli)
	{
		return batch2.IsValidStimuli2Category2(stimuli);
	}

	public bool IsValidStimuli1Category1(Sprite stimuli)
	{
		return batch1Img.IsValidStimuli1Category1(stimuli);
	}

	public bool IsValidStimuli1Category2(Sprite stimuli)
	{
		return batch1Img.IsValidStimuli1Category2(stimuli);
	}

	public bool IsValidStimuli2Category1(Sprite stimuli)
	{
		return batch2Img.IsValidStimuli2Category1(stimuli);
	}

	public bool IsValidStimuli2Category2(Sprite stimuli)
	{
		return batch2Img.IsValidStimuli2Category2(stimuli);
	}

	public int GetStimuli1Index(Sprite stimuli)
	{
		return batch1Img.GetStimuli1Index(stimuli);
	}

	public int GetStimuli2Index(Sprite stimuli)
	{
		return batch2Img.GetStimuli2Index(stimuli);
	}

}
