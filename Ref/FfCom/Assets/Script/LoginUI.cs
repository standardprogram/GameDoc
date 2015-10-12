using UnityEngine;
using System.Collections;
using LitJson;

public class LoginUI : MonoBehaviour {

	public UIInput username;
	public UIInput password;

	void Awake ()
	{
		//获取需要监听的按钮对象
		GameObject button = GameObject.Find("Camera/Anchor/Panel/Submit");
		//设置这个按钮的监听，指向本类的ButtonClick方法中。
		UIEventListener.Get(button).onClick = ButtonClick;
	}

	//计算按钮的点击事件
	void ButtonClick(GameObject button)
	{
		JsonData data = new JsonData ();
		data ["name"] = username.text;
		data ["password"] = password.text;
		byte[] msg = System.Text.UTF8Encoding.UTF8.GetBytes (data.ToJson ());
		NetMgr.Instance.SendMsg (msg, 1);
	}

	
	void Start ()   
	{     

	}
	
	void OnGUI()  
	{  

	}  
}
