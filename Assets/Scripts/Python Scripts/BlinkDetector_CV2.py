
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

        self.face_cascade = cv2.CascadeClassifier(face_cascade)
        self.eye_cascade = cv2.CascadeClassifier(eye_cascade)

        self.cap = cv2.VideoCapture(cam)
        ret,img = self.cap.read()
 
        #Variable store execution state
        first_read = True
        has_blinked = False
        counter=0
 
    def __del__(self):
        self.Close()

    def Close(self):
        self.cap.release()
        cv2.destroyAllWindows()

    def StartFeed(self):
        while(ret):
            ret,img = self.cap.read()
            #Converting the recorded image to grayscale
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            #Applying filter to remove impurities
            gray = cv2.bilateralFilter(gray,5,1,1)
        
            #Detecting the face for region of image to be fed to eye classifier
            faces = self.face_cascade.detectMultiScale(gray, 1.1, 5,minSize=(100,100))
            if(len(faces)>0):
                for (x,y,w,h) in faces:
                    img = cv2.rectangle(img,(x,y),(x+w,y+h),(0,255,0),2)
        
                    #roi_face is face which is input to eye classifier
                    roi_face = gray[y:y+h,x:x+w]
                    roi_face_clr = img[y:y+h,x:x+w]
                    eyes = self.eye_cascade.detectMultiScale(roi_face,1.1,5,minSize=(20,20))
        
                    #Examining the length of eyes object for eyes
                    if len(eyes)>=2:
                        #Check if program is running for detection 
                        if(first_read and not has_blinked):
                            first_read = False
                        else:
                            cv2.putText(img, 
                            "Eyes open!", (70,70), 
                            cv2.FONT_HERSHEY_PLAIN, 2,
                            (255,255,255),2)
                            has_blinked = False
                    else:
                        if first_read:
                            #To ensure if the eyes are present before starting
                            cv2.putText(img, 
                            "No eyes detected", (70,70),
                            cv2.FONT_HERSHEY_PLAIN, 3,
                            (0,0,255),2)
                            #print("no eyes detected-----")
                        elif not first_read and len(eyes)==0 :
                            #This will print on console and restart the algorithm
                            print("Blink detected--------------")
                            #cv2.waitKey(3000)
                            first_read = True
                            has_blinked=True
                            counter += 1
                    
            else:
                cv2.putText(img,
                "No face detected",(100,100),
                cv2.FONT_HERSHEY_PLAIN, 3, 
                (0,255,0),2)
        
            #Controlling the algorithm with keys
            cv2.imshow('img',img)
            a = cv2.waitKey(1)
            if(a==ord('q')):
                print(counter)
                break
            elif(a==ord('s') and first_read):
                #This will start the detection
                first_read = False
