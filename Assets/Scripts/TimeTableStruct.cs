/*
@file TimeTableStruct.cs
@author NDark
@date 20131012 . file created.
@date 20131220 
. add class method ParseTimeStr()
. add class method CopyFrom()

*/
using UnityEngine;

public class TimeTableStruct 
{
	public int Hour 
	{
		get { return m_Hour ; } 
		set { m_Hour = value ; }
	}
	
	public int Minite 
	{
		get { return m_Minite ; } 
		set { m_Minite = value ; }
	}
	
	public string Station 
	{
		get { return m_Station ; } 
		set { m_Station = value ; }
	}
	
	public void CopyFrom( TimeTableStruct _Src )
	{
		m_Hour = _Src.m_Hour ;
		m_Minite = _Src.m_Minite ;
		m_Station = _Src.m_Station ;
	}
	
	public bool Setup( string _TimeStr , string _StationStr )
	{
		bool ret = ParseTimeStr( _TimeStr ) ;
		Station = _StationStr ;
		return ret ;
	}
	
	public bool ParseTimeStr( string _TimeStr )
	{
		string[] splitor = { ":" }  ;
		string []timeVec = _TimeStr.Split( splitor , System.StringSplitOptions.None ) ;
		if( timeVec.Length >= 2 )
		{
			int.TryParse( timeVec[ 0 ] , out m_Hour ) ;
			int.TryParse( timeVec[ 1 ] , out m_Minite ) ;
			return true ;
		}
		return false ;
	}
	
	
	public bool ParseTimeStr( string _TimeStr , ref int _Hour , ref int _Minute )
	{
		string[] splitor = { ":" }  ;
		string []timeVec = _TimeStr.Split( splitor , System.StringSplitOptions.None ) ;
		if( timeVec.Length >= 2 )
		{
			int.TryParse( timeVec[ 0 ] , out _Hour ) ;
			int.TryParse( timeVec[ 1 ] , out _Minute ) ;
			return true ;
		}
		return false ;
	}	
	
	private int m_Hour = 0 ;
	private int m_Minite = 0 ;
	private string m_Station = "" ;
	
}
