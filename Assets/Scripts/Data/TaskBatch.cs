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
	public string FirstStimuli;
	public string SecondStimuli;
	public string FirstStimuliCategory1, FirstStimuliCategory2;
	public string SecondStimuliCategory1, SecondStimuliCategory2;
	string[] firstStimuliCategory1Options, firstStimuliCategory2Options;
	string[] secondStimuliCategory1Options, secondStimuliCategory2Options;
	string[] firstStimuliOptions;
	string[] secondStimuliOptions;

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

	public StimuliSet GetSet()
	{
		return new StimuliSet(randomStimuli1(), randomStimuli2());
	}

	public int GetStimuli1Index(string stimuli)
	{
		return ArrayUtil.IndexOf(firstStimuliOptions, stimuli);
	}

	public int GetStimuli2Index(string stimuli)
	{
		return ArrayUtil.IndexOf(secondStimuliOptions, stimuli);
	}

	public bool IsValidStimuli1Category1(string stimuli)
	{
		return ArrayUtil.Contains(firstStimuliCategory1Options, stimuli);
	}

	public bool IsValidStimuli1Category2(string stimuli)
	{
		return ArrayUtil.Contains(firstStimuliCategory2Options, stimuli);
	}

	public bool IsValidStimuli2Category1(string stimuli)
	{
		return ArrayUtil.Contains(secondStimuliCategory1Options, stimuli);
	}

	public bool IsValidStimuli2Category2(string stimuli)
	{
		return ArrayUtil.Contains(secondStimuliCategory2Options, stimuli);
	}

	string randomStimuli1()
	{
		return firstStimuliOptions[UnityEngine.Random.Range(0, firstStimuliOptions.Length)];
	}

	string randomStimuli2()
	{
		return secondStimuliOptions[UnityEngine.Random.Range(0, secondStimuliOptions.Length)];
	}

	void getStimuliNames(string[] names)
	{
		FirstStimuli = names[0];
		SecondStimuli = names[1];
		fetcher.GetValueList(FirstStimuli, getFirstStimuliCategories);
		fetcher.GetValueList(SecondStimuli, getSecondStimuliCategories);
	}

	void getFirstStimuliCategories(string[] categories)
	{
		FirstStimuliCategory1 = categories[0];
		FirstStimuliCategory2 = categories[1];
		fetcher.GetValueList(FirstStimuliCategory1, getFirstStimuliCategory1Options);
		fetcher.GetValueList(FirstStimuliCategory2, getFirstStimuliCategory2Options);
	}

	void getSecondStimuliCategories(string[] categories)
	{
		SecondStimuliCategory1 = categories[0];
		SecondStimuliCategory2 = categories[1];
		fetcher.GetValueList(SecondStimuliCategory1, getSecondStimuliCategory1Options);
		fetcher.GetValueList(SecondStimuliCategory2, getSecondStimuliCategory2Options);
	}

	protected virtual void getFirstStimuliCategory1Options(string[] options)
	{
		firstStimuliCategory1Options = options;
		checkToRunCallback();
	}

	protected virtual void getFirstStimuliCategory2Options(string[] options)
	{
		firstStimuliCategory2Options = options;
		checkToRunCallback();
	}

	protected virtual void getSecondStimuliCategory1Options(string[] options)
	{
		secondStimuliCategory1Options = options;
		checkToRunCallback();
	}

	protected virtual void getSecondStimuliCategory2Options(string[] options)
	{
		secondStimuliCategory2Options = options;
		checkToRunCallback();
	}

	protected void checkToRunCallback()
	{
		if(shouldRunCallback())
		{
			runCallback();
		}
	}

	protected virtual bool shouldRunCallback()
	{
		return firstStimuliCategory1Options != null &&
			firstStimuliCategory2Options != null &&
			secondStimuliCategory1Options != null &&
			secondStimuliCategory2Options != null &&
			!finalFetchCallbackFired;
	}

	protected virtual void runCallback()
	{
		onStimuliFetched();
		firstStimuliOptions = ArrayUtil.Concat(firstStimuliCategory1Options, firstStimuliCategory2Options);
		secondStimuliOptions = ArrayUtil.Concat(secondStimuliCategory1Options, secondStimuliCategory2Options);
		finalFetchCallbackFired = true;
	}

}

public class StimuliSet
{
	public string Stimuli1;
	public string Stimuli2;

	protected StimuliSet(){}

	public StimuliSet(string stimuli1, string stimuli2)
	{
		this.Stimuli1 = stimuli1;
		this.Stimuli2 = stimuli2;
	}

}
