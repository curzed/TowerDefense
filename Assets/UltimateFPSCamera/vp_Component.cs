/////////////////////////////////////////////////////////////////////////////////
//
//	vp_Component.cs
//	© 2012 VisionPunk, Minea Softworks. All Rights Reserved.
//
//	description:	base class for Ultimate FPS Camera MonoBehaviours
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class vp_Component : MonoBehaviour
{

	public bool Persist = false;
	protected float m_Delta = 1.0f;

	protected vp_StateManager m_StateManager = null;

	public List<vp_StateInfo> States = new List<vp_StateInfo>();	// list of state presets for this component
	protected vp_StateInfo m_DefaultState = null;
	
#if UNITY_EDITOR
	// initial state is needed because we refresh default state upon
	// inspector changes, and this will mess with our ability to save
	// difference presets (save tweaks) by breaking the compare values.
	// on the other hand we need to be able to refresh default state in
	// order not to loose inspector changes (i.e. if we accidentally
	// press zoom or crouch when tweaking components in the inspector)
	protected vp_StateInfo m_InitialState = null;		// used at editor runtime only
	public vp_StateInfo InitialState { get { return m_InitialState; } }
#endif


	//////////////////////////////////////////////////////////
	// properties
	//////////////////////////////////////////////////////////
	public vp_StateManager StateManager { get { return m_StateManager; } }
	public vp_StateInfo DefaultState { get { return m_DefaultState; } }


	//////////////////////////////////////////////////////////
	// 
	//////////////////////////////////////////////////////////
	public void Awake()
	{
		m_StateManager = new vp_StateManager(this, States);
		StateManager.SetState("Default", enabled);
	}


	//////////////////////////////////////////////////////////
	// 
	//////////////////////////////////////////////////////////
	public void Start()
	{
	}


	//////////////////////////////////////////////////////////
	// 
	//////////////////////////////////////////////////////////
	public void Update()
	{

		// treat delta as 1 at an application target framerate of 60
		m_Delta = (Time.deltaTime * 60.0f);

	}


	///////////////////////////////////////////////////////////
	// sets 'state' true / false on the component and refreshes it
	///////////////////////////////////////////////////////////
	public void SetState(string state, bool enabled)
	{

		m_StateManager.SetState(state, enabled);
		Refresh();

	}


	///////////////////////////////////////////////////////////
	// asks statemanager to disable all states except the default
	// state, and enables the default state. then refreshes.
	///////////////////////////////////////////////////////////
	public void ResetState()
	{

		m_StateManager.Reset();
		Refresh();

	}


	///////////////////////////////////////////////////////////
	// allows or disallows 'state' on this object
	///////////////////////////////////////////////////////////
	public virtual void AllowState(string state, bool isAllowed)
	{

		m_StateManager.AllowState(state, isAllowed);
		Refresh();

	}


	///////////////////////////////////////////////////////////
	// allows or disallows 'state' on this object, then scans
	// the underlying hierarchy for vp_Components and does the
	// same on every object found
	///////////////////////////////////////////////////////////
	public void AllowStateRecursively(string state, bool isAllowed)
	{

		AllowState(state, isAllowed);

		Component[] components;
		components = GetComponentsInChildren<vp_Component>();
		foreach (Component c in components)
		{
			vp_Component vc = (vp_Component)c;
			vc.AllowState(state, isAllowed);
		}

	}


	///////////////////////////////////////////////////////////
	// returns true if the state associated with the passed
	// string is on the list & enabled, otherwise returns null
	///////////////////////////////////////////////////////////
	public bool StateEnabled(string stateName)
	{

		return m_StateManager.IsEnabled(stateName);

	}


	///////////////////////////////////////////////////////////
	// sees if the component has a default state. if so, makes
	// sure it's in index zero of the list, if not, creates it.
	///////////////////////////////////////////////////////////
	public void RefreshDefaultState()
	{

		vp_StateInfo defaultState = null;

		if (States.Count == 0)
		{
			// there are no states, so create default state
			defaultState = new vp_StateInfo(GetType().Name, "Default", null);
			States.Add(defaultState);
		}
		else
		{
			for (int v = States.Count - 1; v > -1; v--)
			{
				if (States[v].Name == "Default")
				{
					// found default state, make sure it's in the back
					defaultState = States[v];
					States.Remove(defaultState);
					States.Add(defaultState);
				}
			}
			if (defaultState == null)
			{
				// there are states, but no default state so create it
				defaultState = new vp_StateInfo(GetType().Name, "Default", null);
				States.Add(defaultState);
			}
		}

		if (defaultState.Preset == null || defaultState.Preset.ComponentType == null)
			defaultState.Preset = new vp_ComponentPreset();

		if(defaultState.TextAsset == null)
			defaultState.Preset.InitFromComponent(this);

		defaultState.Enabled = true;	// default state is always enabled

		m_DefaultState = defaultState;

	}


	///////////////////////////////////////////////////////////
	// copies component values into the default state's preset.
	// if needed, creates & adds default state to the state list.
	// to be called on app startup and statemanager recombine
	///////////////////////////////////////////////////////////
#if UNITY_EDITOR
	public void RefreshInitialState()
	{

		m_InitialState = null;
		m_InitialState = new vp_StateInfo(GetType().Name, "Internal_Initial", null);
		m_InitialState.Preset = new vp_ComponentPreset();
		m_InitialState.Preset.InitFromComponent(this);

	}
#endif



	///////////////////////////////////////////////////////////
	// helper method to apply a preset from memory and refresh
	// settings. for cleaner syntax
	///////////////////////////////////////////////////////////
	public void ApplyPreset(vp_ComponentPreset preset)
	{
		vp_ComponentPreset.Apply(this, preset);
		RefreshDefaultState();
		Refresh();
	}


	///////////////////////////////////////////////////////////
	// helper method to load a preset from the resources folder,
	// and refresh settings. for cleaner syntax
	///////////////////////////////////////////////////////////
	public vp_ComponentPreset Load(string path)
	{
		vp_ComponentPreset preset = vp_ComponentPreset.LoadFromResources(this, path);
		RefreshDefaultState();
		Refresh();
		return preset;
	}


	///////////////////////////////////////////////////////////
	// helper method to load a preset from a text asset,
	// and refresh settings. for cleaner syntax
	///////////////////////////////////////////////////////////
	public vp_ComponentPreset Load(TextAsset asset)
	{
		vp_ComponentPreset preset = vp_ComponentPreset.LoadFromTextAsset(this, asset);
		RefreshDefaultState();
		Refresh();
		return preset;
	}


	///////////////////////////////////////////////////////////
	// to be overridden in inherited classes, for resetting
	// various important variables on the component
	///////////////////////////////////////////////////////////
	public virtual void Refresh()
	{
	}

}


