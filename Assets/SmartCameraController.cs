using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SmartCameraController : MonoBehaviour
{
    private int Width = 660;
    private int Height = 440;
    private int FPS = 30;
    private WebCamTexture webcamTexture;
    private Color32[] color32;

	public string url = "https://61f6fe29.ngrok.io/";
    public class Item
	{
		public string data;
	}

    void Start () {
        WebCamDevice[] devices = WebCamTexture.devices;
        webcamTexture = new WebCamTexture(devices[0].name, Width, Height, FPS );
        webcamTexture.Play();

    }
    public void CaptureStart(){
        GameObject target=GameObject.Find("Slide");
        if(target!=null){
            Debug.Log("Find Target!");
            Debug.Log("Capture now...");
            color32 =webcamTexture.GetPixels32();
            Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
			// target.GetComponent<Renderer>().material.mainTexture = texture;
			texture.SetPixels32(color32);

			StartCoroutine(ImageTransform(target, texture, webcamTexture.width, webcamTexture.height));

        }
    }

    public IEnumerator ImageTransform(GameObject target, Texture2D texture, int width, int height)
	{
		byte[] bytes = texture.EncodeToJPG();
		WWWForm form = new WWWForm();
		form.AddBinaryData("sample.jpg", bytes, "sample.jpg", "image/jpeg");

		var res = UnityWebRequest.Post(url + "upload", form);

        yield return res.SendWebRequest();

		Debug.Log("code : " + res.responseCode);
        if (res.isNetworkError || res.isHttpError)
		{
			Debug.Log("failure");
		}
        else
		{
			Debug.Log("success");
		}

		Item item = JsonUtility.FromJson<Item>(res.downloadHandler.text);

		var image_res = UnityWebRequestTexture.GetTexture(url + "upload/" + item.data);
		yield return image_res.SendWebRequest();

		Texture2D trans_texture = new Texture2D(width, height);
		trans_texture.LoadImage(image_res.downloadHandler.data);

		target.GetComponent<Renderer>().material.mainTexture = trans_texture;

		// trans_texture.SetPixels32(color32);
		// trans_texture.Apply();
		Debug.Log("Success!");
		webcamTexture.Stop();
		GetComponent<GazeController>().Restart();
		webcamTexture.Play();
	}
}
