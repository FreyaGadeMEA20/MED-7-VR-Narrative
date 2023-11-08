
#All the imports go here

 
#Initializing the face and eye cascade classifiers from xml files
# OPENCV PATH SYSTEM IS REALLY BAD, SO YOU NEED TO GIVE IT THE DIRECT PATH FOR NOW
# BIG ISSUE THAT WOULD NEED TO BE FIXED LATER
import numpy as np
import cv2

class BlinkDetector():
    def __init__(self,face_cascade, eye_cascade, cam):
        """
        Constructor
        :param face_cascade: Directory to the face cascade file
        :param eye_cascade: Directory to the eye cascade file
        :param cam: Chosen camera
        """

        self.face_cascade = cv2.CascadeClassifier(str(face_cascade))
        self.eye_cascade = cv2.CascadeClassifier(str(eye_cascade))

        self.cap = cv2.VideoCapture(int(cam))
        self.ret, self.img = self.cap.read()

        #Variable store execution state
        self.first_read = True
        self.has_blinked = False
        self.counter=0
        print("Blink initialised")
 
    def __del__(self):
        self.Close()

    def Close(self):
        self.cap.release()
        cv2.destroyAllWindows()

    def StartFeed(self):
        self.ret, self.img = self.cap.read()
        #Converting the recorded image to grayscale
        gray = cv2.cvtColor(self.img, cv2.COLOR_BGR2GRAY)
        #Applying filter to remove impurities
        gray = cv2.bilateralFilter(gray,5,1,1)
    
        #Detecting the face for region of image to be fed to eye classifier
        faces = self.face_cascade.detectMultiScale(gray, 1.1, 5,minSize=(100,100))
        if(len(faces)>0):
            for (x,y,w,h) in faces:
                self.img = cv2.rectangle(self.img,(x,y),(x+w,y+h),(0,255,0),2)
    
                #roi_face is face which is input to eye classifier
                roi_face = gray[y:y+h,x:x+w]
                roi_face_clr = self.img[y:y+h,x:x+w]
                eyes = self.eye_cascade.detectMultiScale(roi_face,1.1,5,minSize=(20,20))
    
                #Examining the length of eyes object for eyes
                if len(eyes)>=2:
                    #Check if program is running for detection 
                    if(self.first_read and not self.has_blinked):
                        self.first_read = False
                    else:
                        cv2.putText(self.img, 
                        "Eyes open!", (70,70), 
                        cv2.FONT_HERSHEY_PLAIN, 2,
                        (255,255,255),2)
                        self.has_blinked = False
                else:
                    if self.first_read:
                        #To ensure if the eyes are present before starting
                        cv2.putText(self.img, 
                        "No eyes detected", (70,70),
                        cv2.FONT_HERSHEY_PLAIN, 3,
                        (0,0,255),2)
                        #print("no eyes detected-----")
                    elif not self.first_read and len(eyes)==0 :
                        #This will print on console and restart the algorithm
                        print("Blink detected--------------")
                        #cv2.waitKey(3000)
                        self.first_read = True
                        self.has_blinked=True
                        self.counter += 1
                        return 1
                
        else:
            cv2.putText(self.img,
            "No face detected",(100,100),
            cv2.FONT_HERSHEY_PLAIN, 3, 
            (0,255,0),2)
    
        #Controlling the algorithm with keys
        cv2.imshow('img',self.img)
        a = cv2.waitKey(1)
        if(a==ord('q')):
            print(self.counter)
            return 2
        elif(a==ord('s') and self.first_read):
            #This will start the detection
            self.first_read = False
        return 0
