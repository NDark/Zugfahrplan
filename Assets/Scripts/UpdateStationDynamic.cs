/**
@file UpdateStationDynamic.cs
@author NDark
@date 20140501 file started.
*/
using UnityEngine;
using System.Collections.Generic;

public class UpdateStationDynamic : MonoBehaviour 
{
	LevelGeneratorDynamic pLevelGenerator = null ;
	public StationDisplayMode m_StationDisplayMode = StationDisplayMode.AllStaion ;
	
	// Use this for initialization
	void Start () 
	{
		pLevelGenerator = this.gameObject.GetComponent<LevelGeneratorDynamic>() ;
		if( null == pLevelGenerator )
		{
			Debug.LogError( "null == pLevelGenerator" ) ;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if( null == pLevelGenerator )
			return ;
		
		Dictionary<int , StationData> stationDataVec = pLevelGenerator.m_Stations ;
		Dictionary<int , StationDisplay > stationDisplayVec = pLevelGenerator.m_StationDisplay ;
		Dictionary<int , TrainDisplay > trainDisplayVec = pLevelGenerator.m_TrainDisplay ;
		
		Vector3 mousePositionPixel = Input.mousePosition ;
		
		Vector3 mouseViewport = Camera.main.ScreenToViewportPoint( mousePositionPixel ) ;
		float threashold = 0.03f ;
		
		Dictionary<int , StationDisplay >.Enumerator stationDisplayE = stationDisplayVec.GetEnumerator() ;
		while( stationDisplayE.MoveNext() )
		{
			bool isShow = false ;
			GameObject displayObj = stationDisplayE.Current.Value.Obj ;
			int stationID = stationDisplayE.Current.Key ;
			
			switch( m_StationDisplayMode )
			{
			case StationDisplayMode.AllStaion :
				isShow = true ;
				break ;
			case StationDisplayMode.BigStation :
			{	
				StationType stationType = stationDataVec [ stationID ].ThisStationType ;
				if( StationType.Big == stationType )
				{
					isShow = true ;
				}
			}
				break ;
			case StationDisplayMode.SmartDisplay :
			{
				// 檢查是否是大站
				
				StationType stationType = stationDataVec [ stationID ].ThisStationType ;
				if( StationType.Big == stationType )
				{
					isShow = true ;
				}
				else
				{
					bool targetStation = false ;
					// 檢查不是大站,就要掃過所有的列車,看是否正要抵達
					Dictionary<int , TrainDisplay >.Enumerator trainDisplayE = trainDisplayVec.GetEnumerator() ;
					while( trainDisplayE.MoveNext() )
					{
						if( stationID == trainDisplayE.Current.Value.TargetStationID )
						{
							targetStation = true ;
							break ;
						}
					}
					
					if( true == targetStation )
					{
						isShow = true ;
					}
				}
				
				// 檢查滑鼠位置,是否接近
				Vector3 distanceVec = displayObj.transform.position - mouseViewport ;
				distanceVec.z = 0 ;
				//			Debug.Log( "displayObj.transform.position" + displayObj.transform.position ) ;
				//			Debug.Log( "mouseViewport" + mouseViewport ) ;
				//			Debug.Log( "distanceVec.magnitude" + distanceVec.magnitude ) ;
				if( distanceVec.magnitude < threashold )
				{
					isShow = true ;
				}
			}
				break ;
			}
			
			
			
			
			SetStationVisible( displayObj , isShow ) ;
		}
	}
	
	private void SetStationVisible( GameObject _Obj , bool _Set )
	{
		if( null != _Obj.GetComponent<GUITexture>() )
		{
			_Obj.GetComponent<GUITexture>().enabled = _Set ;
		}
		GUIText text = null ;
		if( null != ( text = _Obj.GetComponentInChildren<GUIText>() ) )
		{
			text.enabled = _Set ;
		}
	}
	
}



