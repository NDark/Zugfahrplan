/*
@file TrainDisplay.cs
@author NDark
@date 20131012 . file created.
*/
using UnityEngine;

public class TrainDisplay 
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

	public int TargetStationID
	{
		get { return m_TargetStationID ; } 
		set { m_TargetStationID = value ; }
	}
	private int m_TargetStationID = 0 ;
	
	public GameObject Obj 
	{
		set
		{
			m_GameObject = value ;
		}
		get
		{
			return m_GameObject ;
		}
	}
	
	public void Setup( int _ID ,
					   Vector3 _Vector )
	{
		ID = _ID ;
		Position = _Vector ;
	}
	
	private int m_ID = 0 ;
	private Vector3 m_Position = Vector3.zero ;
	private GameObject m_GameObject = null ;
	
	
}
