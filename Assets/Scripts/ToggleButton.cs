using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToggleButton : MonoBehaviour {
	private GameObject buttonContainer;
	private bool isOn = false;
	private Button myButton;
	private Color offColor = Color.white;
	private Color onColor = Color.green;

	void Start() {
		myButton = gameObject.GetComponent<Button>();
		buttonContainer = gameObject.transform.parent.gameObject;
	}

	public void Click() {
		RectTransform child;
		Button childButton;

		// Loop over all of the buttons in the container and, if they're on, turn them off
		for (int i = 0; i < buttonContainer.transform.childCount; ++i) {
			child = buttonContainer.transform.GetChild(i) as RectTransform;
			childButton = child.GetComponent<Button>();
			ToggleButton childToggleButton;

			if (childButton != myButton) {
				childToggleButton = childButton.GetComponent<ToggleButton>();
				if (childToggleButton.GetIsOn()) {
					childToggleButton.TurnOff();
				}
			}
		}

		isOn = true;

		myButton.image.color = onColor;
	}

	public bool GetIsOn() {
		return isOn;
	}

	public void TurnOff() {
		myButton.image.color = offColor;
	}
}
