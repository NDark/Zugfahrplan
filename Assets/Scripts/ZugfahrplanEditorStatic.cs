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
@date 20140405 by NDark
. 將 m_ParseFilepath 預設改為 import
@date 20140503 by NDark . 移除 :00 這樣的格式 at ParseExcelTrainTable()

*/
// #define DEBUG

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


	// route table
	public int m_RouteStartIndex = 0 ;
	public int m_RouteNumInPage = 5 ;	
	public int m_SetRouteTrainID = 0 ;


	private string m_ParseFilepath = "import" ;
	
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
			Screen.width ,
		                        Screen.height ) ;
		
	
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
		
		GUILayout.Label( "Insert" ) ;
		GUILayout.BeginHorizontal() ;
		
		string inputTrainLabel = "" ;
		DrawTrainData( ref inputTrainLabel ) ;
		if( true == GUILayout.Button( "Insert" ) &&
			0 != inputTrainLabel.Length ) 
		{
			TrainData newTD = new TrainData() ;
			newTD.ID = m_LevelGeneratorStaticPtr.GetANewTrainID() ;
			newTD.DisplayName = inputTrainLabel ;
			
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
				m_SetRouteTrainID = td.ID ;
				m_DisplayRouteOfTrain = true ;
				m_DisplayTrainData = false ;
				return ;// end this round
			}			
			GUILayout.EndHorizontal() ;
			++i ;
		}
	}
	
	private void DrawEditor_TrainRoute()
	{
		bool IsReturn = false ;
		string trainName = "(invalid)" ;
		TrainData setRouteTrain = null ;
		if( false == m_LevelGeneratorStaticPtr.m_TrainData.ContainsKey( m_SetRouteTrainID ) )
		{
			// no such train.
			IsReturn = true ;
		}
		else
		{
			setRouteTrain = m_LevelGeneratorStaticPtr.m_TrainData[ m_SetRouteTrainID ] ;
			trainName = setRouteTrain.DisplayName ;
		}
		
		
		// trainName
		GUILayout.BeginHorizontal() ;
		GUILayout.Label( "The Routes of Train" ) ;
		GUILayout.Label( trainName ) ;
		GUILayout.EndHorizontal() ;		
		
		if( true == IsReturn )
			return ;
		
		// insert
		GUILayout.BeginHorizontal() ;
		GUILayout.Label( "Insert" ) ;
		string inputRouteStation = "" ;
		int inputRouteHour = 0 ;
		int inputRouteMinute = 0 ;
		DrawRouteData( ref inputRouteHour ,
					   ref inputRouteMinute ,
					   ref inputRouteStation ) ;
		if( true == GUILayout.Button( "Insert" ) &&
			0 != inputRouteHour &&
			0 != inputRouteMinute &&
			0 != inputRouteStation.Length ) 
		{
			// insert a route
		}
		
		GUILayout.EndHorizontal() ;		
		
		GUILayout.BeginHorizontal() ;
		GUILayout.Label( "Route Table" ) ;
		GUILayout.Label( "Page Start Index:" + m_RouteStartIndex ) ;
		if( true == GUILayout.Button( "Previous Page" ) )
		{
			m_RouteStartIndex -= m_RouteNumInPage ;
			if( m_RouteStartIndex < 0 )
				m_RouteStartIndex = 0 ;			
		}
		if( true == GUILayout.Button( "Next Page" ) )
		{
			m_RouteStartIndex += m_RouteNumInPage ;
			if( m_RouteStartIndex >= setRouteTrain.m_TimeTable.Count )
				m_RouteStartIndex -= m_RouteNumInPage ;
		}		
		GUILayout.EndHorizontal() ;
		
		int i = 0 ;
		foreach( TimeTableStruct timeStation in setRouteTrain.m_TimeTable ) 
		{
			if( i < m_RouteStartIndex || i >= m_RouteStartIndex + m_RouteNumInPage )
			{
				++i ;
				continue ;
			}
			
			GUILayout.BeginHorizontal() ;
			
			int hour = timeStation.Hour ;
			int minute = timeStation.Minite ;
			string station = timeStation.Station ;
			DrawRouteData( ref hour , 
						   ref minute ,
						   ref station ) ;
			timeStation.Hour = hour ;
			timeStation.Minite = minute ;
			timeStation.Station = station ;
			
			
			if( true == GUILayout.Button( "MoveUp" ) ) 
			{
				SwapRouteData( i , i-1 ) ;
				return ;// end this round
			}
			if( true == GUILayout.Button( "MoveDown" ) ) 
			{
				SwapRouteData( i , i+1 ) ;
				return ;// end this round
			}
			if( true == GUILayout.Button( "Remove" ) ) 
			{
				setRouteTrain.RemoveTimeRoute( timeStation ) ;
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
			DrawEditor_TrainRoute() ;
		}
		
		
		if( true == GUILayout.Button( "ReLoad All Scene" ) ) 
		{
			m_LevelGeneratorStaticPtr.ReCreateScene() ;
			m_LevelGeneratorStaticPtr.ReCreateTrainDisplayByData() ;
		}

		GUILayout.BeginHorizontal() ;
		m_ParseFilepath = GUILayout.TextField( m_ParseFilepath ) ;
		if( true == GUILayout.Button( "Parse Excel Train Table" ) ) 
		{
			ParseExcelTrainTable( m_ParseFilepath ) ;
		}
		GUILayout.EndHorizontal() ;

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
		
	
	private void DrawRouteInput( ref string _TimeStr , ref string _Station )
	{
		GUILayout.Label( "<Hour>:<Minute>" ) ;
		_TimeStr = GUILayout.TextField( _TimeStr ) ;
		GUILayout.Label( "Station" ) ;
		_Station = GUILayout.TextField( _Station ) ;
	}
			
	private void DrawRouteData( ref int _Hour , ref int _Minute , ref string _Station )
	{
		GUILayout.Label( "Time" ) ;
		string hourStr = GUILayout.TextField( _Hour.ToString() ) ;
		GUILayout.Label( ":" ) ;
		string minuteStr = GUILayout.TextField( _Minute.ToString() ) ;
		int.TryParse( hourStr , out _Hour ) ;
		int.TryParse( minuteStr , out _Minute ) ;		
		
		GUILayout.Label( "Station" ) ;
		_Station = GUILayout.TextField( _Station ) ;
	}	
		
	private void SwapRouteData( int _i , int _j )
	{
		TrainData setRouteTrain = null ;
		if( false == m_LevelGeneratorStaticPtr.m_TrainData.ContainsKey( m_SetRouteTrainID ) )
		{
			return ;
		}
		setRouteTrain = m_LevelGeneratorStaticPtr.m_TrainData[ m_SetRouteTrainID ] ;
		
		TimeTableStruct timeStation_tmp = new TimeTableStruct() ;
		TimeTableStruct[] timeTable = setRouteTrain.m_TimeTable.ToArray() ;
		if( _i < timeTable.Length &&
			_j < timeTable.Length )
		{
			timeStation_tmp.CopyFrom( timeTable[ _j ] ) ;
			timeTable[ _j ].CopyFrom( timeTable[ _i ] );
			timeTable[ _i ].CopyFrom( timeStation_tmp ) ;
		}
	}

	private void ParseExcelTrainTable( string _ParseFilepath )
	{
		TextAsset ta = (TextAsset) Resources.Load( _ParseFilepath , typeof(TextAsset) ) ;
		if( null == ta )
		{
			Debug.LogError( "ParseExcelTrainTable() _ParseFilepath load failed=" + _ParseFilepath ) ;
			return ;
		}
		
		// m_Stations
		string finalString = "" ;
		string content = ta.text ;
		#if DEBUG
		Debug.Log( "content=" + content ) ;
		#endif
		string[] splitor1 = { "\r\n" , "\n" , "\r" }  ;
		string []lineVec = content.Split( splitor1 , System.StringSplitOptions.None ) ;
		#if DEBUG
		Debug.Log( "lineVec.Length=" + lineVec.Length ) ;
		#endif
		for( int i = 0 ; i < lineVec.Length ; ++i )
		{
			if( 0 < lineVec[ i ].Length )
			{
				#if DEBUG
				Debug.Log( lineVec[ i ] ) ;
				#endif

				string[] splitor2 = { "," }  ;
				string []segmentOfLine = lineVec[ i ].Split( splitor2 , System.StringSplitOptions.None ) ;

				// 1,七堵,11:28:00,11:28:00
				// 11:28@七堵;11:38@汐止
				string arriveTime = "" ;
				if( segmentOfLine.Length >= 4 &&
				    0 != segmentOfLine[3].Length )
				{
					arriveTime = segmentOfLine[3] ;
				}
				else
				{
					arriveTime = segmentOfLine[2] ;
				}

				if( 0 == i )
				{
					finalString = arriveTime + "@" + segmentOfLine[1] ;
				}
				else
				{
					finalString += ";" + arriveTime + "@" + segmentOfLine[1] ;
				}
				#if DEBUG
				Debug.Log( finalString ) ;
				#endif
			}

			System.IO.StreamWriter SW = new System.IO.StreamWriter( _ParseFilepath + "string.txt" ) ;
			SW.Write( finalString ) ;
			SW.Close() ;
		}
		#if DEBUG
		Debug.Log( "LoadStationTable() end." ) ;
		#endif
	}
	
}
