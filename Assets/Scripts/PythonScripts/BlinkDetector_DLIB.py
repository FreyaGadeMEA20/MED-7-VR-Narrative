# Importing the required dependencies 
import cv2  # for video rendering 
import dlib  # for face and landmark detection 
import imutils 
  
# for calculating dist b/w the eye landmarks 
from scipy.spatial import distance as dist 
  
# to get the landmark ids of the left 
# and right eyes ----you can do this  
# manually too 
from imutils import face_utils 

class BlinkDetector():
    def __init__(self, landmark_predict, cam, eye_thresh):
        """
        Constructor
        :param landmark_predict: Directory to the face shape predictor file.
        :param cam: Chosen camera.
        """

        # Camera
        self.cap = cv2.VideoCapture(int(cam)) 
        self.ret, self.frame = self.cap.read() 

        # define two constants, one for the eye aspect ratio to indicate
        # blink and then a second constant for the number of consecutive
        # frames the eye must be below the threshold
        self.EYE_AR_THRESH = float(eye_thresh)
        self.EYE_AR_CONSEC_FRAMES = 2

        # initialize the frame counters and the total number of blinks
        self.COUNTER = 0
        self.TOTAL = 0

        # Eye landmarks 
        (self.L_start, self.L_end) = face_utils.FACIAL_LANDMARKS_IDXS["left_eye"] 
        (self.R_start, self.R_end) = face_utils.FACIAL_LANDMARKS_IDXS['right_eye'] 

        # Initializing the Models for Landmark and 
        # face Detection 
        print("[INFO] loading facial landmark predictor...")
        self.detector = dlib.get_frontal_face_detector() 

        #ap = argparse.ArgumentParser()
        #print("{}".format(landmark_predict))
        #ap.add_argument("-p","--shape-predictor", required=True, help="{}".format(landmark_predict))
        #ap.add_argument("-v","--video",type=str,default="",help=self.cap)

        #args = vars(ap.parse_args())
        self.landmark_predict = dlib.shape_predictor(landmark_predict) 

    def calculate_EAR(self, eye): 
        """
        :param eye: The eye information. I.e. the 6 points of the eye from the shape predictor
        """
        # calculate the vertical distances 
        # euclidean distance is basically  
        # the same when you calculate the 
        # hypotenuse in a right triangle 
        y1 = dist.euclidean(eye[1], eye[5]) 
        y2 = dist.euclidean(eye[2], eye[4]) 
    
        # calculate the horizontal distance 
        x1 = dist.euclidean(eye[0], eye[3]) 
    
        # calculate the EAR 
        EAR = (y1+y2) / (x1) 
    
        return EAR
    
    def UpdateFeed(self):
        self.ret, self.frame = self.cap.read() 
        self.frame = imutils.resize(self.frame, width=640) 

        # converting frame to gray scale to pass 
        # to detector 
        img_gray = cv2.cvtColor(self.frame, cv2.COLOR_BGR2GRAY) 
            
        # detecting the faces---# 
        faces = self.detector(img_gray, 0) 
        #print(faces)
        for face in faces: 
            # Establish the dimensions of the face
            x = face.left()
            y = face.top() #could be face.bottom() - not sure
            w = face.right() - face.left()
            h = face.bottom() - face.top()
            cv2.rectangle(self.frame, (x,y),(x+w,y+h), (200, 0, 0), 1) 

            # landmark detection 
            shape = self.landmark_predict(img_gray, face) 

            # converting the shape class directly 
            # to a list of (x,y) coordinates 
            shape = face_utils.shape_to_np(shape) 

            # parsing the landmarks list to extract 
            # lefteye and righteye landmarks--# 
            lefteye = shape[self.L_start: self.L_end] 
            righteye = shape[self.R_start: self.R_end] 

            # Calculate the EAR 
            left_EAR = self.calculate_EAR(lefteye) 
            right_EAR = self.calculate_EAR(righteye) 

            # compute the convex hull for the left and right eye, then
            # visualize each of the eyes
            leftEyeHull = cv2.convexHull(lefteye)
            rightEyeHull = cv2.convexHull(righteye)
            cv2.drawContours(self.frame, [leftEyeHull], -1, (0, 255, 0), 1)
            cv2.drawContours(self.frame, [rightEyeHull], -1, (0, 255, 0), 1)

            # Avg of left and right eye EAR 
            avg = (left_EAR+right_EAR)/2

            # check to see if the eye aspect ratio is below the blink
            # threshold, and if so, increment the blink frame counter
            if avg < self.EYE_AR_THRESH:
                self.COUNTER += 1
            # otherwise, the eye aspect ratio is not below the blink
            # threshold
            else:
                prevScore = self.TOTAL
                # if the eyes were closed for a sufficient number of
                # then increment the total number of blinks
                if self.COUNTER >= self.EYE_AR_CONSEC_FRAMES:
                    self.TOTAL += 1
                # reset the eye frame counter
                self.COUNTER = 0
                if self.TOTAL != prevScore:
                    return 1

            # draw the total number of blinks on the frame along with
            # the computed eye aspect ratio for the frame
            cv2.putText(self.frame, "Blinks: {}".format(self.TOTAL), (10, 30),
                cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
            cv2.putText(self.frame, "EAR: {:.2f}".format(avg), (300, 30),
                cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
            """if avg < self.blink_thresh: 
                self.count_frame += 1  # incrementing the frame count 
            else: 
                if self.count_frame >= self.succ_frame: 
                    cv2.putText(self.frame, 'Blink Detected', (30, 30), 
                                cv2.FONT_HERSHEY_DUPLEX, 1, (0, 200, 0), 1) 
                    print(self.count_frame, "|", self.succ_frame)
                    self.counter += 1
                    return 1
                else: 
                    self.count_frame = 0"""

        cv2.imshow("Video", self.frame) 
        if cv2.waitKey(5) & 0xFF == ord('q'): 
            print(self.COUNTER)
            return 2
        return 0

    def __del__(self):
        self.Close()

    def Close(self):
        self.cap.release()
        cv2.destroyAllWindows()