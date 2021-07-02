using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuIndicatorBehavior : MonoBehaviour
{
	public List<GameObject> buttons = new List<GameObject>();
	public Vector2 indicatorOffset = new Vector2();

	void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		if (buttons.Count == 0)
		{
			Debug.LogError("No buttons associated with the MenuIndicatorBehavior: " + this.gameObject.name);
		}

		ChangeIndicatorPosition(0);

		foreach (GameObject button in buttons)
		{
			EventTrigger trigger = button.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener((data) => { OnSelectionHover((PointerEventData)data); });
			trigger.triggers.Add(entry);
		}
	}

	public void LockInput(InputField inputField)
	{
		inputField.onEndEdit.RemoveAllListeners();
		isEditingInputField = 2;
	}

	// Update is called once per frame
	void Update()
	{
		int selectionChange = 0;
		if (isEditingInputField == 1)
		{
			return;
		}

		if (Input.GetButtonUp("select"))
		{
			if (isEditingInputField == 2)
			{
				isEditingInputField = 0;
				return;
			}

			Button button = buttons[currentSelection].GetComponent<Button>();
			if (button)
			{
				button.onClick.Invoke();
			}
			else
			{
				InputField inputField = buttons[currentSelection].GetComponent<InputField>();
				if (inputField)
				{
					isEditingInputField = 1;
					inputField.onEndEdit.AddListener(delegate { LockInput(inputField); });
					inputField.Select();
				}
			}
		}
		else if (Input.GetButtonUp("up"))
		{
			selectionChange = -1;
		}
		else if (Input.GetButtonUp("down"))
		{
			selectionChange = 1;
		}

		if (selectionChange != 0)
		{
			currentSelection += selectionChange;
			if (currentSelection >= buttons.Count)
			{
				currentSelection = 0;
			}
			else if (currentSelection < 0)
			{
				currentSelection = buttons.Count - 1;
			}

			ChangeIndicatorPosition(currentSelection);
		}
	}

	public void OnSelectionHover(PointerEventData p)
	{
		GameObject thisObject;
		foreach (GameObject obj in p.hovered)
		{
			thisObject = obj;
			while (thisObject != null)
			{
				foreach (GameObject button in buttons)
				{
					if (thisObject == button)
					{
						currentSelection = buttons.IndexOf(button);
						ChangeIndicatorPosition(currentSelection);
						return;
					}
				}
				thisObject = thisObject.transform.parent == null ? null : thisObject.transform.parent.gameObject;
			}
		}
	}

	private void ChangeIndicatorPosition(int index)
	{
		RectTransform newRectTransform = buttons[index].GetComponent<RectTransform>();

		Vector3[] vectors = new Vector3[4];
		newRectTransform.GetWorldCorners(vectors);
		Vector3 midLeft = new Vector3(vectors[0].x, ((vectors[1].y - vectors[0].y) * 0.5f) + vectors[0].y, vectors[0].z);
		rectTransform.position = new Vector3(midLeft.x + indicatorOffset.x,
			midLeft.y + indicatorOffset.y,
			midLeft.z);
	}

	private RectTransform rectTransform;
	private int currentSelection = 0;
	private int isEditingInputField = 0;
}
