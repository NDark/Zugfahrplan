/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20131012 file started.
*/
using UnityEngine;
using System.Collections.Generic;
using System ;

public class UpdateTrain : MonoBehaviour 
{
	LevelGeneratorStatic pLevelGenerator = null ;
	
	// Use this for initialization
	void Start () 
	{
		pLevelGenerator = this.gameObject.GetComponent<LevelGeneratorStatic>() ;
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
		
		int currentHour = System.DateTime.Now.Hour ;
		int currentMinute = System.DateTime.Now.Minute ;
		
		Dictionary<int , TrainDisplay > trainDisplayVec = pLevelGenerator.m_TrainDisplay ;
		Dictionary<int , TrainData> trainDataVec = pLevelGenerator.m_TrainData ;
		Dictionary<int , StationData> stationDataVec = pLevelGenerator.m_Stations ;
		
		foreach( TrainDisplay trainDisplay in trainDisplayVec.Values )
		{
			Debug.Log( trainDisplay.ID ) ;
			if( false == trainDataVec.ContainsKey( trainDisplay.ID ) )
				continue ;
			
			// 依照時間計算目前火車在哪兩個站間,然後內差
			Vector3 trainPos = FindTrainPosition( currentHour , 
												  currentMinute ,
												  trainDataVec[ trainDisplay.ID ] , 
												  stationDataVec ) ;
			Debug.Log( trainPos ) ;
			trainDisplay.Position = trainPos ;
		}
	}
	
	private Vector3 FindTrainPosition( int _SpcifiedHour ,
									   int _SpcifiedMinite , 
									   TrainData _TrainData , 
									   Dictionary<int , StationData> _StationDataVec ) 
	{
		Vector3 ret = Vector3.zero ;
		int j = 0 ;
		
		int totalMinSpcified = _SpcifiedHour * 60 + _SpcifiedMinite ;
		int totalMin_i = 0 ;
		int totalMin_j = 0 ;
		for( int i = 0 ; i < _TrainData.m_TimeTable.Count - 1 ; ++i )
		{
			j = i + 1 ;
			
			int id_i = pLevelGenerator.GetStationIDByStationName( _TrainData.m_TimeTable[ i ].Station ) ;
			int id_j = pLevelGenerator.GetStationIDByStationName( _TrainData.m_TimeTable[ j ].Station ) ;
			
			if( false == _StationDataVec.ContainsKey( id_i ) ||
				false == _StationDataVec.ContainsKey( id_j ) )
				continue ;

			Vector3 pos_i = _StationDataVec[ id_i ].Position ;
			Vector3 pos_j = _StationDataVec[ id_i ].Position ;
		
			totalMin_i = _TrainData.m_TimeTable[ i ].Hour * 60 + _TrainData.m_TimeTable[ i ].Minite ;
			totalMin_j = _TrainData.m_TimeTable[ j ].Hour * 100 + _TrainData.m_TimeTable[ j ].Minite ;
			if( totalMinSpcified >= totalMin_i &&
				totalMinSpcified < totalMin_j )
			{
				ret = Vector3.Lerp( pos_i , 
							  pos_j , 
							 ( totalMin_i ) / (  totalMin_j - totalMin_i ) ) ;
			}
			else if( j == _TrainData.m_TimeTable.Count - 1 && 
					 	  totalMinSpcified >= totalMin_j )
			{
				ret = pos_j ;
			}
			else if( i == 0 && 
					 totalMinSpcified < totalMin_i )
			{
				ret = pos_i ;
			}
			
		}
		ret.z = pLevelGenerator.m_TrainLayerZShift ;
		return ret ;
	}
}
