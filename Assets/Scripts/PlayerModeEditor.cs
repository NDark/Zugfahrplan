/*
@file PlayerModeEditor.cs
@author NDark
@date 20140104 file started.
*/
using UnityEngine;
using System.Collections;
using UnityEditor ;

public class PlayerModeEditor : MonoBehaviour 
{
	public bool m_DisplayEditor = false ;
	private int m_EditorID = 0;
	public Rect m_EditorRect = new Rect() ;
	public int m_DisplayEditorWindowWidth = 300 ;
	public int m_DisplayEditorWindowHeight = 300 ;
	
	public DisplayEditorMode m_DisplayEditorMode = DisplayEditorMode.NearStation ;
	public string m_SpecifiedStation = "Taipei" ;
	public string m_StatusText = "XD" ;
	
	// Use this for initialization
	void Start () 
	{
		m_EditorRect = new Rect( 0 , 0 , 
			m_DisplayEditorWindowWidth ,
			m_DisplayEditorWindowHeight ) ;

	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if( true == m_DisplayEditor )
		{
			m_EditorRect = GUI.Window( m_EditorID , m_EditorRect , DrawDisplayEditor , "DisplayEditor" ) ;
		}
		else
		{
			if( true == GUILayout.Button( "Open Menu" ) ) 
			{
				m_DisplayEditor = true ;
			}
		}
			

	}	
	
	public enum DisplayEditorMode 
	{
		NearStation = 0,
		SpecifiedTrain,
	}
	
	public int m_SelectMode = 0 ;
	private void DrawDisplayEditor( int _ID )
	{
		string [] selectionString = 
		{
			DisplayEditorMode.NearStation.ToString(),
			DisplayEditorMode.SpecifiedTrain.ToString(),
		} ;
		
		GUILayout.BeginHorizontal() ;
		m_SelectMode = GUILayout.SelectionGrid( m_SelectMode , 
			selectionString , selectionString.Length ) ;
		GUILayout.EndHorizontal() ;
		
		GUILayout.BeginHorizontal() ;
		switch( (DisplayEditorMode) m_SelectMode )
		{
		case DisplayEditorMode.NearStation :
			// specified station
			GUILayout.Label( "Station" ) ;
			m_SpecifiedStation = GUILayout.TextField( m_SpecifiedStation ) ;
			break ;
		case DisplayEditorMode.SpecifiedTrain :
			;
			break ;			
		}
		GUILayout.EndHorizontal() ;
		
		GUILayout.Label( "Description:" ) ;
		m_StatusText = GUILayout.TextArea( m_StatusText ) ;
		
		
		if( true == GUILayout.Button( "Close" ) ) 
		{
			m_DisplayEditor = false ;
		}
		GUI.DragWindow() ;
	}	
}
