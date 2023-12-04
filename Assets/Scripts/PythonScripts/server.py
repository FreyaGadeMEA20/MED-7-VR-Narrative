# Created by Youssef Elashry to allow two-way communication between Python3 and Unity to send and receive strings

# Feel free to use this in your individual or commercial projects BUT make sure to reference me as: Two-way communication between Python 3 and Unity (C#) - Y. T. Elashry
# It would be appreciated if you send me how you have used this in your projects (e.g. Machine Learning) at youssef.elashry@gmail.com

# Use at your own risk
# Use under the Apache License 2.0

# Example of a Python UDP server

import UdpComms as U
import BlinkDetector_CV2 as blink
import BlinkDetector_DLIB as dlibBlink
import time

import os
from dotenv import load_dotenv

load_dotenv()
FACE_PATH = os.getenv('FACE_PATH')
EYE_PATH = os.getenv('EYE_PATH')
WEBCAM = os.getenv('WEBCAM')
LANDMARK_PREDICT = os.getenv('LANDMARK_PREDICT')

# Create UDP socket to use for sending (and receiving)
sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)

i = 0

countDLIB = 0

print("Initializing blink")
eyeT_dlib = dlibBlink.BlinkDetector(LANDMARK_PREDICT, WEBCAM)
while True:
    eyeStatus_dlib = eyeT_dlib.StartFeed()

    if eyeStatus_dlib == 1:
        if eyeStatus_dlib == 1:
            print(eyeStatus_dlib,"dlib")
            countDLIB +=1

    
    if eyeStatus_dlib == 1:
        #sock.SendData(str(1)) # Send this string to other application
        print("Blink detected. Sending to server")
        time.sleep(1)
        #sock.SendData(str(0))
    elif eyeStatus_dlib == 2:
        #sock.SendData(str(2))
        print("Python Closed. Server disconnected")
    #i += 1

    #data = sock.ReadReceivedData() # read data

    #if data != None: # if NEW data has been received since last ReadReceivedData function call
    #    print(data) # print new received data

    if eyeStatus_dlib == 2:
        eyeT_dlib.Close()
        print(countDLIB, "dlib blinks tracked")
        break

    time.sleep(.01)