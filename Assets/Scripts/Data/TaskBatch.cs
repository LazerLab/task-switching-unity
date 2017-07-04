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
	public virtual string FirstStimuli
	{
		get
		{
			return _firstStimuli;
		}
	}

	public virtual string SecondStimuli
	{
		get
		{
			return _secondStimuli;
		}
	}

	public virtual string FirstStimuliCategory1
	{
		get
		{
			return _firstStimuliCategory1;
		}
	}

	public virtual string FirstStimuliCategory2
	{
		get
		{
			return _firstStimuliCategory2;
		}
	}

	public virtual string SecondStimuliCategory1
	{
		get
		{
			return _secondStimuliCategory1;
		}
	}

	public virtual string SecondStimuliCategory2
	{
		get
		{
			return _secondStimuliCategory2;
		}
	}

	string _firstStimuli;
	string _secondStimuli;
	string _firstStimuliCategory1, _firstStimuliCategory2;
	string _secondStimuliCategory1, _secondStimuliCategory2;

	string[] firstStimuliCategory1Options, firstStimuliCategory2Options;
	string[] secondStimuliCategory1Options, secondStimuliCategory2Options;
	string[] firstStimuliOptions;
	string[] secondStimuliOptions;

	Action onStimuliFetched;
	bool finalFetchCallbackFired = false;
	VariableFetcher fetcher;

	public TaskBatch(string batchKey, Action onStimuliFetched)
	{
		earlyInit();
		this.batchKey = batchKey;
		this.onStimuliFetched = onStimuliFetched;
		this.fetcher = VariableFetcher.Get;
		fetcher.GetValueList(batchKey, getStimuliNames);
	}

	// Empty constructor for use by subclasses
	protected TaskBatch(){}

	public virtual StimuliSet GetSet()
	{
		return new StimuliSet(randomStimuli1(), randomStimuli2());
	}

	public virtual int GetStimuli1Index(string stimuli)
	{
		return ArrayUtil.IndexOf(firstStimuliOptions, stimuli);
	}

	public virtual int GetStimuli2Index(string stimuli)
	{
		return ArrayUtil.IndexOf(secondStimuliOptions, stimuli);
	}

	public virtual bool IsValidStimuli1Category1(string stimuli)
	{
		return ArrayUtil.Contains(firstStimuliCategory1Options, stimuli);
	}

	public virtual bool IsValidStimuli1Category2(string stimuli)
	{
		return ArrayUtil.Contains(firstStimuliCategory2Options, stimuli);
	}

	public virtual bool IsValidStimuli2Category1(string stimuli)
	{
		return ArrayUtil.Contains(secondStimuliCategory1Options, stimuli);
	}

	public virtual bool IsValidStimuli2Category2(string stimuli)
	{
		return ArrayUtil.Contains(secondStimuliCategory2Options, stimuli);
	}

	protected virtual void earlyInit()
	{
		// NOTHING
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
		_firstStimuli = names[0];
		_secondStimuli = names[1];
		fetcher.GetValueList(FirstStimuli, getFirstStimuliCategories);
		fetcher.GetValueList(SecondStimuli, getSecondStimuliCategories);
	}

	void getFirstStimuliCategories(string[] categories)
	{
		_firstStimuliCategory1 = categories[0];
		_firstStimuliCategory2 = categories[1];
		fetcher.GetValueList(FirstStimuliCategory1, getFirstStimuliCategory1Options);
		fetcher.GetValueList(FirstStimuliCategory2, getFirstStimuliCategory2Options);
	}

	void getSecondStimuliCategories(string[] categories)
	{
		_secondStimuliCategory1 = categories[0];
		_secondStimuliCategory2 = categories[1];
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
