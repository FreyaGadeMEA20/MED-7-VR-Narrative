using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System;

public class PythonTest : MonoBehaviour
{
    //[SerializeField] TextMeshProUGUI pythonRcvdText = null;
    //[SerializeField] TextMeshProUGUI sendToPythonText = null;

    string tempStr = "Sent from Python xxxx";
    int numToSendToPython = 0;
    UdpSocket udpSocket;

    public Timeline_Change timelineController;

    public void QuitApp()
    {
        print("Quitting");
        Application.Quit();
    }

    public void UpdatePythonRcvdText(string str)
    {
        tempStr = str;
        print("Python RCVD Text : " + tempStr);
    }

    public void SendToPython()
    {
        udpSocket.SendData("Sent From Unity: " + numToSendToPython.ToString());
        numToSendToPython++;
        //sendToPythonText.text = "Send Number: " + numToSendToPython.ToString();
        print("Send Number: " + numToSendToPython.ToString());
    }

    private void Start()
    {
        if (timelineController == null){
            print("NO TIMELINE CONTROLLER");
            Application.Quit();
        }
        udpSocket = FindObjectOfType<UdpSocket>();
        //sendToPythonText.text = "Send Number: " + numToSendToPython.ToString();
        print("Send Number: " + numToSendToPython.ToString());
    }

    void Update()
    {
        //pythonRcvdText.text = tempStr;
        try
        {
            int result = int.Parse(tempStr);
            if(result == 1){
                //print(result);

                timelineController.SwitchTimeline();
            }
            Console.WriteLine(result);
        }
        catch (FormatException)
        {
            Console.WriteLine($"Unable to parse '{tempStr}'");
        }
    }
}
