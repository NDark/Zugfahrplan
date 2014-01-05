/*
@file PlayerModeEditor.cs
@author NDark
@date 20140104 file started.
@date 20140105 by NDark 
. add mode editors at DrawDisplayEditor()
. add slider for specified time at DrawDisplayEditor()

*/
using UnityEngine;
using System.Collections;
using UnityEditor ;

public class PlayerModeEditor : MonoBehaviour 
{
	public LevelGeneratorStatic pLevelGeneratorPtr = null ;
	public UpdateTrain pUpdateTrainPtr = null ;
	
	public bool m_DisplayEditor = false ;
	private int m_EditorID = 0;
	public Rect m_EditorRect = new Rect() ;
	public int m_DisplayEditorWindowWidth = 300 ;
	public int m_DisplayEditorWindowHeight = 300 ;
	
	public DisplayEditorMode m_DisplayEditorMode = DisplayEditorMode.NearStation ;
	public string m_SpecifiedStation = "Taipei" ;
	public string m_SpecifiedTrain = "S207" ;
	public string m_StatusText = "XD" ;
	
	// Use this for initialization
	void Start () 
	{
		pLevelGeneratorPtr = this.gameObject.GetComponent<LevelGeneratorStatic>() ;
		if( null == pLevelGeneratorPtr )
		{
			
		}
		
		pUpdateTrainPtr = this.gameObject.GetComponent<UpdateTrain>() ;
		if( null == pUpdateTrainPtr )
		{
			
		}		
		
		
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
	

	
	public int m_SelectMode = 0 ;
	private void DrawDisplayEditor( int _ID )
	{
		string [] selectionString = 
		{
			DisplayEditorMode.NearStation.ToString(),
			DisplayEditorMode.SpecifiedTrain.ToString(),
			DisplayEditorMode.AllTrains.ToString(),
			
		} ;
		string keyword = "" ;
		
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
			keyword = m_SpecifiedStation = GUILayout.TextField( m_SpecifiedStation ) ;
			break ;
		case DisplayEditorMode.SpecifiedTrain :
			GUILayout.Label( "Train" ) ;
			keyword = m_SpecifiedTrain = GUILayout.TextField( m_SpecifiedTrain ) ;			
			break ;			
		case DisplayEditorMode.AllTrains :
			GUILayout.Label( "Be careful!!!" ) ;
			break ;						
		}
		GUILayout.EndHorizontal() ;
		
		GUILayout.Label( "Description:" ) ;
		m_StatusText = GUILayout.TextArea( m_StatusText ) ;
		
		GUILayout.BeginHorizontal() ;
		if( true == GUILayout.Button( "Close" ) ) 
		{
			m_DisplayEditor = false ;
		}
		if( true == GUILayout.Button( "Apply" ) ) 
		{
			// RecreatScene
			pLevelGeneratorPtr.ReCreateTrainDisplayByData( 
				(DisplayEditorMode) m_SelectMode , 
				keyword ) ;
		}	
		GUILayout.EndHorizontal() ;
		string [] modeString = 
		{
			UpdateTrainTimeMode.SystemTime.ToString(),
			UpdateTrainTimeMode.SpecifiedTime.ToString(),
		} ;		
		int updateTrainTimeMode = (int) pUpdateTrainPtr.m_TimeMode ;
		
		GUILayout.BeginHorizontal() ;
		
		updateTrainTimeMode = GUILayout.SelectionGrid( updateTrainTimeMode , 
			modeString , modeString.Length ) ;
		
		pUpdateTrainPtr.SetUpdateTrainTimeMode( (UpdateTrainTimeMode) updateTrainTimeMode ) ;
		
		GUILayout.EndHorizontal() ;
		
		GUILayout.BeginHorizontal() ;
		
		GUILayout.Label( "Specified Time:" ) ;
		
		GUILayout.TextField( pUpdateTrainPtr.m_SpecifiedHour.ToString() ) ;
		GUILayout.TextField( pUpdateTrainPtr.m_SpecifiedMinute.ToString() ) ;
		GUILayout.EndHorizontal() ;
		
		switch( pUpdateTrainPtr.m_TimeMode )
		{
		case UpdateTrainTimeMode.SystemTime :
			break ;
		case UpdateTrainTimeMode.SpecifiedTime :
			int specifiedTimeValue = pUpdateTrainPtr.m_SpecifiedHour * 60 + pUpdateTrainPtr.m_SpecifiedMinute ;
			int maxTimeADay = 24 * 60 ;
			specifiedTimeValue = (int) GUILayout.HorizontalSlider( (float)specifiedTimeValue , 
				(float)0 , (float)maxTimeADay ) ;
			pUpdateTrainPtr.SetSpecifiedTime( specifiedTimeValue ) ;
			
			break ;			
		}
		
		GUI.DragWindow() ;
	}	
}
