/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20130922 by NDark
@date 20131019 by NDark 
. rename class member m_BackgroundScebeImageWidth by m_BackgroundSceneImageWidth
. rename class member m_BackgroundScebeImageHeight by m_BackgroundSceneImageHeight
. 增加各種公開介面
. add class method AdjustStationDisplayPos()

*/
using UnityEngine;
using System.Collections.Generic;
using System.IO ;

public class LevelGeneratorStatic : MonoBehaviour 
{
	public string m_SysInitFilepath = "" ;
	public string m_StationTableFilepath = "Stations" ;
	public string m_TrainTableFilepath = "Trains" ;
	
	// station data
	public float m_StationLayerZShift = 1 ;
	public string m_StationIconTexturePath = "stationIcon6464" ;
	public Dictionary<int , StationData> m_Stations = new Dictionary<int , StationData>() ;
	public Dictionary<int , StationDisplay > m_StationDisplay = new Dictionary<int , StationDisplay>() ;

	// train data
	public float m_TrainLayerZShift = 2 ;
	public string m_TrainIconTexturePath = "TrainSymbol" ;
	public Dictionary<int , TrainData> m_TrainData = new Dictionary<int , TrainData>() ;
	public Dictionary<int , TrainDisplay > m_TrainDisplay = new Dictionary<int , TrainDisplay>() ;
	
	// background image parameter
	public float m_BackgroundSceneImageWidth = 1200 ;
	public float m_BackgroundSceneImageHeight = 1661 ;
	public string m_BackgroundSceneTexturePath = "map8_1" ;
	public string m_BackgroundSceneGUIObjName = "GUI_BackgroundTextureObj" ;
	public GameObject m_BackgroundSceneGUIObjPtr = null ;
	
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
	
	
	public void SwapStationData( int _stationID )
	{
		
		
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
				Debug.Log( lineVec[ i ] ) ;
				stationData = new StationData() ;
				stationData.ParseFromString( lineVec[ i ] ) ;
				m_Stations.Add( stationData.ID , stationData ) ;
				DebugLog( "m_Stations.Add=" + stationData.DisplayName ) ;
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
		
		GameObject guiBGObj = new GameObject( m_BackgroundSceneGUIObjName ) ;
		m_BackgroundSceneGUIObjPtr = guiBGObj ;
		
		GUITexture guiTexture = guiBGObj.AddComponent<GUITexture>() ;
		if( null != guiTexture )
		{
			guiBGObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
			guiTexture.texture = bgTexture ;
			
			float ratioWH = m_BackgroundSceneImageWidth / m_BackgroundSceneImageHeight ;
			
			float width = 0 ;
			if( ratioWH > 1 )
			{
				width = ratioWH * Camera.mainCamera.GetScreenHeight() ;
				guiTexture.pixelInset = new Rect( width / 2 , 0 , 
					Camera.mainCamera.GetScreenWidth() , 
					Camera.mainCamera.GetScreenWidth() / ratioWH ) ;
			}
			else 
			{
				width = ratioWH * Camera.mainCamera.GetScreenHeight() ;
				guiTexture.pixelInset = new Rect( width / 2 , 0 , 
					ratioWH * Camera.mainCamera.GetScreenHeight() , 
					Camera.mainCamera.GetScreenHeight() ) ;
			}
			
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
			
			GUITexture guiTexture = stationObj.AddComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				stationObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
				
				stationObj.transform.Translate( sd.Position.x , sd.Position.y , m_StationLayerZShift ) ;
				guiTexture.texture = stationTexture ;
				guiTexture.pixelInset = new Rect( -1 * 64 / 2 , 
					-1 * 64 / 2 , 
					64 , 
					64 ) ;
				StationDisplay stationDisplay = new StationDisplay() ;
				stationDisplay.Position = new Vector3( sd.Position.x , sd.Position.y , m_StationLayerZShift ) ;
				stationDisplay.ID = sd.ID ;
				stationDisplay.Obj = stationObj ;
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
		
		Texture2D trainTexture = (Texture2D)Resources.Load( m_TrainIconTexturePath , 
													   typeof(Texture2D) ) ;
		if( null == trainTexture )
		{
			Debug.LogError( "CreateTrainsByData() trainTexture load failed=" + m_TrainIconTexturePath ) ;
			return ;
		}
		
		foreach( TrainData td in m_TrainData.Values ) 
		{
			string objName = "Train_" + td.DisplayName + "(" + td.ID + ")" ;
			DebugLog( "CreateTrainsByData() ." + td.ID ) ;
			GameObject trainObj = new GameObject( objName ) ;
			
			
			GUITexture guiTexture = trainObj.AddComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				trainObj.transform.position = new Vector3( 0 , 0 , m_TrainLayerZShift ) ;
				trainObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
				guiTexture.texture = trainTexture ;
				guiTexture.pixelInset = new Rect( -1 * 64 / 2 , 
					-1 * 64 / 2 , 
					64 , 
					64 ) ;
			}
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
	

	
	
	private void DebugLog( string _Log )
	{
#if DEBUG 
		Debug.Log( _Log ) ;
#endif 
	}
}
