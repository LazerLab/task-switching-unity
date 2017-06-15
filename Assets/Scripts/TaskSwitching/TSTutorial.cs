/*
 * Author(s): Isaiah Mann
 * Description: Tutorial instructions for Task Switching
 * Usage: [no notes]
 */

public class TSTutorial 
{
	string[] steps;
	int stepIndex = 0;

	public string CurrentStep()
	{
		return steps[stepIndex];
	}

	public string NextStep()
	{
		if(HasNext())
		{
			return steps[++stepIndex];
		}
		else
		{
			return CurrentStep();
		}
	}

	public bool HasNext()
	{
		return stepIndex + 1 < steps.Length;
	}

	public string PreviousStep()
	{
		if(HasPrevious())
		{
			return steps[--stepIndex];
		}
		else
		{
			return CurrentStep();
		}
	}

	public bool HasPrevious()
	{
		return stepIndex - 1 >= 0;
	}

	public void Init(int numSteps)
	{
		steps = new string[numSteps];
	}

	public void SetStep(int stepIndex, string value)
	{
		steps[stepIndex] = value;
	}

	public bool TutorialLoaded()
	{
		if(steps != null)
		{
			for(int i = 0; i < steps.Length; i++)
			{
				if(steps[i] == null)
				{
					return false;
				}
			}
			return true;
		}
		else
		{
			return false;
		}
	}

}
