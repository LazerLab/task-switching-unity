/*
 * Author(s): Isaiah Mann
 * Description: Describes a batch of task, featuring two different stimuli which are paired together to be categorized
 * Usage: [no notes]
 */

using System;

using UnityEngine;

using VolunteerScience;

public class TaskBatch 
{
	#region Instance Accessors 

	public string BatchKey
	{
		get
		{
			return this.batchKey;
		}
	}

	#endregion

	string batchKey;
	string firstStimuli;
	string secondStimuli;
	string firstStimuliCategory1, firstStimuliCategory2;
	string secondStimuliCategory1, secondStimuliCategory2;
	string[] firstStimuliCategory1Options, firstStimuliCategory2Options;
	string[] secondStimuliCategory1Options, secondStimuliCategory2Options;

	Action onStimuliFetched;
	bool finalFetchCallbackFired = false;
	VariableFetcher fetcher;

	public TaskBatch(string batchKey, Action onStimuliFetched)
	{
		this.batchKey = batchKey;
		this.onStimuliFetched = onStimuliFetched;
		this.fetcher = VariableFetcher.Get;
		fetcher.GetValueList(batchKey, getStimuliNames);
	}

	void getStimuliNames(string[] names)
	{
		firstStimuli = names[0];
		secondStimuli = names[1];
		fetcher.GetValueList(firstStimuli, getFirstStimuliCategories);
		fetcher.GetValueList(secondStimuli, getSecondStimuliCategories);
	}

	void getFirstStimuliCategories(string[] categories)
	{
		firstStimuliCategory1 = categories[0];
		firstStimuliCategory2 = categories[1];
		fetcher.GetValueList(firstStimuliCategory1, getFirstStimuliCategory1Options);
		fetcher.GetValueList(firstStimuliCategory2, getFirstStimuliCategory2Options);
	}

	void getSecondStimuliCategories(string[] categories)
	{
		secondStimuliCategory1 = categories[0];
		secondStimuliCategory2 = categories[1];
		fetcher.GetValueList(secondStimuliCategory1, getSecondStimuliCategory1Options);
		fetcher.GetValueList(secondStimuliCategory2, getSecondStimuliCategory2Options);
	}

	void getFirstStimuliCategory1Options(string[] options)
	{
		firstStimuliCategory1Options = options;
		checkToRunCallback();
	}

	void getFirstStimuliCategory2Options(string[] options)
	{
		firstStimuliCategory2Options = options;
		checkToRunCallback();
	}

	void getSecondStimuliCategory1Options(string[] options)
	{
		secondStimuliCategory1Options = options;
		checkToRunCallback();
	}

	void getSecondStimuliCategory2Options(string[] options)
	{
		secondStimuliCategory2Options = options;
		checkToRunCallback();
	}

	void checkToRunCallback()
	{
		if(shouldRunCallback())
		{
			onStimuliFetched();
			Debug.Log(ArrayUtil.ToString(firstStimuliCategory1Options));
			Debug.Log(ArrayUtil.ToString(firstStimuliCategory2Options));
			Debug.Log(ArrayUtil.ToString(secondStimuliCategory1Options));
			Debug.Log(ArrayUtil.ToString(secondStimuliCategory2Options));
			finalFetchCallbackFired = true;
		}
	}

	bool shouldRunCallback()
	{
		return firstStimuliCategory1Options != null &&
			firstStimuliCategory2Options != null &&
			secondStimuliCategory1Options != null &&
			secondStimuliCategory2Options != null &&
			!finalFetchCallbackFired;
	}

}
