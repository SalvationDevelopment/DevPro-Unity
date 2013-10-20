using UnityEngine;
using System.Collections;

public class MessageBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
	GUI.Box(new Rect(0,0,100,100),"Test");
		
	}
}
