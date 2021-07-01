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
		RectTransform newRectTransform = buttons[0].GetComponent<RectTransform>();

		rectTransform.localPosition = new Vector3(newRectTransform.localPosition.x + indicatorOffset.x,
			newRectTransform.localPosition.y + indicatorOffset.y,
			newRectTransform.localPosition.z);

		foreach (GameObject button in buttons)
		{
			EventTrigger trigger = button.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener((data) => { OnSelectionHover((PointerEventData)data); });
			trigger.triggers.Add(entry);
		}
	}

	// Update is called once per frame
	void Update()
	{
		int selectionChange = 0;
		if (Input.GetButtonUp("select"))
		{
			buttons[currentSelection].GetComponent<Button>().onClick.Invoke();
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
			while ( thisObject != null )
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

		rectTransform.localPosition = new Vector3(newRectTransform.localPosition.x + indicatorOffset.x,
			newRectTransform.localPosition.y + indicatorOffset.y,
			newRectTransform.localPosition.z);
	}

	private RectTransform rectTransform;
	private int currentSelection = 0;
}
