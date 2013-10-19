/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20131019 file started.

*/
using UnityEngine;
using System.Collections;

public class ZugfahrplanEditorStatic : MonoBehaviour 
{
	public LevelGeneratorStatic m_LevelGeneratorStaticPtr = null ;
	public int m_EditorID = 0 ;
	public Rect m_EditorRect = new Rect() ;
	public bool m_DisplayEditor = false ;
	public bool m_DisplayBackgroundImageInformation = false ;
	
	public bool m_DisplayStationData = false ;
	string m_InputStationLabel = "" ;
	string m_InputStationPosX = "0" ;
	string m_InputStationPosY = "0" ;
	public string m_SaveStationDataFilepath = "" ;
	
	// Use this for initialization
	void Start () 
	{
		
		m_LevelGeneratorStaticPtr = this.gameObject.GetComponent<LevelGeneratorStatic>() ;
		if( null == m_LevelGeneratorStaticPtr )
		{
			
		}
		else
		{
			m_SaveStationDataFilepath = m_LevelGeneratorStaticPtr.m_StationTableFilepath ;
		}
		
		m_EditorRect = new Rect( 0 , 0 , 
			Camera.mainCamera.GetScreenWidth() ,
			Camera.mainCamera.GetScreenHeight() ) ;
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if( true == m_DisplayEditor )
		{
			m_EditorRect = GUI.Window( m_EditorID , m_EditorRect , DrawEditor , "Editor" ) ;
		}
		else
		{
			if( true == GUILayout.Button( "Open" ) ) 
			{
				m_DisplayEditor = true ;
			}
		}
			

	}
	
	private void DrawEditor_BackgroundImage()
	{
		GUILayout.Label( "SceneBackgroundImage Path" ) ;
		m_LevelGeneratorStaticPtr.m_BackgroundSceneTexturePath = 
			GUILayout.TextField( m_LevelGeneratorStaticPtr.m_BackgroundSceneTexturePath ) ;
		
		GUILayout.Label( "SceneBackgroundImage Width" ) ;
		int tmpInt = 0 ;
		string intStr = 
			GUILayout.TextField( m_LevelGeneratorStaticPtr.m_BackgroundSceneImageWidth.ToString() ) ;
		if( true == int.TryParse( intStr , out tmpInt ) )
		{
			m_LevelGeneratorStaticPtr.m_BackgroundSceneImageWidth = tmpInt ;
		}
		
		GUILayout.Label( "SceneBackgroundImage Height" ) ;
		intStr = GUILayout.TextField( m_LevelGeneratorStaticPtr.m_BackgroundSceneImageHeight.ToString() ) ;
		if( true == int.TryParse( intStr , out tmpInt ) )
		{
			m_LevelGeneratorStaticPtr.m_BackgroundSceneImageHeight = tmpInt ;
		}
	}
	private void DrawEditor( int _ID )
	{
		float tmpFloatX = 0 ;
		float tmpFloatY = 0 ;

		m_DisplayBackgroundImageInformation = GUILayout.Toggle( m_DisplayBackgroundImageInformation , "SceneBackgroundImage Information" ) ;
		if( true == m_DisplayBackgroundImageInformation )
		{
			DrawEditor_BackgroundImage() ;
		}

		m_DisplayStationData = GUILayout.Toggle( m_DisplayStationData , "DisplayStationData" ) ;
		if( true == m_DisplayStationData )
		{
			GUILayout.Label( "Insert" ) ;
			GUILayout.BeginHorizontal() ;
			DrawStationData( ref m_InputStationLabel , 
							 ref m_InputStationPosX ,
							 ref m_InputStationPosY ) ;
			
			if( true == GUILayout.Button( "Insert" ) &&
				0 != m_InputStationLabel.Length ) 
			{
				StationData newSD = new StationData() ;
				newSD.ID = m_LevelGeneratorStaticPtr.GetANewStationID() ;
				newSD.DisplayName = m_InputStationLabel ;
				if( true == float.TryParse( m_InputStationPosX , out tmpFloatX ) &&
					true == float.TryParse( m_InputStationPosY , out tmpFloatY ) )
				{
					newSD.Position = new Vector3( tmpFloatX , 
												  tmpFloatY ,
												  m_LevelGeneratorStaticPtr.m_StationLayerZShift ) ;
				}
				Debug.Log( "newSD.ID" +  newSD.ID ) ;
				m_LevelGeneratorStaticPtr.m_Stations.Add( newSD.ID , newSD ) ;
				m_LevelGeneratorStaticPtr.ReCreateStationDisplayByData() ;
			}
			GUILayout.EndHorizontal() ;
			
			GUILayout.Label( "Station Table" ) ;
			
			foreach( StationData sd in m_LevelGeneratorStaticPtr.m_Stations.Values )
			{
				GUILayout.BeginHorizontal() ;
				string PreDisplayName = sd.DisplayName ;
				string PrePosX = string.Format( "{0:0.00}" , sd.Position.x ) ;
				string PrePosY = string.Format( "{0:0.00}" , sd.Position.y ) ;
				

				Vector3 tmpVec = Vector3.zero ;
				DrawStationData( ref PreDisplayName , 
								 ref PrePosX ,
								 ref PrePosY ) ;
				
				sd.DisplayName = PreDisplayName ;

				if( true == float.TryParse( PrePosX , out tmpFloatX ) &&
					true == float.TryParse( PrePosY , out tmpFloatY ) )
				{
					if( tmpFloatX >= 0 && tmpFloatX <= 1 &&
						tmpFloatY >= 0 && tmpFloatY <= 1 )
					{
						tmpVec.Set( tmpFloatX , tmpFloatY , 
							m_LevelGeneratorStaticPtr.m_StationLayerZShift ) ;
						if( sd.Position != tmpVec )
						{
							sd.Position = tmpVec ;
							// adjust station pos only
							m_LevelGeneratorStaticPtr.AdjustStationDisplayPos( sd.DisplayName , tmpVec ) ;
						}
					}
				}

				if( true == GUILayout.Button( "MoveUp" ) ) 
				{
					
				}			
				if( true == GUILayout.Button( "MoveDown" ) ) 
				{
					
				}						
				GUILayout.EndHorizontal() ;
			}
				
			if( true == GUILayout.Button( "Recreate All Station" ) ) 
			{
				m_LevelGeneratorStaticPtr.ReCreateStationDisplayByData() ;
			}
			GUILayout.BeginHorizontal() ;
			GUILayout.Label( "Station Data Filepath" ) ;
			m_SaveStationDataFilepath = GUILayout.TextField( m_SaveStationDataFilepath ) ;
			if( true == GUILayout.Button( "Save Station Data" ) ) 
			{
				m_LevelGeneratorStaticPtr.SaveStationTable() ;
			}
			GUILayout.EndHorizontal() ;
		}		
		

		
		if( true == GUILayout.Button( "ReLoad All Scene" ) ) 
		{
			m_LevelGeneratorStaticPtr.ReCreateScene() ;
		}
		
		if( true == GUILayout.Button( "Close" ) ) 
		{
			m_DisplayEditor = false ;
		}
		GUI.DragWindow() ;
	}
	
	private void DrawStationData( ref string _Label , ref string _PosX , ref string _PosY )
	{
		GUILayout.Label( "Label" ) ;
		_Label = GUILayout.TextField( _Label ) ;
		GUILayout.Label( "PositionXY" ) ;
		_PosX = GUILayout.TextField( _PosX ) ;
		_PosY = GUILayout.TextField( _PosY ) ;
	}
}
