/**
@file UpdateTrainDynamic.cs
@author NDark
@date 20140501 file started.

*/
// #define DEBUG
using UnityEngine;
using System.Collections.Generic;
// using System ;


public class UpdateTrainDynamic : MonoBehaviour 
{
	LevelGeneratorDynamic pLevelGenerator = null ;
	
	
	public UpdateTrainTimeMode m_TimeMode = UpdateTrainTimeMode.SystemTime ;
	public int m_SpecifiedHour = 0 ;
	public int m_SpecifiedMinute = 0 ;
	
	public void SetUpdateTrainTimeMode( UpdateTrainTimeMode _UpdateTrainTimeMode ) 
	{
		m_TimeMode = _UpdateTrainTimeMode ;
	}
	
	public void SetSpecifiedTime( int _TotalMinute ) 
	{
		int hour = _TotalMinute / 60 ;
		int minute = _TotalMinute % 60 ; 
		SetSpecifiedTime( hour , minute ) ;
	}
	
	public void SetSpecifiedTime( int _Hour , int _Minute ) 
	{
		m_SpecifiedHour = _Hour ;
		m_SpecifiedMinute = _Minute ;
	}
	
	
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
		
		int currentHour = System.DateTime.Now.Hour ;
		int currentMinute = System.DateTime.Now.Minute ;
		
		int specifiedHour = m_SpecifiedHour ;
		int specifiedMinute = m_SpecifiedMinute ;
		
		switch( m_TimeMode )
		{
		case UpdateTrainTimeMode.SystemTime:
			m_SpecifiedHour = specifiedHour = currentHour ;
			m_SpecifiedMinute = specifiedMinute = currentMinute ;
			break; 
		case UpdateTrainTimeMode.SpecifiedTime :
			
			break ;
		}
		pLevelGenerator.SetShareTime( specifiedHour , specifiedMinute ) ;
		
		Dictionary<int , TrainDisplay > trainDisplayVec = pLevelGenerator.m_TrainDisplay ;
		Dictionary<int , TrainData> trainDataVec = pLevelGenerator.m_TrainData ;
		Dictionary<int , StationData> stationDataVec = pLevelGenerator.m_Stations ;
		
		Dictionary<int , TrainDisplay >.Enumerator trainDisplayE = trainDisplayVec.GetEnumerator() ;
		while( trainDisplayE.MoveNext() )
		{
			// Debug.Log( trainDisplay.ID ) ;
			if( false == trainDataVec.ContainsKey( trainDisplayE.Current.Key ) )
				continue ;
			int targetID = 0 ;
			// 依照時間計算目前火車在哪兩個站間,然後內差得到目前位置
			Vector3 trainPos = FindTrainPosition( specifiedHour , 
			                                     specifiedMinute ,
			                                     trainDataVec[ trainDisplayE.Current.Key ] , 
			                                     stationDataVec , 
			                                     ref targetID ) ;
			
			DebugLog( "trainPos=" + trainPos ) ;
			trainDisplayE.Current.Value.Position = trainPos ;
			trainDisplayE.Current.Value.Obj.transform.position = trainPos ;
			trainDisplayE.Current.Value.TargetStationID = targetID ;
			DebugLog( "UpdateTrainDynamic::Update()" + trainDisplayE.Current.Value.Obj.transform.position ) ;
		}
	}
	
	
	
	
	private Vector3 FindTrainPosition( int _SpcifiedHour ,
	                                  int _SpcifiedMinite , 
	                                  TrainData _TrainData , 
	                                  Dictionary<int , StationData> _StationDataVec ,
	                                  ref int _TargetID ) 
	{
		DebugLog( _SpcifiedHour + ":" + _SpcifiedMinite ) ;
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
			DebugLog( "_TrainData.m_TimeTable[ i ].Station" + _TrainData.m_TimeTable[ i ].Station ) ;
			DebugLog( "_TrainData.m_TimeTable[ j ].Station" + _TrainData.m_TimeTable[ j ].Station ) ;
			
			
			if( false == _StationDataVec.ContainsKey( id_i ) ||
			   false == _StationDataVec.ContainsKey( id_j ) )
			{
				DebugLog( "false == _StationDataVec" + id_i + ","+ id_j ) ;
				continue ;
			}
			
			Vector3 pos_i = _StationDataVec[ id_i ].Position ;
			Vector3 pos_j = _StationDataVec[ id_j ].Position ;
			
			totalMin_i = _TrainData.m_TimeTable[ i ].Hour * 60 + _TrainData.m_TimeTable[ i ].Minite ;
			totalMin_j = _TrainData.m_TimeTable[ j ].Hour * 60 + _TrainData.m_TimeTable[ j ].Minite ;
			
			DebugLog( "totalMinSpcified=" + totalMinSpcified + " totalMin_i=" + totalMin_i + " totalMin_j" + totalMin_j ) ;
			
			
			float interpolateValue =   (float) ( totalMinSpcified - totalMin_i ) / 
				( (float) totalMin_j - (float) totalMin_i ) ;
			if( totalMinSpcified >= totalMin_i &&
			   totalMinSpcified < totalMin_j )
			{
				// 在i,j之間
				_TargetID = id_j ;
				// Debug.Log( "interpolateValue" + interpolateValue ) ;
				ret = Vector3.Lerp( pos_i , 
				                   pos_j , 
				                   interpolateValue ) ;
				DebugLog( "pos_i" + pos_i ) ;
				DebugLog( "pos_j" + pos_j ) ;
				DebugLog( "ret" + ret ) ;
				break ;
			}
			else if( j == _TrainData.m_TimeTable.Count - 1 && 
			        totalMinSpcified >= totalMin_j )
			{
				// 在最後一位
				_TargetID = id_j ;
				DebugLog( "pos_j" + pos_j ) ;
				ret = Vector3.Lerp( pos_i , 
				                   pos_j , 
				                   1 ) ;
				break ;
			}
			else if( i == 0 && 
			        totalMinSpcified < totalMin_i )
			{
				// 在第一位
				_TargetID = id_i ;
				DebugLog( "pos_i" + pos_i ) ;
				ret = Vector3.Lerp( pos_i , 
				                   pos_j , 
				                   0 ) ;
				break ;
			}
		}
		
		ret.z = pLevelGenerator.m_TrainLayerZShift ;
		DebugLog( "ret" + ret ) ;
		return ret ;
	}
	
	private void DebugLog( string _Log )
	{
		#if DEBUG 
		Debug.Log( _Log ) ;
		#endif 
	}
}
