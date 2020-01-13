using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GazeController : MonoBehaviour
{
    private RaycastHit hit;
    private float gaze_count;
    [SerializeField] private GameObject eye_ring;

    [SerializeField] private GameObject capture_button;
    
    private Material capture_button_material;

    private bool capture_able=true;

    void Start(){
        capture_button_material=capture_button.GetComponent<Renderer>().material;
    }
    public void Restart(){
        capture_able=true;
        capture_button_material.color=Color.white;
    }
    // Update is called once per frame
    void Update()
    {
        if(!capture_able){
            return;
        }
        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit,Mathf.Infinity)){
            if(hit.collider.gameObject==capture_button){
                eye_ring.SetActive(true);
                capture_button_material.color=Color.green;
                gaze_count+=Time.deltaTime;
                if(gaze_count>2f){
                    capture_button_material.color=Color.yellow;
                    //撮影待機
                    capture_able=false;
                    eye_ring.SetActive(false);
                    StartCoroutine(WaitCapture());
                }
            }
        }else{
            gaze_count=0;
            capture_button_material.color=Color.white;
            eye_ring.SetActive(false);
        }
    }
    IEnumerator WaitCapture(){
        Debug.Log("Wait now");
        yield return new WaitForSeconds(2f);
        Debug.Log("Wait Finish");
        GetComponent<SmartCameraController>().CaptureStart();
    }
}
