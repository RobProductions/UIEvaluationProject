using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

namespace UIDemo.Core
{
	/// <summary>
	/// Core input API which wraps the Input System actions
	/// </summary>
	public class InputCore : MonoBehaviour
	{
		[System.Serializable]
		public class CInputActionWrapper
		{
			public InputAction action;
			public bool actionDown = false;
			public bool actionUp = false;
			public bool actionHeld = false;

			public bool hadUpdateForDown = false;
			public bool hadUpdateForUp = false;

			public bool GetActionDown()
			{
				return (actionDown && hadUpdateForDown);
			}

			public bool GetActionUp()
			{
				return (actionUp && hadUpdateForUp);
			}

			public bool GetAction()
			{
				return actionHeld;
			}
		}

		[System.Serializable]
		public class CInputActionMapWrapper
		{
			public InputActionMap actionMap;
			[HideInInspector]
			public Dictionary<string, CInputActionWrapper> actionDict;
		}


		[System.Serializable]
		public class MInputStats
		{
			public InputActionAsset inputMap;
		}

		public MInputStats stats;

		[System.Serializable]
		public class MInputReference
		{
			[HideInInspector]
			public Dictionary<string, CInputActionMapWrapper> actionMapDict;
		}

		public MInputReference reference;

		private readonly Dictionary<string, string> bindingNameRemap = new Dictionary<string, string> {
			{"Left Button", "Left Mouse"},
			{"Right Button", "Right Mouse"},
			{"Space", "Space"},
		};

		private Vector2 navigationDirectionVector = Vector2.zero;

		static private InputDevice latestInputDevice = null;

		// Awake is called before the first frame update
		void Awake()
		{
			EnableAllActions();
			CreateInputWrappers(stats.inputMap);

			var uiMoveAction = GetInputActionWrapper("UI", "Navigate");
			uiMoveAction.action.performed += NavigationStartedCallback;
			uiMoveAction.action.canceled += NavigationAdjustCallback;
		}

		void EnableAllActions()
		{
			if (stats.inputMap)
			{
				stats.inputMap.Enable();
			}
		}


		//DICTIONARY

		void CreateInputWrappers(InputActionAsset inMap)
		{
			if (reference.actionMapDict == null)
			{
				reference.actionMapDict = new Dictionary<string, CInputActionMapWrapper>();
			}
			reference.actionMapDict.Clear();

			if (inMap)
			{
				for (int i = 0; i < inMap.actionMaps.Count; i++)
				{
					InputActionMap thisActionMap = inMap.actionMaps[i];

					CInputActionMapWrapper newMapWrapper = new CInputActionMapWrapper();
					newMapWrapper.actionMap = thisActionMap;

					if (newMapWrapper.actionDict == null)
					{
						newMapWrapper.actionDict = new Dictionary<string, CInputActionWrapper>();
					}
					newMapWrapper.actionDict.Clear();

					for (int j = 0; j < thisActionMap.actions.Count; j++)
					{
						InputAction thisAction = thisActionMap.actions[j];

						thisAction.performed += ctx => InputActionPerformed(ctx, thisActionMap.name, thisAction.name);
						thisAction.canceled += ctx2 => InputActionCanceled(ctx2, thisActionMap.name, thisAction.name);

						CInputActionWrapper newWrapper = new CInputActionWrapper();
						newWrapper.action = thisAction;

						newMapWrapper.actionDict.Add(thisAction.name, newWrapper);
					}

					reference.actionMapDict.Add(thisActionMap.name, newMapWrapper);
				}
			}
		}

		public void InputActionPerformed(InputAction.CallbackContext ctx, string mapName, string actionName)
		{
			CInputActionWrapper performedWrapper = GetInputActionWrapper(mapName, actionName);
			if (performedWrapper != null)
			{
				performedWrapper.actionDown = true;
				performedWrapper.actionHeld = true;
			}
			if (ctx.action.type == InputActionType.Button || (ctx.action.type == InputActionType.PassThrough && ctx.valueType == typeof(bool)))
			{
				latestInputDevice = ctx.control.device;
			}
		}

		public void InputActionCanceled(InputAction.CallbackContext ctx, string mapName, string actionName)
		{
			CInputActionWrapper performedWrapper = GetInputActionWrapper(mapName, actionName);
			if (performedWrapper != null)
			{
				performedWrapper.actionUp = true;
				performedWrapper.actionHeld = false;
			}
		}

		//RESETS

		private void Update()
		{
			foreach (KeyValuePair<string, CInputActionMapWrapper> mapWrap in reference.actionMapDict)
			{
				foreach (KeyValuePair<string, CInputActionWrapper> actWrap in mapWrap.Value.actionDict)
				{
					if (actWrap.Value.actionDown)
					{
						actWrap.Value.hadUpdateForDown = true;
					}
					if (actWrap.Value.actionUp)
					{
						actWrap.Value.hadUpdateForUp = true;
					}
				}
			}
		}

		private void LateUpdate()
		{
			foreach (KeyValuePair<string, CInputActionMapWrapper> mapWrap in reference.actionMapDict)
			{
				foreach (KeyValuePair<string, CInputActionWrapper> actWrap in mapWrap.Value.actionDict)
				{
					if (actWrap.Value.hadUpdateForDown)
					{
						actWrap.Value.actionDown = false;
					}
					if (actWrap.Value.hadUpdateForUp)
					{
						actWrap.Value.actionUp = false;
					}
				}
			}
		}

		//INPUT WRAP

		public CInputActionWrapper GetInputActionWrapper(string mapName, string actionName)
		{
			CInputActionMapWrapper ret;
			if (reference.actionMapDict.TryGetValue(mapName, out ret))
			{
				CInputActionWrapper act;
				if (ret.actionDict.TryGetValue(actionName, out act))
				{
					return act;
				}
			}

			Debug.Log("InputActionWrapper " + mapName + " __ " + actionName + " was not found!!");

			return null;
		}

		public CInputActionMapWrapper GetInputActionMapWrapper(string mapName)
		{
			CInputActionMapWrapper ret;
			if (reference.actionMapDict.TryGetValue(mapName, out ret))
			{
				return ret;
			}

			Debug.Log("InputActionMapWrapper " + mapName + " was not found!!");

			return null;
		}

		public Dictionary<string, CInputActionMapWrapper> GetAllInputMaps()
		{
			return reference.actionMapDict;
		}

		//BINDING NAMES

		/// <summary>
		/// Acquire the binding display of an action based on index. 
		/// Accounts for composite bindings by accumulating all "part of composite" into a single string.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="bindingIndex"></param>
		/// <param name="useShortNames"></param>
		/// <returns></returns>
		public string GetBindingDisplayName(InputAction v, int bindingIndex, bool useShortNames = false)
		{
			if (v == null)
			{
				return "";
			}

			int finalIndex = bindingIndex;

			if (v.type == InputActionType.Value)
			{
				int macroIndex = -1;
				string buildComposite = "";
				for (int i = 0; i < v.bindings.Count; i++)
				{
					if (v.bindings[i].isPartOfComposite)
					{
						if (macroIndex == bindingIndex)
						{
							if (buildComposite != "")
							{
								buildComposite += ", ";
							}
							buildComposite += GetRawBindingDisplayName(v, i, useShortNames);
						}
					}
					else
					{
						macroIndex++;
					}
				}

				if (buildComposite != "")
				{
					return buildComposite;
				}

			}

			return GetRawBindingDisplayName(v, bindingIndex, useShortNames);
		}

		/// <summary>
		/// Gets the binding display string based on raw index, composites are not accounted for.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="bindingIndex"></param>
		/// <param name="useShortNames"></param>
		/// <returns></returns>
		public string GetRawBindingDisplayName(InputAction v, int bindingIndex, bool useShortNames = false)
		{
			if (v == null)
			{
				return "";
			}
			var stringOp = InputBinding.DisplayStringOptions.DontUseShortDisplayNames;
			if (useShortNames)
			{
				stringOp = (InputBinding.DisplayStringOptions)0;
			}

			//var ret = v.GetBindingDisplayString(v.bindings[bindingIndex], stringOp);
			var ret = v.bindings[bindingIndex].ToDisplayString(stringOp);
			//Debug.Log(v.name + " | " + ret);
			if (bindingNameRemap.ContainsKey(ret))
			{
				return bindingNameRemap[ret];
			}

			return ret;
		}

		/// <summary>
		/// Gets the first 2 bindings and places them together for a keyboard binding display string.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="useShortNames"></param>
		/// <returns></returns>
		public string GetKeyboardBindingsDisplayName(InputAction v, bool useShortNames = false)
		{
			if (v == null)
			{
				return "";
			}

			var ret = GetBindingDisplayName(v, 0, useShortNames);
			if (v.bindings.Count > 1)
			{
				ret += " | " + GetBindingDisplayName(v, 1, useShortNames);
			}

			return ret;
		}

		//EVENT SELECTION

		public void NavigationStartedCallback(InputAction.CallbackContext ctx)
		{
			navigationDirectionVector = ctx.ReadValue<Vector2>();
		}

		public void NavigationAdjustCallback(InputAction.CallbackContext ctx)
		{
			var system = EventSystem.current;

			if (system.currentSelectedGameObject == null || !system.currentSelectedGameObject.activeInHierarchy)
			{
				if (Selectable.allSelectableCount > 0)
				{
					var allSelected = Selectable.allSelectablesArray;

					/*
					for (int i = 0; i < allSelected.Length; i++)
					{
						if (allSelected[i].navigation.mode != Navigation.Mode.None)
						{
							Debug.Log("A" + allSelected[i].transform.position, allSelected[i]);
						}
					}
					*/

					//Sort by input direction to select top or bottom item
					if (navigationDirectionVector.y < 0f)
					{
						allSelected = allSelected.OrderByDescending(item1 => item1.transform.position.y).ToArray();
					}
					else
					{
						allSelected = allSelected.OrderBy(item1 => item1.transform.position.y).ToArray();
					}

					for (int i = 0; i < allSelected.Length; i++)
					{
						if (allSelected[i].navigation.mode != Navigation.Mode.None && allSelected[i].interactable)
						{
							allSelected[i].Select();
							break;
						}
					}

				}

			}
		}

		//INPUT UTILITIES

		public int GetConnectedControllerCount()
		{
			int ret = 0;
			foreach (var device in InputSystem.devices)
			{
				if (device is Gamepad)
				{
					ret++;
				}
			}

			return ret;
		}

		public InputDevice GetLatestInputDevice()
		{
			return latestInputDevice;
		}
	}

}