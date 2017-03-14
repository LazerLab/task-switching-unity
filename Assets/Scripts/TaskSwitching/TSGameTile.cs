/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class TSGameTile : TSGameObject 
{	
	[SerializeField]
	GameObject successIcon;
	[SerializeField]
	GameObject failureIcon;
	[SerializeField]
	float timeShowIcon = 1f;

	public TSMatchCondition GetMatchCondition
	{
		get
		{
			return this.tileMatchCondition;
		}
	}
		
	public TSMatchType GetMatchType
	{
		get
		{
			if(this.tileMatchCondition == TSMatchCondition.ConsonantLetter || 
				this.tileMatchCondition == TSMatchCondition.VowelLetter)
			{
				return TSMatchType.Letter;
			}
			else if(this.tileMatchCondition == TSMatchCondition.EvenNumber ||
				this.tileMatchCondition == TSMatchCondition.OddNumber)
			{
				return TSMatchType.Number;
			}
			else
			{
				return default(TSMatchType);
			}
		}
	}

	public bool HasPiece
	{
		get
		{
			return activePiece != null;
		}
	}

	public TSGamePiece GetPiece
	{
		get
		{
			return activePiece;
		}
	}

	[SerializeField]
	TSMatchCondition tileMatchCondition;

	TSGamePiece activePiece;

	public void SetPiece(TSGamePiece piece)
	{
		activePiece = piece;
	}

	public void ClearPiece()
	{
		activePiece.ToggleVisible(isVisibile:false);
		activePiece = null;
	}

	public void TimedShowIcon(bool successfulPlacement)
	{
		// Cleanup to ensure all icons are hidden and no extra coroutines are running
		StopAllCoroutines();
		successIcon.SetActive(false);
		failureIcon.SetActive(false);

		GameObject icon = successfulPlacement ? successIcon : failureIcon;
		icon.SetActive(true);
		StartCoroutine(showIconRoutine(icon, timeShowIcon));
	}

	IEnumerator showIconRoutine(GameObject icon, float time)
	{
		yield return new WaitForSeconds(time);
		icon.SetActive(false);
	}

}
