/*
@file PlayerModeEditor.cs
@author NDark
@date 20140104 file started.
@date 20140105 by NDark 
. add mode editors at DrawDisplayEditor()
. add slider for specified time at DrawDisplayEditor()
. move code of CLOSE and APPLY to buttom at DrawDisplayEditor()
@date 20140405 by NDark
. 調整Play Mode編輯視窗的大小
. 關閉狀態欄位
. 設定正確的指定車站與指定列車
. 將時間改為Label
. 加上 車站顯示模式的選擇 
*/
using UnityEngine;
using System.Collections;
// using UnityEditor ;

public class PlayerModeEditor : MonoBehaviour 
{
	public LevelGeneratorStatic pLevelGeneratorPtr = null ;
	public UpdateTrain pUpdateTrainPtr = null ;
	public UpdateStation pUpdateStaionPtr = null ;
	
	public bool m_DisplayEditor = false ;
	private int m_EditorID = 0;
	public Rect m_EditorRect = new Rect() ;
	private int m_DisplayEditorWindowWidth = 420 ;
	private int m_DisplayEditorWindowHeight = 200 ;

	public DisplayEditorMode m_DisplayEditorMode = DisplayEditorMode.NearStation ;

	private string m_SpecifiedStation = "" ;
	private string m_SpecifiedTrain = "" ;
	// public string m_StatusText = "XD" ;
	
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

		pUpdateStaionPtr = this.gameObject.GetComponent<UpdateStation>() ;
		if( null == pUpdateStaionPtr )
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
	

	public int m_StationDisplayMode = 0 ;
	public int m_SelectMode = 0 ;
	private void DrawDisplayEditor( int _ID )
	{
		string [] stationDisplayModeString = 
		{
			StationDisplayMode.AllStaion.ToString(),
			StationDisplayMode.BigStation.ToString(),
			StationDisplayMode.SmartDisplay.ToString(),
		} ;

		GUILayout.BeginHorizontal() ;
		m_StationDisplayMode = GUILayout.SelectionGrid( m_StationDisplayMode , 
		                                       stationDisplayModeString , stationDisplayModeString.Length ) ;
		pUpdateStaionPtr.m_StationDisplayMode = (StationDisplayMode) m_StationDisplayMode ;
		GUILayout.EndHorizontal() ;


		string [] selectionString = 
		{
			DisplayEditorMode.NearStation.ToString(),
			DisplayEditorMode.SpecifiedTrain.ToString(),
			DisplayEditorMode.AllTrains.ToString(),
			DisplayEditorMode.NoTrain.ToString(),
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

			if( 0 == m_SpecifiedStation.Length )
			{
				StationData[] stationData = new StationData[ pLevelGeneratorPtr.m_Stations.Count ] ;
				pLevelGeneratorPtr.m_Stations.Values.CopyTo( stationData , 0 ) ;
				if( stationData.Length > 0 )
				{
					m_SpecifiedStation = stationData[ 0 ].DisplayName;
				}
			}
			keyword = m_SpecifiedStation = GUILayout.TextField( m_SpecifiedStation ) ;
			break ;
		case DisplayEditorMode.SpecifiedTrain :

			if( 0 == m_SpecifiedTrain.Length )
			{
				int[] trainKeys = new int[ pLevelGeneratorPtr.m_TrainData.Count ] ;
				pLevelGeneratorPtr.m_TrainData.Keys.CopyTo( trainKeys , 0 ) ;
				if( trainKeys.Length > 0 )
				{
					m_SpecifiedTrain = trainKeys[ 0 ].ToString() ;
				}
			}
			GUILayout.Label( "Train" ) ;
			keyword = m_SpecifiedTrain = GUILayout.TextField( m_SpecifiedTrain ) ;			
			break ;			
		case DisplayEditorMode.AllTrains :
			GUILayout.Label( "Be careful!!!" ) ;
			break ;						
		case DisplayEditorMode.NoTrain :
			GUILayout.Label( "No Train." ) ;
			break ;	
		}
		GUILayout.EndHorizontal() ;
		
		// GUILayout.Label( "Description:" ) ;
		// m_StatusText = GUILayout.TextArea( m_StatusText ) ;
		

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


		GUILayout.Label( modeString[ updateTrainTimeMode] ) ;
		
		GUILayout.Label( pUpdateTrainPtr.m_SpecifiedHour.ToString() + ":" + pUpdateTrainPtr.m_SpecifiedMinute.ToString() ) ;
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
		
		GUI.DragWindow() ;
	}	
}
