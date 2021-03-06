/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20130922 by NDark
@date 20131019 by NDark 
. rename class member m_BackgroundScebeImageWidth by m_BackgroundSceneImageWidth
. rename class member m_BackgroundScebeImageHeight by m_BackgroundSceneImageHeight
. 增加各種公開介面
. add class method AdjustStationDisplayPos()
@date 20131220 by NDark 
. add class method GetANewTrainID()
. add class method RemoveStation()
. add class method RemoveTrain()
. add display text object at CreateStationsByData()
. add display text object at CreateTrainsByData()
@date 20140329 
. implement SysInit.txt
@date 20140405 by NDark
. 縮小車站圖片及字體大小
. 新增 No Train 的顯示方式
@dae 20140501 by NDark
. remove class member m_TrainIconTexturePath

*/
// #define DEBUG

using UnityEngine;
using System.Collections.Generic;
using System.IO ;
using System.Xml ;

// define the mode how to display trains.
public enum DisplayEditorMode 
{
	NearStation = 0,
	SpecifiedTrain,
	AllTrains ,
	NoTrain ,
}

public class LevelGeneratorStatic : MonoBehaviour 
{
	private string m_SysInitFilepath = "SysInit" ;
	public string m_StationTableFilepath = "Stations" ;
	public string m_TrainTableFilepath = "Trains" ;

	// SysInit data
	// train type to texture name
	public Dictionary<string , string > m_TrainTypeTextureMap = new Dictionary<string, string>() ;

	// station data
	public float m_StationLayerZShift = 1 ;
	public string m_StationIconTexturePath = "stationIcon6464" ;
	public Dictionary<int , StationData> m_Stations = new Dictionary<int , StationData>() ;
	public Dictionary<int , StationDisplay > m_StationDisplay = new Dictionary<int , StationDisplay>() ;

	// train data
	public float m_TrainLayerZShift = 2 ;
	public Dictionary<int , TrainData> m_TrainData = new Dictionary<int , TrainData>() ;
	public Dictionary<int , TrainDisplay > m_TrainDisplay = new Dictionary<int , TrainDisplay>() ;
	
	// background image parameter
	public float m_BackgroundSceneImageWidth = 1200 ;
	public float m_BackgroundSceneImageHeight = 1661 ;
	public string m_BackgroundSceneTexturePath = "map8_1" ;
	public string m_BackgroundSceneGUIObjName = "GUI_BackgroundTextureObj" ;
	public GameObject m_BackgroundSceneGUIObjPtr = null ;
	
	private DisplayEditorMode m_DisplayEditorMode = DisplayEditorMode.AllTrains ;
	private string m_RecreateTrainKeyword = "" ;
	
	private int m_ShareHour = 0 ;
	private int m_ShareMinute = 0 ;
	
	public void SetShareTime( int _Hour , int _Minute )
	{
		m_ShareHour = _Hour ;
		m_ShareMinute = _Minute ;
	}
	
	public int GetStationIDByStationName( string _StationName )
	{
		foreach( StationData sd in m_Stations.Values )
		{
			if( sd.DisplayName == _StationName )
				return sd.ID ;
		}
		return 0 ;
	}
	public int GetANewStationID()
	{
		int iter = m_Stations.Count ;
		bool collide = true ;
		
		while( true == collide )
		{
			collide = false ;
			foreach( StationData sd in m_Stations.Values )
			{
				if( iter == sd.ID )
				{
					++iter ;
					collide = true ;
					break ;
				}
			}
		}
		return iter ;
	}	
	
	public int GetANewTrainID()
	{
		int iter = m_TrainData.Count ;
		bool collide = true ;
		
		while( true == collide )
		{
			collide = false ;
			foreach( TrainData td in m_TrainData.Values )
			{
				if( iter == td.ID )
				{
					++iter ;
					collide = true ;
					break ;
				}
			}
		}
		return iter ;
	}		
	
	// 立即重新創造所有關卡
	public void ReCreateScene()
	{
		LoadSysInit( m_SysInitFilepath ) ;
		LoadStationTable(m_StationTableFilepath ) ;
		LoadTrainTable(m_TrainTableFilepath ) ;
		CreateBackgroundScene() ;
	}
	
	public void ReloadSysInit( string _SysInitFilepath )
	{
		m_SysInitFilepath = _SysInitFilepath ;
		LoadSysInit( m_SysInitFilepath ) ;
	}
	
	public void ReloadStationTableData( string _StationTableFilepath )
	{
		m_StationTableFilepath = _StationTableFilepath ;
		LoadStationTable(m_StationTableFilepath ) ;
	}
	
	public void SaveStationDataByFilepath( string _StationTableFilepath )
	{
		m_StationTableFilepath = _StationTableFilepath ;
		SaveStationTable() ;
	}
	
	public void SaveStationTable()
	{
		Debug.Log( "SaveStationTable" + m_StationTableFilepath ) ;
		string savePath = "Assets/Resources/" + m_StationTableFilepath + ".txt" ;
		System.IO.StreamWriter SW = new System.IO.StreamWriter( savePath ) ;
		foreach( StationData sd in m_Stations.Values )
		{
			string strWrite = sd.CreateString() ;
			SW.WriteLine( strWrite ) ;
		}
		SW.Close() ;
	}
	
	public void ReloadTrainTableData( string _TrainTableFilepath )
	{
		m_TrainTableFilepath = _TrainTableFilepath ;
		LoadTrainTable(m_TrainTableFilepath ) ;
	}	
	
	// 重新創造背景貼圖
	public void ReCreateBackgroundScene( string _BackgroundSceneTexturePath , 
										 float _BackgroundSceneImageWidth ,
										 float _BackgroundSceneImageHeight )
	{
		m_BackgroundSceneTexturePath = _BackgroundSceneTexturePath ;
		m_BackgroundSceneImageWidth = _BackgroundSceneImageWidth ;
		m_BackgroundSceneImageHeight = _BackgroundSceneImageHeight ;
		
		CreateBackgroundScene() ;
	}
	
	public void ReCreateStationDisplayByData()
	{
		CreateStationsByData() ;
	}

	public void ReCreateTrainDisplayByData()
	{
		ReCreateTrainDisplayByData( m_DisplayEditorMode , 
							m_RecreateTrainKeyword ) ;
	}
	
	public void ReCreateTrainDisplayByData( DisplayEditorMode _Mode , string _Keyword )
	{
		m_DisplayEditorMode = _Mode ;
		m_RecreateTrainKeyword = _Keyword ;
		
		CreateTrainsByData() ;
	}
	
	public void AdjustStationDisplayPos( string _StationLabel , Vector3 _NewPos )
	{
		int stationID = GetStationIDByStationName( _StationLabel ) ;
		if( 0 != stationID )
		{
			m_StationDisplay[ stationID ].Position = _NewPos ;
			m_StationDisplay[ stationID ].Obj.transform.position = _NewPos ;
		}
	}
	
	public void RemoveStation( int _ID )
	{
		if( true == m_Stations.ContainsKey( _ID ) )
		{
			m_Stations.Remove( _ID ) ;
		}
		
		if( true == m_StationDisplay.ContainsKey( _ID ) )
		{
			GameObject.Destroy( m_StationDisplay[ _ID ].Obj ) ;
			m_StationDisplay.Remove( _ID ) ;
		}
	}
	
	public void RemoveTrain( int _ID )
	{
		if( true == m_TrainData.ContainsKey( _ID ) )
		{
			m_TrainData.Remove( _ID ) ;
		}
		
		if( true == m_TrainDisplay.ContainsKey( _ID ) )
		{
			GameObject.Destroy( m_TrainDisplay[ _ID ].Obj ) ;
			m_TrainDisplay.Remove( _ID ) ;
		}
	}
	
	
	
	
	// Use this for initialization
	void Start () 
	{
		LoadSysInit( m_SysInitFilepath ) ;
		LoadStationTable(m_StationTableFilepath ) ;
		LoadTrainTable(m_TrainTableFilepath ) ;
		CreateScene() ;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void LoadSysInit( string _Filepath )
	{
		XmlDocument doc = new XmlDocument() ;
		Debug.Log( "LoadSysInit()" + _Filepath ) ;
		TextAsset tx = Resources.Load<TextAsset>( _Filepath ) ;

		doc.LoadXml( tx.text ) ;
		XmlNode root = doc.FirstChild;
		if( root.HasChildNodes )
		{
			Debug.Log( root.ChildNodes.Count ) ;
			foreach( XmlNode node in root.ChildNodes )
			{
				if( "Traintype" == node.Name )
				{
					if( null != node.Attributes[ "label" ] &&
					   null != node.Attributes[ "textureName" ] )
					{
						string label = node.Attributes[ "label" ].Value ;
						string textureName = node.Attributes[ "label" ].Value ;
						Debug.Log( label +" " + textureName ) ;
						m_TrainTypeTextureMap.Add( label, textureName ) ;
					}
				}
			}
		}
	}	
	
	private void LoadStationTable( string _Filepath )
	{
		DebugLog( "LoadStationTable() start." ) ;
		
		DestroyStationsData() ;
		
		TextAsset ta = (TextAsset) Resources.Load( _Filepath , typeof(TextAsset) ) ;
		if( null == ta )
		{
			Debug.LogError( "LoadStationTable() m_StationTableFilepath load failed=" + _Filepath ) ;
			return ;
		}
		
		// m_Stations
		string content = ta.text ;
		DebugLog( "content=" + content ) ;
		string[] splitor1 = { "\r\n" , "\n" , "\r" }  ;
		// Debug.Log( "splitor1=" + splitor1[0] ) ;
		StationData stationData = null ;
		string []lineVec = content.Split( splitor1 , System.StringSplitOptions.None ) ;
		DebugLog( "lineVec.Length=" + lineVec.Length ) ;
		for( int i = 0 ; i < lineVec.Length ; ++i )
		{
			if( 0 < lineVec[ i ].Length )
			{
#if DEBUG
				Debug.Log( lineVec[ i ] ) ;
#endif				
				stationData = new StationData() ;
				stationData.ParseFromString( lineVec[ i ] ) ;
				if( false == m_Stations.ContainsKey( stationData.ID ) )
				{
					m_Stations.Add( stationData.ID , stationData ) ;
					DebugLog( "m_Stations.Add=" + stationData.DisplayName ) ;
				}
			}
		}
		DebugLog( "LoadStationTable() end." ) ;
	}
	
	private void DestroyStationsData()
	{
		m_Stations.Clear() ;
	}
	

	
	private void LoadTrainTable( string _Filepath )
	{
		DebugLog( "LoadTrainTable() start." ) ;
		
		DestroyTrainData() ;
		
		TextAsset ta = (TextAsset) Resources.Load( _Filepath , typeof(TextAsset) ) ;
		if( null == ta )
		{
			Debug.LogError( "LoadTrainTable() _Filepath load failed=" + _Filepath ) ;
			return ;
		}
		
		// m_TrainData
		string content = ta.text ;
		DebugLog( "content=" + content ) ;
		string[] splitor1 = { "\r\n" , "\n" , "\r" }  ;
		// Debug.Log( "splitor1=" + splitor1[0] ) ;
		TrainData trainData = null ;
		string []lineVec = content.Split( splitor1 , System.StringSplitOptions.None ) ;
		DebugLog( "lineVec.Length=" + lineVec.Length ) ;
		for( int i = 0 ; i < lineVec.Length ; ++i )
		{
			if( 0 < lineVec[ i ].Length )
			{
#if DEBUG				
				Debug.Log( lineVec[ i ] ) ;
#endif				
				trainData = new TrainData() ;
				
				trainData.ParseFromString( lineVec[ i ] ) ;
				m_TrainData.Add( trainData.ID , trainData ) ;
				DebugLog( "m_TrainData.Add=" + trainData.DisplayName ) ;
			}
		}
		DebugLog( "LoadTrainTable() end." ) ;
	}
	
	private void DestroyTrainData()
	{
		m_TrainData.Clear() ;
	}

	private void CreateScene()
	{
		CreateBackgroundScene() ;
		CreateStationsByData() ;
		CreateTrainsByData() ;
	}
	
	private void CreateBackgroundScene()
	{
		DebugLog( "CreateBackgroundScene() start." ) ;
		
		DestroyBackgroundSceneObject() ;
		
		Texture2D bgTexture = (Texture2D)Resources.Load( m_BackgroundSceneTexturePath , 
													   typeof(Texture2D) ) ;
		if( null == bgTexture )
		{
			Debug.LogError( "CreateBackgroundScene() bgTexture load failed=" + m_BackgroundSceneTexturePath ) ;
			return ;
		}


		// Debug.Log( "Screen.width=" + Screen.width ) ;

		GameObject guiBGObj = new GameObject( m_BackgroundSceneGUIObjName ) ;
		m_BackgroundSceneGUIObjPtr = guiBGObj ;
		
		GUITexture guiTexture = guiBGObj.AddComponent<GUITexture>() ;
		if( null != guiTexture )
		{
			guiBGObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
			guiTexture.texture = bgTexture ;
			
			float ratioWH = m_BackgroundSceneImageWidth / m_BackgroundSceneImageHeight ;
			float scale = 1.0f ;

			if( ratioWH > 1 )
			{
				// 寬大於高的圖
				// x 方向 0 ~ width
				// y 方向置中
				scale = Screen.width / m_BackgroundSceneImageWidth ;
				float bottom = ( Screen.height - m_BackgroundSceneImageHeight * scale ) / 2 ;

				// float width = ratioWH * Screen.height ;

				guiTexture.pixelInset = new Rect( 0 , 
				                                 bottom , 
				                                 Screen.width, 
				                                 Screen.height / ratioWH ) ;
			}
			else 
			{
				// 寬小於高的圖
				// 將高頂至 0 ~ ScreenHight
				// 寬等比例縮小並且放置中心
				// 寬度為依照比例縮小
				scale = Screen.height / m_BackgroundSceneImageHeight ;
				// Debug.Log( "scale=" + scale ) ;
				float left = ( Screen.width - m_BackgroundSceneImageWidth * scale ) / 2 ;

				guiTexture.pixelInset = new Rect( left , 
				                                 0 , 
				                                 ratioWH * Screen.height , 
				                                 Screen.height ) ;
			}
			// Debug.Log( "guiTexture.pixelInset=" + guiTexture.pixelInset ) ;
		}
		DebugLog( "CreateBackgroundScene() ended." ) ;
	}	
	
	private void DestroyBackgroundSceneObject()
	{
		GameObject.Destroy( m_BackgroundSceneGUIObjPtr ) ;
	}
	
	private void CreateStationsByData()
	{
		DebugLog( "CreateStationsByData() start." ) ;
		
		DestroyStationDisplayObjects() ;
		
		Texture2D stationTexture = (Texture2D)Resources.Load( m_StationIconTexturePath , 
													   typeof(Texture2D) ) ;
		if( null == stationTexture )
		{
			Debug.LogError( "CreateStationsByData() stationTexture load failed=" + m_StationIconTexturePath ) ;
			return ;
		}
		
		foreach( StationData sd in m_Stations.Values ) 
		{
			string objName = "Station_" + sd.DisplayName ;
			GameObject stationObj = new GameObject( objName ) ;
			StationDisplay stationDisplay = null ;
			GUITexture guiTexture = stationObj.AddComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				stationObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
				
				stationObj.transform.Translate( sd.Position.x , sd.Position.y , m_StationLayerZShift ) ;
				guiTexture.texture = stationTexture ;
				guiTexture.pixelInset = new Rect( -1 * 32 / 2 , 
				                                 -1 * 32 / 2 , 
				                                 32 , 
				                                 32 ) ;
				stationDisplay = new StationDisplay() ;
				stationDisplay.Position = new Vector3( sd.Position.x , sd.Position.y , m_StationLayerZShift ) ;
				stationDisplay.ID = sd.ID ;
				stationDisplay.Obj = stationObj ;
				
			}
			
			GameObject textObj = new GameObject( objName + "_Text"  ) ;
			GUIText guiText = textObj.AddComponent<GUIText>() ;
			if( null != guiText )
			{
				guiText.fontSize = 16 ;
				guiText.material.color = Color.black ;
				guiText.text = sd.DisplayName ;
				guiText.pixelOffset = new Vector2 (
					1 * 32 / 2 , 
					0 ) ;
			}
			textObj.transform.parent = stationObj.transform ;
			textObj.transform.localPosition = new Vector3( 0 , 0 , m_StationLayerZShift + 1 ) ;
			
			if( null != stationDisplay )
			{
				m_StationDisplay.Add( sd.ID , stationDisplay ) ;
			}
		}
		
		DebugLog( "CreateStationsByData() ended." ) ;

	}
	
	private void DestroyStationDisplayObjects()
	{
		foreach( StationDisplay sd in m_StationDisplay.Values )
		{
			GameObject.Destroy( sd.Obj ) ;
		}
		m_StationDisplay.Clear() ;
	}
	
	private void CreateTrainsByData()
	{
		DebugLog( "CreateTrainsByData() start." ) ;
		
		DestroyTrainDisplayObjects() ;

		
		foreach( TrainData td in m_TrainData.Values ) 
		{
			bool skip = false ;
	
			DebugLog( "m_DisplayEditorMode=" + m_DisplayEditorMode.ToString() ) ;
			DebugLog( "m_RecreateTrainKeyword=" + m_RecreateTrainKeyword ) ;

			// 圖示
			string textureName = "" ;
			if( true == m_TrainTypeTextureMap.ContainsKey( td.TypeStr ) )
			{
				textureName = m_TrainTypeTextureMap[ td.TypeStr ] ;
			}
			
			Texture2D trainTexture = (Texture2D)Resources.Load( textureName , 
			                                                   typeof(Texture2D) ) ;
			if( null == trainTexture )
			{
				Debug.LogError( "CreateTrainsByData() trainTexture load failed=" + textureName ) ;
				return ;
			}

			switch( m_DisplayEditorMode )
			{
			case DisplayEditorMode.NearStation :
				int stationid_i = 0 ;
				int stationid_j = 0 ;
				
				if( true == FindTrainBetween( td , 
											  ref stationid_i , 
											  ref stationid_j ) )
				{
					
					Dictionary<int , StationData> stationDataVec = m_Stations ;
					
					DebugLog( "i ].DisplayName" + stationDataVec[ stationid_i ].DisplayName ) ;
					DebugLog( "j ].DisplayName" + stationDataVec[ stationid_j ].DisplayName ) ;
					
					if( m_RecreateTrainKeyword != stationDataVec[ stationid_i ].DisplayName &&
						m_RecreateTrainKeyword != stationDataVec[ stationid_j ].DisplayName )
					{
						skip = true ;
					}
				}
				break ;
			case DisplayEditorMode.SpecifiedTrain :
				if( td.DisplayName != m_RecreateTrainKeyword )
					skip = true; 
				break ;
			case DisplayEditorMode.AllTrains :
				// do nothing , all pass.
				break ;				
			case DisplayEditorMode.NoTrain :
				return ;// do nothing at all

			}
			if( true == skip )
				continue ;
			
			
			string objName = "Train_" + td.DisplayName + "(" + td.ID + ")" ;
			DebugLog( "CreateTrainsByData() ." + td.ID ) ;
			GameObject trainObj = new GameObject( objName ) ;

			float trainSize = 32.0f ;

			GUITexture guiTexture = trainObj.AddComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				trainObj.transform.position = new Vector3( 0 , 0 , m_TrainLayerZShift ) ;
				trainObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
				guiTexture.texture = trainTexture ;
				guiTexture.pixelInset = new Rect( -1 * trainSize / 2 , 
				                                 -1 * trainSize / 2 , 
				                                 trainSize , 
				                                 trainSize ) ;
			}
			
			GameObject textObj = new GameObject( objName + "_Text"  ) ;
			GUIText guiText = textObj.AddComponent<GUIText>() ;
			if( null != guiText )
			{
				guiText.fontSize = 24 ;
				guiText.material.color = Color.white ;
				guiText.text = td.DisplayName ;
				guiText.pixelOffset = new Vector2 (
					1 * trainSize / 2 , 
					0 ) ;
			}
			textObj.transform.parent = trainObj.transform ;
			textObj.transform.localPosition = new Vector3( 0 , 0 , m_TrainLayerZShift + 1 ) ;
			
			
			TrainDisplay tDisplay = new TrainDisplay() ;
			tDisplay.ID = td.ID ;
			tDisplay.Obj = trainObj ;
			
			m_TrainDisplay.Add( tDisplay.ID , tDisplay ) ;
		}
		
		DebugLog( "CreateTrainsByData() ended." ) ;
	}	
	
	private void DestroyTrainDisplayObjects()
	{
		foreach( TrainDisplay td in m_TrainDisplay.Values )
		{
			GameObject.Destroy( td.Obj ) ;
		}
		m_TrainDisplay.Clear() ;
	}
	
	
	public bool FindTrainBetween( TrainData _TrainData ,
								  ref int _StationID_i ,
								  ref int _StationID_j )
		
	{	
		return FindTrainBetween( m_ShareHour , 
								 m_ShareMinute ,
								 _TrainData , 
								 m_Stations , 
								 ref _StationID_i , 
								 ref _StationID_j )  ;
	}
	
	public bool FindTrainBetween( int _SpcifiedHour ,
								  int _SpcifiedMinite , 
								  TrainData _TrainData , 
								  Dictionary<int , StationData> _StationDataVec ,
								  ref int _StationID_i ,
								  ref int _StationID_j )
		
	{	
		Debug.Log( "FindTrainBetween() " + _SpcifiedHour + ":" + _SpcifiedMinite ) ;
		
		int j = 0 ;
		int totalMinSpcified = _SpcifiedHour * 60 + _SpcifiedMinite ;
		
		int totalMin_i = 0 ;
		int totalMin_j = 0 ;
		for( int i = 0 ; i < _TrainData.m_TimeTable.Count - 1 ; ++i )
		{
			j = i + 1 ;
			
			int id_i = GetStationIDByStationName( _TrainData.m_TimeTable[ i ].Station ) ;
			int id_j = GetStationIDByStationName( _TrainData.m_TimeTable[ j ].Station ) ;
			
			if( false == _StationDataVec.ContainsKey( id_i ) ||
				false == _StationDataVec.ContainsKey( id_j ) )
				continue ;

			// Vector3 pos_i = _StationDataVec[ id_i ].Position ;
			// Vector3 pos_j = _StationDataVec[ id_j ].Position ;
		
			totalMin_i = _TrainData.m_TimeTable[ i ].Hour * 60 + _TrainData.m_TimeTable[ i ].Minite ;
			totalMin_j = _TrainData.m_TimeTable[ j ].Hour * 60 + _TrainData.m_TimeTable[ j ].Minite ;
				
			DebugLog( "FindTrainBetween() totalMinSpcified" + 
				totalMinSpcified + " totalMin_i" + totalMin_i + " totalMin_j" + totalMin_j ) ;
			
			if( totalMinSpcified >= totalMin_i &&
				totalMinSpcified < totalMin_j )
			{
				// 之間
				_StationID_i = id_i ;
				_StationID_j = id_j ;
				return true ;
			}
			else if( j == _TrainData.m_TimeTable.Count - 1 && 
					 	  totalMinSpcified >= totalMin_j )
			{
				// 最後一個
				_StationID_i = id_j ;
				_StationID_j = id_j ;
				return true ;
			}
			else if( i == 0 && 
					 totalMinSpcified < totalMin_i )
			{
				// 第一站
				_StationID_i = id_i ;
				_StationID_j = id_i ;
				return true ;
			}
		}
		return false ;
	}	
	
	private void DebugLog( string _Log )
	{
#if DEBUG 
			Debug.Log( _Log ) ;
#endif 
	}
}
