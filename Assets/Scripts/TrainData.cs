/*
@file TrainData.cs
@author NDark
@date 20131012 . file created.
@date 20131220 
. add class method CopyFrom()
. add class method RemoveTimeRoute()
@date 20140329 by NDark . add class member m_TypeStr
@date 20140504 by NDark . add logging at ParseTimeTable()

*/
using UnityEngine;
using System.Collections.Generic ;

public class TrainData 
{
	public int ID 
	{
		set
		{
			m_ID = value ;
		}
		get
		{
			return m_ID ;
		}
	}
	
	public string DisplayName 
	{
		set
		{
			m_DisplayName = value ;
		}
		get
		{
			return m_DisplayName ;
		}
	}	

	public string TypeStr 
	{
		set
		{
			m_TypeStr = value ;
		}
		get
		{
			return m_TypeStr ;
		}
	}	
	
	
	public void CopyFrom( TrainData _Src )
	{
		Setup( this.ID ,
			   _Src.m_DisplayName ) ;
		m_TimeTable = _Src.m_TimeTable ;
	}
	
	public void Setup( int _ID , 
					   string _DisplayName )
	{
		ID = _ID ;
		DisplayName = _DisplayName ;
	}
	
	public bool ParseFromString( string _str )
	{
		string[] splitor = { "," }  ;
		string []paramVec = _str.Split( splitor , System.StringSplitOptions.None ) ;

//		for( int j = 0 ; j < paramVec.Length ; ++j )
//		{
//			Debug.Log( paramVec[ j ] ) ;
//		}
		
		if( paramVec.Length >= 3 )
		{
			int tmpInt = 0 ;
			if( true == int.TryParse( paramVec[ 0 ] , out tmpInt ) )
			{
				ID = tmpInt ;
			}
			
			DisplayName = paramVec[ 1 ]  ;
			m_TypeStr = paramVec[ 2 ]  ;
			ParseTimeTable( paramVec[ 3 ] , ref m_TimeTable ) ;
			
			return true ;
		}
		return false ;
	}
	
	public void RemoveTimeRoute( TimeTableStruct _src )
	{
		m_TimeTable.Remove( _src ) ;
	}
	
	private bool ParseTimeTable( string _Conent , ref List< TimeTableStruct > _TimeTable )
	{
		string[] splitor = { ";" }  ;
		string[] atsplitor = { "@" }  ;
		string []timeTableVec = _Conent.Split( splitor , System.StringSplitOptions.None ) ;
		foreach( string timeTableStr in timeTableVec )
		{
			string []timeStationVec = timeTableStr.Split( atsplitor , System.StringSplitOptions.None ) ;
			if( timeStationVec.Length >= 2 )
			{
				TimeTableStruct timeTableObj = new TimeTableStruct() ;
				
				if( false == timeTableObj.Setup( timeStationVec[ 0 ] ,
									timeStationVec[ 1 ] ) )
				{
					Debug.LogError( "false == timeTableObj.Setup" + timeStationVec[ 0 ] ) ;
				}
									
				
				_TimeTable.Add( timeTableObj ) ;
			}
		}
		return true ;
	}
	
	
	public List< TimeTableStruct > m_TimeTable = new List<TimeTableStruct>() ;
	
	private int m_ID = 0 ;
	private string m_DisplayName = "(undefined)" ;
	private string m_TypeStr = "(undefined)" ;
	
}
