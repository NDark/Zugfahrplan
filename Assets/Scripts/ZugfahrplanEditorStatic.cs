/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20131019 file started.
@date 20131208 by NDark . add class method SwapStationData()
@date 2013
. add class member m_TrainStartIndex
. add class member m_TrainNumInPage
. add Previous page of train data
. add Next Page of train data
. add MoveUp of train data
. add MoveDown of train data
. add Remove of train data
. add class method SwapTrainData()

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
	public Vector2 m_StationScrollViewPos = Vector2.zero ;
	public int m_StationStartIndex = 0 ;
	public int m_StationNumInPage = 5 ;	

	// traini data
	public bool m_DisplayTrainData = false ;
	public bool m_DisplayRouteOfTrain = false ;
	public int m_TrainStartIndex = 0 ;
	public int m_TrainNumInPage = 5 ;	

	string m_InputTrainLabel = "" ;

	
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
			if( true == GUILayout.Button( "Open Menu" ) ) 
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
	private void DrawEditor_StationsData()
	{
		float tmpFloatX = 0 ;
		float tmpFloatY = 0 ;
		
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
		
		GUILayout.BeginHorizontal() ;
		GUILayout.Label( "Station Table" ) ;
		GUILayout.Label( "Page Start Index:" + m_StationStartIndex ) ;
		
		
		if( true == GUILayout.Button( "Previous Page" ) )
		{
			m_StationStartIndex -= m_StationNumInPage ;
			if( m_StationStartIndex < 0 )
				m_StationStartIndex = 0 ;
		}
		if( true == GUILayout.Button( "Next Page" ) )
		{
			m_StationStartIndex += m_StationNumInPage ;
			if( m_StationStartIndex >= m_LevelGeneratorStaticPtr.m_Stations.Count )
				m_StationStartIndex -= m_StationNumInPage ;
		}
		GUILayout.EndHorizontal() ;
		
		int i = 0 ;
		foreach( StationData sd in m_LevelGeneratorStaticPtr.m_Stations.Values )
		{
			if( i < m_StationStartIndex || i >= m_StationStartIndex + m_StationNumInPage )
			{
				++i ;
				continue ;
			}

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
				SwapStationData( i , i-1 ) ;
				return ;// end this round
			}			
			if( true == GUILayout.Button( "MoveDown" ) ) 
			{
				SwapStationData( i , i+1 ) ;
				return ;// end this round
			}
			if( true == GUILayout.Button( "Remove" ) ) 
			{
				m_LevelGeneratorStaticPtr.RemoveStation( sd.ID ) ;
				return ;// end this round
			}			
			
			GUILayout.EndHorizontal() ;
			++i ;
		}
		
		if( true == GUILayout.Button( "Recreate All visible stations." ) ) 
		{
			m_LevelGeneratorStaticPtr.ReCreateStationDisplayByData() ;
		}
		
		// Save
		GUILayout.BeginHorizontal() ;
		GUILayout.Label( "Station Data Filepath" ) ;
		m_SaveStationDataFilepath = GUILayout.TextField( m_SaveStationDataFilepath ) ;
		if( true == GUILayout.Button( "Save Station Data" ) ) 
		{
			m_LevelGeneratorStaticPtr.SaveStationTable() ;
		}
		GUILayout.EndHorizontal() ;

	}
	private void DrawEditor_TrainsData()
	{
		float tmpFloatX = 0 ;
		float tmpFloatY = 0 ;
		
		GUILayout.Label( "Insert" ) ;
		GUILayout.BeginHorizontal() ;
		DrawTrainData( ref m_InputTrainLabel ) ;
		if( true == GUILayout.Button( "Insert" ) &&
			0 != m_InputTrainLabel.Length ) 
		{
			TrainData newTD = new TrainData() ;
			newTD.ID = m_LevelGeneratorStaticPtr.GetANewTrainID() ;
			newTD.DisplayName = m_InputTrainLabel ;
			
			Debug.Log( "newSD.ID" +  newTD.ID ) ;
			m_LevelGeneratorStaticPtr.m_TrainData.Add( newTD.ID , newTD ) ;
			m_LevelGeneratorStaticPtr.ReCreateTrainDisplayByData() ;
		}
		GUILayout.EndHorizontal() ;
		
		GUILayout.BeginHorizontal() ;
		GUILayout.Label( "Train Table" ) ;
		GUILayout.Label( "Page Start Index:" + m_StationStartIndex ) ;
		if( true == GUILayout.Button( "Previous Page" ) )
		{
			m_TrainStartIndex -= m_TrainNumInPage ;
			if( m_TrainStartIndex < 0 )
				m_TrainStartIndex = 0 ;			
		}
		if( true == GUILayout.Button( "Next Page" ) )
		{
			m_TrainStartIndex += m_TrainNumInPage ;
			if( m_TrainStartIndex >= m_LevelGeneratorStaticPtr.m_TrainData.Count )
				m_TrainStartIndex -= m_TrainNumInPage ;
		}		
		GUILayout.EndHorizontal() ;
		
		int i = 0 ;
		foreach( TrainData td in m_LevelGeneratorStaticPtr.m_TrainData.Values ) 
		{
			if( i < m_TrainStartIndex || i >= m_TrainStartIndex + m_TrainNumInPage )
			{
				++i ;
				continue ;
			}
			
			GUILayout.BeginHorizontal() ;
			
			string PreDisplayName = td.DisplayName ;
						
			DrawTrainData( ref PreDisplayName ) ;
			
			td.DisplayName = PreDisplayName ;
				
			if( true == GUILayout.Button( "MoveUp" ) ) 
			{
				SwapTrainData( i , i-1 ) ;
				return ;// end this round
			}
			if( true == GUILayout.Button( "MoveDown" ) ) 
			{
				SwapTrainData( i , i+1 ) ;
				return ;// end this round
			}
			if( true == GUILayout.Button( "Remove" ) ) 
			{
				m_LevelGeneratorStaticPtr.RemoveTrain( td.ID ) ;
				return ;// end this round
			}					
			if( true == GUILayout.Button( "SetRoutes" ) ) 
			{
				m_DisplayRouteOfTrain = true ;
				m_DisplayTrainData = false ;
				return ;// end this round
			}			
			GUILayout.EndHorizontal() ;
			++i ;
		}
		
	}
	private void DrawEditor( int _ID )
	{
		m_DisplayBackgroundImageInformation = GUILayout.Toggle( m_DisplayBackgroundImageInformation , "SceneBackgroundImage Information" ) ;
		if( true == m_DisplayBackgroundImageInformation )
		{
			DrawEditor_BackgroundImage() ;
		}

		m_DisplayStationData = GUILayout.Toggle( m_DisplayStationData , "DisplayStationData" ) ;
		if( true == m_DisplayStationData )
		{
			DrawEditor_StationsData() ;
		}		
		
		m_DisplayTrainData = GUILayout.Toggle( m_DisplayTrainData , "DisplayTrainData" ) ;
		if( true == m_DisplayTrainData )
		{
			m_DisplayRouteOfTrain = false ;
			DrawEditor_TrainsData() ;
		}
		if( true == m_DisplayRouteOfTrain )
		{
			
		}
		
		
		if( true == GUILayout.Button( "ReLoad All Scene" ) ) 
		{
			m_LevelGeneratorStaticPtr.ReCreateScene() ;
			m_LevelGeneratorStaticPtr.ReCreateTrainDisplayByData() ;
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
	
	private void SwapStationData( int _i , int _j )
	{
		int iter = 0 ;
		StationData sd_i = null ;
		StationData sd_j = null ;
		StationData sd_tmp = new StationData() ;
		foreach( StationData sd in m_LevelGeneratorStaticPtr.m_Stations.Values )
		{
			if( iter == _i )
			{
				sd_i = sd ;
			}
			else if( iter == _j )
			{
				sd_j = sd ;
			}
			++iter ;
		}
		
		if( null != sd_i && 
			null != sd_j )
		{
			sd_tmp.CopyFrom( sd_j ) ;
			sd_j.CopyFrom( sd_i );
			sd_i.CopyFrom( sd_tmp ) ;
		}
	}
	
	private void DrawTrainData( ref string _Label )
	{
		GUILayout.Label( "Label" ) ;
		_Label = GUILayout.TextField( _Label ) ;
	}	
	
	
	private void SwapTrainData( int _i , int _j )
	{
		int iter = 0 ;
		TrainData td_i = null ;
		TrainData td_j = null ;
		TrainData td_tmp = new TrainData() ;
		foreach( TrainData td in m_LevelGeneratorStaticPtr.m_TrainData.Values )
		{
			if( iter == _i )
			{
				td_i = td ;
			}
			else if( iter == _j )
			{
				td_j = td ;
			}
			++iter ;
		}
		
		if( null != td_i && 
			null != td_j )
		{
			td_tmp.CopyFrom( td_j ) ;
			td_j.CopyFrom( td_i );
			td_i.CopyFrom( td_tmp ) ;
		}
	}
		
}
