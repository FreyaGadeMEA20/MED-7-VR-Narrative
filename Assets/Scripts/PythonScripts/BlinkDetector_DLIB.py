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
    def __init__(self, landmark_predict, cam):
        """
        Constructor
        :param landmark_predict: Directory to the face shape predictor file.
        :param cam: Chosen camera.
        """

        # Camera
        self.cap = cv2.VideoCapture(int(cam)) 
        self.ret, self.frame = self.cap.read() 
        print(self.frame)

        # Variables 
        self.blink_thresh = 0.45
        self.succ_frame = 2
        self.count_frame = 0

        # Eye landmarks 
        (self.L_start, self.L_end) = face_utils.FACIAL_LANDMARKS_IDXS["left_eye"] 
        (self.R_start, self.R_end) = face_utils.FACIAL_LANDMARKS_IDXS['right_eye'] 

        # Initializing the Models for Landmark and 
        # face Detection 
        self.detector = dlib.get_frontal_face_detector() 
        self.landmark_predict = dlib.shape_predictor(landmark_predict) 

        # Count check
        self.counter=0


    def calculate_EAR(self, eye): 
        """
        :param eye: The eye information.
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
        EAR = (y1+y2) / x1 
    
        return EAR
    
    def StartFeed(self):
        self.ret, self.frame = self.cap.read() 
        self.frame = imutils.resize(self.frame, width=640) 

        # converting frame to gray scale to pass 
        # to detector 
        img_gray = cv2.cvtColor(self.frame, cv2.COLOR_BGR2GRAY) 
            
        # detecting the faces---# 
        faces = self.detector(img_gray) 
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

            # Avg of left and right eye EAR 
            avg = (left_EAR+right_EAR)/2
            if avg < self.blink_thresh: 
                self.count_frame += 1  # incrementing the frame count 
            else: 
                if self.count_frame >= self.succ_frame: 
                    cv2.putText(self.frame, 'Blink Detected', (30, 30), 
                                cv2.FONT_HERSHEY_DUPLEX, 1, (0, 200, 0), 1) 
                    print(self.count_frame, "|", self.succ_frame)
                    self.counter += 1
                    return 1
                else: 
                    count_frame = 0

        cv2.imshow("Video", self.frame) 
        if cv2.waitKey(5) & 0xFF == ord('q'): 
            print(self.counter)
            return 2
        return 0

    def __del__(self):
        self.Close()

    def Close(self):
        self.cap.release()
        cv2.destroyAllWindows()