/*
@file LevelGeneratorStatic.cs
@author NDark
@date 20131012 file started.
*/
using UnityEngine;
using System.Collections.Generic;

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
		
		
		Dictionary<int , TrainDisplay > trainDisplayVec = pLevelGenerator.m_TrainDisplay ;
		Dictionary<int , TrainData> trainDataVec = pLevelGenerator.m_TrainData ;
		Dictionary<int , StationData> stationDataVec = pLevelGenerator.m_Stations ;
		foreach( TrainDisplay trainDisplay in trainDisplayVec.Values )
		{
			// 依照時間計算目前火車在哪兩個站間,然後內差
		}
	}
}
