/*
 * Author(s): Isaiah Mann
 * Description: UI controller for Task Switching tutorial
 * Usage: [no notes]
 */

using System;

using VolunteerScience;

using UnityEngine;

public class TSTutorialController : SingletonController<TSTutorialController>
{
	const string TUTORIAL_COUNT = "tutorialCount";
	const string TUTORIAL_KEY = "tutorial";

	[SerializeField]
	UIElement tutorialDisplay;
	[SerializeField]
	UIButton skipButton;
	[SerializeField]
	UIButton nextButton;
	[SerializeField]
	UIButton previousButton;
	[SerializeField]
	KeyCode nextKey;
	[SerializeField]
	KeyCode previousKey;
	[SerializeField]
	string skipText = "Skip";
	[SerializeField]
	string endText = "End";

	TSTutorial tutorial;
	bool tutorialComplete;

	public bool TutorialComplete()
	{
		return tutorialComplete;
	}

	protected override void fetchReferences()
	{
		base.fetchReferences();
		skipButton.SubscribeToPress(endTutorial);
		nextButton.SubscribeToPress(nextStep);
		previousButton.SubscribeToPress(previousStep);
		loadTutorial();
	}

	void Update()
	{
		if(Input.GetKeyDown(nextKey))
		{
			nextStep();
			nextButton.BeginPress();
		}
		else if(Input.GetKeyDown(previousKey))
		{
			previousStep();
			previousButton.BeginPress();
		}
		if(Input.GetKeyUp(nextKey))
		{
			nextButton.EndPress();
		}
		else if(Input.GetKeyUp(previousKey))
		{
			previousButton.EndPress();
		}
	}

	void loadTutorial()
	{
		tutorial = new TSTutorial();
		VariableFetcher fetcher = VariableFetcher.Get;
		fetcher.GetInt(TUTORIAL_COUNT, delegate(int count)
			{
				tutorial.Init(count);
				for(int i = 0; i < count; i++)
				{
					fetcher.GetString(getTutorialKey(i), delegate(string message)
						{
							tutorial.SetStep(i, message);
							if(tutorial.TutorialLoaded())
							{
								startTutorial(tutorial.CurrentStep());
							}
						});
				}
			});
	}

	string getTutorialKey(int index)
	{
		return string.Format("{0}{1}", TUTORIAL_KEY, index);
	}

	void startTutorial(string firstMessage)
	{
		tutorialDisplay.SetText(firstMessage);
		refreshButtons();
	}

	void endTutorial()
	{
		tutorialComplete = true;
		gameObject.SetActive(false);
	}

	void nextStep()
	{
		tutorialDisplay.SetText(tutorial.NextStep());
		refreshButtons();
	}

	void previousStep()
	{
		tutorialDisplay.SetText(tutorial.PreviousStep());
		refreshButtons();
	}

	void refreshButtons()
	{
		nextButton.ToggleActive(tutorial.HasNext());
		previousButton.ToggleActive(tutorial.HasPrevious());
		skipButton.SetText(getSkipButtonText());
	}

	string getSkipButtonText()
	{
		if(tutorial.HasNext())
		{
			return skipText;
		}
		else
		{
			return endText;
		}
	}

}
