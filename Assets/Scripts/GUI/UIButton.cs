/*
 * Author(s): Isaiah Mann
 * Description: [to be added]
 * Usage: [no notes]
 */

using System;

using UnityEngine;
using UnityEngine.UI;

public class UIButton : UIElement 
{	
    public bool IsPressed
    {
        get
        {
            return this.isPressed;
        }
    }

	[SerializeField]
	Color pressedColor = Color.gray;
	[SerializeField]
	Color activeColor = Color.white;
	[SerializeField]
	Color inactiveColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);

	bool isPressed = false;
	bool isActive = true;

	Action onPress;
	Button button;

	protected override void setReferences()
	{
		base.setReferences();
		button = GetComponent<Button>();
		button.onClick.AddListener(handlePress);
	}

	public void BeginPress()
	{
		SetImageColorOverlay(pressedColor);
		isPressed = true;
	}

	public void EndPress()
	{
		SetImageColorOverlay(isActive ? activeColor : inactiveColor);
		isPressed = false;
	}

	public void ToggleActive(bool isActive)
	{
		if(isActive)
		{
			SetActive();
		}
		else
		{
			SetInactive();
		}

	}

	public void SetInactive()
	{
		SetImageColorOverlay(inactiveColor);
		isActive = false;
	}

	public void SetActive()
	{
		SetImageColorOverlay(activeColor);
		isActive = true;
	}

	public void SubscribeToPress(Action callback)
	{
		onPress += callback;
	}

	public void UnsubscribeFromPress(Action callback)
	{
		onPress -= callback;
	}

	void handlePress()
	{
		if(onPress != null)
		{
			onPress();
		}
	}

}
