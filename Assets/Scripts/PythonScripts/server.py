# Two-way communication between Python3 and Unity to send and receive strings, created by Youssef Elashry
# Adapted by the owners of the project to include eye tracking

# Example of a Python UDP server

# Libraries for handling blinking and server
import UdpComms as U
import BlinkDetector_DLIB as dlibBlink
import time

# Libraries for handling data from dotenv files
import os
from dotenv import load_dotenv

# loads the local .env file.
#  - Make sure it is in the same folder as the script
load_dotenv()

# Get the data from the .env file
#  - done like this so we can keep important information offline from github
#  - such as local directory names, as opencv cannot handle relative paths
LANDMARK_PREDICT = os.getenv('LANDMARK_PREDICT')
WEBCAM = os.getenv('WEBCAM')
EYE_THRESH = os.getenv('EYE_THRESH')

# Create UDP socket to use for sending (and receiving) data.
#  - udpIP, portTX, portRX has to be the same for Unity and Python
sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)
    
# Variables for keeping track of blinks and how much time has passed
blinkCount = 0
start = time.time()

# Initializing the blink detector
print("Initializing blink")
eyeT_dlib = dlibBlink.BlinkDetector(LANDMARK_PREDICT, WEBCAM, EYE_THRESH)

# Keeps looping until it gets a break command
while True:
    # Updates the eye tracker feed
    eyeStatus_dlib = eyeT_dlib.UpdateFeed()

    # Gets the status from the eye tracker and updates the blink count
    if eyeStatus_dlib == 1:
        blinkCount +=1

    # Gets the status to what it needs to send to the other UDP socket
    #  - 1 = blink
    #  - 2 = close the program
    if eyeStatus_dlib == 1:
        sock.SendData(str(1))                       # Send this string to other application
        print("Blink detected. Sending to server")  # Sends status to python console
        time.sleep(1)                               # creates a short delay before sending a signal to unity to reset
        sock.SendData(str(0))                       # sends signal to reset
    elif eyeStatus_dlib == 2:
        sock.SendData(str(2))                       # Sends signal to unity to disconnect the server
        print("Python Closed. Server disconnected") # Sends status to python console

    data = sock.ReadReceivedData() # read data

    #if data != None: # if NEW data has been received since last ReadReceivedData function call
    #    print(data) # print new received data

    # Closing the window and breaking the loop
    if eyeStatus_dlib == 2:
        eyeT_dlib.Close()                           # closes the eye tracker
        print(blinkCount, "blinks tracked")         # prints the amount of blinks while the tracker has been open
        end = time.time()                           # gets the final time
        print(end-start, "second elapsed")          # prints time elapsed in seconds
        break                                       # breaks the while loop
    
    # to prevent reading from the server too many times, a small sleep is inserted.
    time.sleep(.01)