/*
@file StationData.cs
@author NDark
@date 20130922 file created.
@date 20131208 by NDark . add class method CopyFrom()

*/
using UnityEngine;

public class StationData 
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
	
	public Vector3 Position 
	{
		set
		{
			m_Position = value ;
		}
		get
		{
			return m_Position ;
		}
	}		
	
	public void CopyFrom( StationData _Src )
	{
		Setup( _Src.ID , 
			   _Src.DisplayName ,
			   _Src.Position ) ;
	}
	
	public void Setup( int _ID , 
					   string _DisplayName , 
					   Vector3 _Position )
	{
		ID = _ID ;
		DisplayName = _DisplayName ;
		Position = _Position ;
	}
	
	public void SetupPos( float _X , float _Y )
	{
		Position = new Vector3( _X , _Y , Position.z ) ;
	}	
	
	public string CreateString()
	{
		return ID.ToString() + "," + DisplayName + "," + Position.x + "," + Position.y ;
	}
	
	public bool ParseFromString( string _str )
	{
		string[] splitor = { "," }  ;
		string []paramVec = _str.Split( splitor , System.StringSplitOptions.None ) ;

//		for( int j = 0 ; j < paramVec.Length ; ++j )
//		{
//			Debug.Log( paramVec[ j ] ) ;
//		}
		
		if( paramVec.Length >= 4 )
		{
			int tmpInt = 0 ;
			if( true == int.TryParse( paramVec[ 0 ] , out tmpInt ) )
			{
				ID = tmpInt ;
			}
			
			DisplayName = paramVec[ 1 ]  ;
			
			float x = 0 ;
			float y = 0 ;
			if( true == float.TryParse( paramVec[ 2 ] , out x ) &&
				true == float.TryParse( paramVec[ 3 ] , out y ))
			{
				SetupPos( x , y ) ;
			}
			
			return true ;
		}
		return false ;
	}
	
	
	private int m_ID = 0 ;
	private string m_DisplayName = "(undefined)" ;
	private Vector3 m_Position = Vector3.zero ;
}
