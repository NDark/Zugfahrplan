/*
@file TimeTableStruct.cs
@author NDark
@date 20131012 . file created.
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
	
	public bool Setup( string _TimeStr , string _StationStr )
	{
		ParseTimeStr( _TimeStr ) ;
		Station = _StationStr ;
		return true ;
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
	
	private int m_Hour = 0 ;
	private int m_Minite = 0 ;
	private string m_Station = "" ;
	
}
