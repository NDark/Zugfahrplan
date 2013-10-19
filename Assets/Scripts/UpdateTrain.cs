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
	public enum TimeMode
	{
		SystemTime ,
		SpecifiedTime ,
	}
	
	public TimeMode m_TimeMode = TimeMode.SystemTime ;
	public int m_SpecifiedHour = 0 ;
	public int m_SpecifiedMinute = 0 ;
	
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
		
		int specifiedHour = m_SpecifiedHour ;
		int specifiedMinute = m_SpecifiedMinute ;
		
		switch( m_TimeMode )
		{
		case TimeMode.SystemTime:
			specifiedHour = currentHour ;
			specifiedMinute = currentMinute ;
			break; 
		case TimeMode.SpecifiedTime :
			break ;
		}
		
		Dictionary<int , TrainDisplay > trainDisplayVec = pLevelGenerator.m_TrainDisplay ;
		Dictionary<int , TrainData> trainDataVec = pLevelGenerator.m_TrainData ;
		Dictionary<int , StationData> stationDataVec = pLevelGenerator.m_Stations ;
		
		Dictionary<int , TrainDisplay >.Enumerator trainDisplayE = trainDisplayVec.GetEnumerator() ;
		while( trainDisplayE.MoveNext() )
		{
			// Debug.Log( trainDisplay.ID ) ;
			if( false == trainDataVec.ContainsKey( trainDisplayE.Current.Key ) )
				continue ;
			
			// 依照時間計算目前火車在哪兩個站間,然後內差得到目前位置
			Vector3 trainPos = FindTrainPosition( specifiedHour , 
												  specifiedMinute ,
												  trainDataVec[ trainDisplayE.Current.Key ] , 
												  stationDataVec ) ;
			
			Debug.Log( "trainPos=" + trainPos ) ;
			trainDisplayE.Current.Value.Position = trainPos ;
			trainDisplayE.Current.Value.Obj.transform.position = trainPos ;
			Debug.Log( "UpdateTrain::Update()" + trainDisplayE.Current.Value.Obj.transform.position ) ;
		}
	}
	
	private Vector3 FindTrainPosition( int _SpcifiedHour ,
									   int _SpcifiedMinite , 
									   TrainData _TrainData , 
									   Dictionary<int , StationData> _StationDataVec ) 
	{
		Debug.Log( _SpcifiedHour + ":" + _SpcifiedMinite ) ;
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
			Vector3 pos_j = _StationDataVec[ id_j ].Position ;
		
			totalMin_i = _TrainData.m_TimeTable[ i ].Hour * 60 + _TrainData.m_TimeTable[ i ].Minite ;
			totalMin_j = _TrainData.m_TimeTable[ j ].Hour * 60 + _TrainData.m_TimeTable[ j ].Minite ;
				
			Debug.Log( "totalMinSpcified" + totalMinSpcified + " totalMin_i" + totalMin_i + " totalMin_j" + totalMin_j ) ;
			
			
			float interpolateValue =   (float) ( totalMinSpcified - totalMin_i ) / 
									 ( (float) totalMin_j - (float) totalMin_i ) ;
			if( totalMinSpcified >= totalMin_i &&
				totalMinSpcified < totalMin_j )
			{
				Debug.Log( "interpolateValue" + interpolateValue ) ;
				ret = Vector3.Lerp( pos_i , 
							  pos_j , 
							  interpolateValue ) ;
				Debug.Log( "pos_i" + pos_i ) ;
				Debug.Log( "pos_j" + pos_j ) ;
				Debug.Log( "ret" + ret ) ;
				break ;
			}
			else if( j == _TrainData.m_TimeTable.Count - 1 && 
					 	  totalMinSpcified >= totalMin_j )
			{
				Debug.Log( "pos_j" + pos_j ) ;
				ret = Vector3.Lerp( pos_i , 
							  pos_j , 
							 1 ) ;
				break ;
			}
			else if( i == 0 && 
					 totalMinSpcified < totalMin_i )
			{
				Debug.Log( "pos_i" + pos_i ) ;
				ret = Vector3.Lerp( pos_i , 
							  pos_j , 
							 0 ) ;
				break ;
			}
		}
		
		ret.z = pLevelGenerator.m_TrainLayerZShift ;
		Debug.Log( "ret" + ret ) ;
		return ret ;
	}
}
