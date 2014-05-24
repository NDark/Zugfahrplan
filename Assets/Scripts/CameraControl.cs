/*
@file CameraControl.cs
@author NDark
@date 20140501 file started.

*/
using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour 
{
	public GameObject trackingObject = null ;
	public bool m_MouseDown = false ;
	public Vector3 m_MousePositionLast =Vector3.zero ;
	public void SetupTrackingObject( GameObject _Set )
	{
		trackingObject = _Set ;
	}

	// Use this for initialization
	void Start () 
	{
		m_MousePositionLast = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float scrollWheel = Input.GetAxis ("Mouse ScrollWheel");
		if (0 != scrollWheel ) 
		{
			Camera.main.orthographicSize +=	(scrollWheel * 200);
		}

		if (null == trackingObject) 
		{
			if( true == Input.GetMouseButtonDown( 0 ) )
			{
				m_MousePositionLast = Input.mousePosition;
			}
			else
			{
				m_MouseDown = Input.GetMouseButton( 0 ) ;
				if( true == m_MouseDown )
				{
					Vector3 vecDiff = Input.mousePosition - m_MousePositionLast ;
					Camera.main.transform.Translate( -1  * vecDiff ) ;
					m_MousePositionLast = Input.mousePosition ;
				}

			}
			
		}
		else
		{
			Camera.main.transform.position = new Vector3( 
				trackingObject.transform.position.x ,
				trackingObject.transform.position.y ,
				 Camera.main.transform.position.z ) ;
			                                             
		}
			


	}
}
