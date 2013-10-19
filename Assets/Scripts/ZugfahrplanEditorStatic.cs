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
	// Use this for initialization
	void Start () 
	{
		
		m_LevelGeneratorStaticPtr = this.gameObject.GetComponent<LevelGeneratorStatic>() ;
		if( null == m_LevelGeneratorStaticPtr )
		{
			
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
	
	private void DrawEditor( int _ID )
	{
		m_DisplayBackgroundImageInformation = GUILayout.Toggle( m_DisplayBackgroundImageInformation , "SceneBackgroundImage Information" ) ;
		if( true == m_DisplayBackgroundImageInformation )
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
		
		
		if( true == GUILayout.Button( "Recreate All Scene" ) ) 
		{
			m_LevelGeneratorStaticPtr.ReCreateScene() ;
		}
		
		if( true == GUILayout.Button( "Close" ) ) 
		{
			m_DisplayEditor = false ;
		}
		GUI.DragWindow() ;
	}
}
