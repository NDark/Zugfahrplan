/*
@file LevelGenerator.cs
@author NDark
@date 20130922 by NDark
*/
using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour 
{
	public string m_SysInitFilepath = "" ;
	public string m_StationTableFilepath = "" ;
	public string m_TrainTableFilepath = "" ;
	
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
		
	}
	
	private void LoadTrainTable( string _Filepath )
	{
		
	}

	private void CreateScene()
	{
		CreateBackgroundScene() ;
	}
	
	private void CreateBackgroundScene()
	{
		Debug.Log( "CreateBackgroundScene() started." ) ;
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
}
