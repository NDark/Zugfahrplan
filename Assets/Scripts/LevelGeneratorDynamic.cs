/*
@file LevelGeneratorDynamic.cs
@author NDark
@date 20140501 . file started and derived from LevelGeneratorStatic

*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;
using System.IO ;
using System.Xml ;

public class LevelGeneratorDynamic : MonoBehaviour 
{
	private string m_SysInitFilepath = "SysInit" ;
	public string m_StationTableFilepath = "Stations" ;
	public string m_TrainTableFilepath = "Trains" ;

	// SysInit data
	// a map from train type to texture name ( train type icon ), 
	// check LoadSysInit()
	public Dictionary<string , string > m_TrainTypeTextureMap = new Dictionary<string, string>() ;

	// station data
	public float m_StationLayerZShift = 1 ;
	
	public Dictionary<int , StationData> m_Stations = new Dictionary<int , StationData>() ;
	public Dictionary<int , StationDisplay > m_StationDisplay = new Dictionary<int , StationDisplay>() ;
	
	// train data
	public float m_TrainLayerZShift = 2 ;
	public Dictionary<int , TrainData> m_TrainData = new Dictionary<int , TrainData>() ;
	public Dictionary<int , TrainDisplay > m_TrainDisplay = new Dictionary<int , TrainDisplay>() ;

	// background image parameter
	public float m_BackgroundSceneLayerZShift = 30 ;
	public float m_BackgroundSceneImageWidth = 1200 ;// 寬高 是用來 創造物件的比例
	public float m_BackgroundSceneImageHeight = 1661 ;
	public string m_BackgroundSceneTexturePath = "map8_1" ;
	public string m_BackgroundSceneGUIObjName = "GUI_BackgroundTextureObj" ;
	public GameObject m_BackgroundSceneGUIObjPtr = null ;

	private DisplayEditorMode m_DisplayEditorMode = DisplayEditorMode.AllTrains ;
	private string m_RecreateTrainKeyword = "" ;

	public CameraControl pCameraControl = null ;
	
	// Time
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
		int maxID = 0;
		foreach( StationData sd in m_Stations.Values )
		{
			if( sd.ID > maxID )
			{
				maxID = sd.ID ;
			}
		}
		return maxID + 1 ;
	}

	public int GetANewTrainID()
	{
		int maxID = 0 ;
		foreach( TrainData td in m_TrainData.Values )
		{
			if( td.ID > maxID )
			{
				maxID = td.ID ;
			}
		}
		return maxID + 1 ;
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

	public void ReCreateTrainDisplayByData( DisplayEditorMode _Mode , 
	                                        string _Keyword )
	{
		m_DisplayEditorMode = _Mode ;
		Debug.Log ("m_DisplayEditorMode=" + m_DisplayEditorMode);
		m_RecreateTrainKeyword = _Keyword ;
		
		CreateTrainsByData() ;
	}

	public void AdjustStationDisplayPos( string _StationLabel , 
	                                     Vector3 _NewPos )
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
		pCameraControl = this.gameObject.GetComponent<CameraControl>() ;
		if( null == pCameraControl )
		{
			Debug.LogError( "null == pCameraControl" ) ;
		}	
		
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
		Debug.Log( "LoadSysInit() _Filepath=" + _Filepath ) ;

		TextAsset tx = Resources.Load<TextAsset>( _Filepath ) ;
		doc.LoadXml( tx.text ) ;

		XmlNode root = doc.FirstChild;
		if( null != root && 
		    root.HasChildNodes )
		{
			Debug.Log( "LoadSysInit(), root.ChildNodes.Count=" + 
			          root.ChildNodes.Count ) ;
			foreach( XmlNode node in root.ChildNodes )
			{
				// <Traintype label="自強" textureName="自強" />
				if( "Traintype" == node.Name )
				{
					if( null != node.Attributes[ "label" ] &&
					    null != node.Attributes[ "textureName" ] )
					{
						string label = node.Attributes[ "label" ].Value ;
						string textureName = node.Attributes[ "label" ].Value ;

						Debug.Log( "LoadSysInit() label=" + label + ", textureName=" + textureName ) ;
						m_TrainTypeTextureMap.Add( label, textureName ) ;
					}
				}
				else 
				{
					// other kind of node
				}
			}
		}
	}// end of LoadSysInit()

	private void LoadStationTable( string _Filepath )
	{
		Debug.Log( "LoadStationTable() start. _Filepath=" + _Filepath ) ;
		
		DestroyStationsData() ;
		
		TextAsset ta = (TextAsset) Resources.Load( _Filepath , 
		                                          typeof(TextAsset) ) ;
		if( null == ta )
		{
			Debug.LogError( "LoadStationTable() _Filepath load failed=" + _Filepath ) ;
			return ;
		}
		
		// m_Stations
		string content = ta.text ;
		// Debug.Log( "content=" + content ) ;
		string[] splitor1 = { "\r\n" , "\n" , "\r" }  ;
		// Debug.Log( "splitor1=" + splitor1[0] ) ;
		StationData stationData = null ;
		string []lineVec = content.Split( splitor1 , 
		                                 System.StringSplitOptions.None ) ;

		// Debug.Log( "lineVec.Length=" + lineVec.Length ) ;
		for( int i = 0 ; i < lineVec.Length ; ++i )
		{
			if( 0 < lineVec[ i ].Length )
			{
				// Debug.Log( lineVec[ i ] ) ;
				stationData = new StationData() ;
				stationData.ParseFromString( lineVec[ i ] ) ;
				if( false == m_Stations.ContainsKey( stationData.ID ) )
				{
					m_Stations.Add( stationData.ID , stationData ) ;
					Debug.Log( "m_Stations.Add=" + stationData.DisplayName ) ;
				}
			}
		}

		Debug.Log( "LoadStationTable() end." ) ;
	}// end of LoadStationTable()

	private void DestroyStationsData()
	{
		m_Stations.Clear() ;
	}

	private void LoadTrainTable( string _Filepath )
	{
		Debug.Log( "LoadTrainTable() start. _Filepath=" + _Filepath ) ;
		
		DestroyTrainData() ;
		
		TextAsset ta = (TextAsset) Resources.Load( _Filepath , 
		                                          typeof(TextAsset) ) ;
		if( null == ta )
		{
			Debug.LogError( "LoadTrainTable() _Filepath load failed=" + _Filepath ) ;
			return ;
		}
		
		// m_TrainData
		string content = ta.text ;
		// Debug.Log( "content=" + content ) ;
		string[] splitor1 = { "\r\n" , "\n" , "\r" }  ;
		// Debug.Log( "splitor1=" + splitor1[0] ) ;

		TrainData trainData = null ;
		string []lineVec = content.Split( splitor1 , 
		                                 System.StringSplitOptions.None ) ;
		// Debug.Log( "lineVec.Length=" + lineVec.Length ) ;
		for( int i = 0 ; i < lineVec.Length ; ++i )
		{
			if( 0 < lineVec[ i ].Length )
			{
				// Debug.Log( lineVec[ i ] ) ;
				trainData = new TrainData() ;
				
				trainData.ParseFromString( lineVec[ i ] ) ;
				m_TrainData.Add( trainData.ID , trainData ) ;
				Debug.Log( "m_TrainData.Add=" + trainData.DisplayName ) ;
			}
		}
		Debug.Log( "LoadTrainTable() end." ) ;
	}// LoadTrainTable

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
		Debug.Log( "CreateBackgroundScene() start." ) ;
		
		DestroyBackgroundSceneObject() ;
		
		Texture2D bgTexture = (Texture2D)Resources.Load( m_BackgroundSceneTexturePath , 
		                                                typeof(Texture2D) ) ;
		if( null == bgTexture )
		{
			Debug.LogError( "CreateBackgroundScene() bgTexture load failed=" + m_BackgroundSceneTexturePath ) ;
			return ;
		}
		
		
		Object prefab = Resources.Load ("CubePrefab"); 
		if (null == prefab) 
		{
			Debug.LogError( "CreateBackgroundScene() CubePrefab load failed=" ) ;
			return ;
		}

		GameObject guiBGObj = (GameObject)GameObject.Instantiate(prefab);
		guiBGObj.name = m_BackgroundSceneGUIObjName ;
		m_BackgroundSceneGUIObjPtr = guiBGObj ;
		
		Renderer renderer = guiBGObj.GetComponent<Renderer>() ;
		if (null == renderer) 
		{
			Debug.LogError( "CreateBackgroundScene() Renderer get failed=" ) ;
			return;
		}

		renderer.material = new Material (Shader.Find("Diffuse"));
		renderer.material.mainTexture = bgTexture;

		// 用來調整物件寬與高的大小
		guiBGObj.transform.localScale = new Vector3 ( -1 * m_BackgroundSceneImageWidth, 
		                                             -1 * m_BackgroundSceneImageHeight, 1 ) ; 
		guiBGObj.transform.position = new Vector3 ( m_BackgroundSceneImageWidth / 2.0f , 
		                                           m_BackgroundSceneImageHeight / 2.0f  , 
		                                          m_BackgroundSceneLayerZShift);
		Camera.main.transform.position = new Vector3 (guiBGObj.transform.position.x, 
		                                             guiBGObj.transform.position.y,
		                                             Camera.main.transform.position.z);
		Debug.Log( "CreateBackgroundScene() ended." ) ;
	}// end of CreateBackgroundScene()

	private void DestroyBackgroundSceneObject()
	{
		GameObject.Destroy( m_BackgroundSceneGUIObjPtr ) ;
	}

	private void CreateStationsByData()
	{
		Debug.Log( "CreateStationsByData() start." ) ;
		
		DestroyStationDisplayObjects() ;
		
		Object prefab = Resources.Load ("StationPrefab"); 
		if (null == prefab) 
		{
			Debug.LogError( "CreateStationsByData() CubePrefab load failed" ) ;
			return ;
		}
		Object textMeshPrefab = Resources.Load ("TextMeshPrefab"); 
		if (null == textMeshPrefab) 
		{
			Debug.LogError( "CreateStationsByData() textMeshPrefab load failed" ) ;
			return ;
		}
		
		foreach( StationData sd in m_Stations.Values ) 
		{
			string objName = "Station_" + sd.DisplayName ;
			GameObject stationObj =(GameObject) GameObject.Instantiate( prefab ) ;
			stationObj.name = objName ;

			StationDisplay stationDisplay = null ;
			Renderer renderer = stationObj.GetComponent<Renderer>() ;
			if( null != renderer )
			{
				stationObj.transform.position = new Vector3( sd.Position.x , 
				                               sd.Position.y ,
				                               m_StationLayerZShift ) ;

				stationDisplay = new StationDisplay() ;
				stationDisplay.Position = new Vector3( sd.Position.x , 
				                                      sd.Position.y , 
				                                      m_StationLayerZShift ) ;
				stationDisplay.ID = sd.ID ;
				stationDisplay.Obj = stationObj ;

				GameObject textObj = (GameObject) GameObject.Instantiate( textMeshPrefab ) ;
				textObj.name = objName + "_Text" ;
				TextMesh textMesh = textObj.GetComponent<TextMesh>() ;
				if( null != textMesh )
				{
					textMesh.text = sd.DisplayName ;
				}
				textObj.transform.parent = stationObj.transform ;
				textObj.transform.localPosition = Vector3.zero ;
				textObj.transform.localScale = new Vector3( -0.5f , -0.5f , 1 ) ;

				m_StationDisplay.Add( sd.ID , stationDisplay ) ;
			}
		}
		Debug.Log( "CreateStationsByData() ended." ) ;
	}// end of CreateStationsByData()

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
		Debug.Log( "CreateTrainsByData() start." ) ;
		
		DestroyTrainDisplayObjects() ;

		Object prefab = Resources.Load ("TrainPrefab"); 
		if (null == prefab) 
		{
			Debug.LogError( "CreateTrainsByData() CubePrefab load failed=" ) ;
			return ;
		}
		Object textMeshPrefab = Resources.Load ("TextMeshPrefab"); 
		if (null == textMeshPrefab) 
		{
			Debug.LogError( "CreateTrainsByData() textMeshPrefab load failed" ) ;
			return ;
		}

		if( null != this.pCameraControl &&
			m_DisplayEditorMode != DisplayEditorMode.SpecifiedTrain )
		{
			this.pCameraControl.trackingObject = null ;
		}
		
		
		foreach( TrainData td in m_TrainData.Values ) 
		{
			bool skip = false ;
			
			Debug.Log( "m_DisplayEditorMode=" + m_DisplayEditorMode.ToString() ) ;
			Debug.Log( "m_RecreateTrainKeyword=" + m_RecreateTrainKeyword ) ;
			


			Dictionary<int , StationData> stationDataVec = m_Stations ;
			switch( m_DisplayEditorMode )
			{
			case DisplayEditorMode.NearStation :

				int stationid_i = 0 ;
				int stationid_j = 0 ;
				
				if( true == FindTrainBetween( td , 
				                             ref stationid_i , 
				                             ref stationid_j ) )
				{
					
					// Debug.Log( "i ].DisplayName" + stationDataVec[ stationid_i ].DisplayName ) ;
					// Debug.Log( "j ].DisplayName" + stationDataVec[ stationid_j ].DisplayName ) ;
					if( m_RecreateTrainKeyword != stationDataVec[ stationid_i ].DisplayName &&
					    m_RecreateTrainKeyword != stationDataVec[ stationid_j ].DisplayName )
					{
						skip = true ;
					}
				}
				break ;

			case DisplayEditorMode.SpecifiedTrain :
				if( td.DisplayName != m_RecreateTrainKeyword )
				{
					skip = true; 
				}
				break ;

			case DisplayEditorMode.AllTrains :
				// do nothing , all pass.
				break ;				

			case DisplayEditorMode.NoTrain :
				return ;// do nothing at all
				
			}

			if( true == skip )
				continue ;

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
				continue ;
			}

			string objName = "Train_" + td.DisplayName + "(" + td.ID + ")" ;
			Debug.Log( "CreateTrainsByData() ." + objName ) ;
			GameObject trainObj = (GameObject)GameObject.Instantiate( prefab );
			trainObj.name = objName ;
			
			Renderer renderer = trainObj.GetComponent<Renderer>() ;
			if( null != renderer )
			{
				trainObj.transform.position = new Vector3( 0 , 0 , m_TrainLayerZShift ) ;
				renderer.material = new Material (Shader.Find(" Diffuse"));
				renderer.material.mainTexture = trainTexture ;
			}
			
			GameObject textObj = (GameObject) GameObject.Instantiate( textMeshPrefab );
			textObj.name = objName + "_Text" ;
			TextMesh textMesh = textObj.GetComponent<TextMesh>() ;
			if( null != textMesh )
			{
				textMesh.text = td.DisplayName ;

			}
			textObj.transform.parent = trainObj.transform ;
			textObj.transform.localPosition = Vector3.zero ;
			textObj.transform.localScale = new Vector3( -0.5f , -0.5f , 1 ) ;
			
			TrainDisplay tDisplay = new TrainDisplay() ;
			tDisplay.ID = td.ID ;
			tDisplay.Obj = trainObj ;
			
			
			m_TrainDisplay.Add( tDisplay.ID , tDisplay ) ;
			
		    if( null != this.pCameraControl &&
			    m_DisplayEditorMode == DisplayEditorMode.SpecifiedTrain )
		    {
				Debug.Log( "td.ID " ) ;
				this.pCameraControl.trackingObject = 
					tDisplay.Obj ;
			}

		}
		
		Debug.Log( "CreateTrainsByData() ended." ) ;
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
		// Debug.Log( "FindTrainBetween() " + _SpcifiedHour + ":" + _SpcifiedMinite ) ;
		
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
			
			// Debug.Log( "FindTrainBetween() totalMinSpcified" + totalMinSpcified + " totalMin_i" + totalMin_i + " totalMin_j" + totalMin_j ) ;
			
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
}
