/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20130922 by NDark
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGeneratorStatic : MonoBehaviour 
{
	public string m_SysInitFilepath = "" ;
	public string m_StationTableFilepath = "Stations" ;
	public string m_TrainTableFilepath = "" ;
	
	// station data
	public float m_StationLayerZShift = 1 ;
	public string m_StationIconTexturePath = "stationIcon6464" ;
	public Dictionary<int , StationData> m_Stations = new Dictionary<int , StationData>() ;
	
	// background image parameter
	public float m_BackgroundScebeImageWidth = 1200 ;
	public float m_BackgroundScebeImageHeight = 1661 ;
	public string m_BackgroundSceneTexturePath = "map8_1" ;
		
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
		Debug.Log( "LoadStationTable() start." ) ;
		
		TextAsset ta = (TextAsset) Resources.Load( m_StationTableFilepath , typeof(TextAsset) ) ;
		if( null == ta )
		{
			Debug.LogError( "CreateBackgroundScene() m_StationTableFilepath load failed=" + m_StationTableFilepath ) ;
			return ;
		}
		
		// m_Stations
		string content = ta.text ;
		Debug.Log( "content=" + content ) ;
		string[] splitor1 = { "\r\n" , "\n" , "\r" }  ;
		// Debug.Log( "splitor1=" + splitor1[0] ) ;
		StationData stationData = null ;
		string []lineVec = content.Split( splitor1 , System.StringSplitOptions.None ) ;
		Debug.Log( "lineVec.Length=" + lineVec.Length ) ;
		for( int i = 0 ; i < lineVec.Length ; ++i )
		{
			if( 0 < lineVec[ i ].Length )
			{
				Debug.Log( lineVec[ i ] ) ;
				stationData = new StationData() ;
				stationData.ParseFromString( lineVec[ i ] ) ;
				m_Stations.Add( stationData.ID , stationData ) ;
				Debug.Log( "m_Stations.Add=" + stationData.DisplayName ) ;
			}
		}
		Debug.Log( "LoadStationTable() end." ) ;
	}
	
	private void LoadTrainTable( string _Filepath )
	{
		
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
		Texture2D bgTexture = (Texture2D)Resources.Load( m_BackgroundSceneTexturePath , 
													   typeof(Texture2D) ) ;
		if( null == bgTexture )
		{
			Debug.LogError( "CreateBackgroundScene() bgTexture load failed=" + m_BackgroundSceneTexturePath ) ;
			return ;
		}
		
		GameObject guiBGObj = new GameObject( "GUI_BackgroundTextureObj" ) ;
		GUITexture guiTexture = guiBGObj.AddComponent<GUITexture>() ;
		if( null != guiTexture )
		{
			guiBGObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
			guiTexture.texture = bgTexture ;
			
			float ratioWH = m_BackgroundScebeImageWidth / m_BackgroundScebeImageHeight ;
			
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
		Debug.Log( "CreateBackgroundScene() ended." ) ;
	}		
	
	private void CreateStationsByData()
	{
		Debug.Log( "CreateStationsByData() start." ) ;
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
			Debug.Log( "CreateStationsByData() ended." ) ;
			GameObject stationObj = new GameObject( objName ) ;
			
			GUITexture guiTexture = stationObj.AddComponent<GUITexture>() ;
			if( null != guiTexture )
			{
				stationObj.transform.localScale = new Vector3( 0 , 0 , 1 ) ;
				stationObj.transform.Translate( sd.Position.x , sd.Position.y , 1 ) ;
				guiTexture.texture = stationTexture ;
				guiTexture.pixelInset = new Rect( -1 * 64 / 2 , 
					-1 * 64 / 2 , 
					64 , 
					64 ) ;
			}
			
		}
		
		Debug.Log( "CreateStationsByData() ended." ) ;

	}

	private void CreateTrainsByData()
	{
		
	}	
}
