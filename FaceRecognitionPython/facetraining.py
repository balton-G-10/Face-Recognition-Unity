''''
Training Multiple Faces stored on a DataBase:
	==> Each face should have a unique numeric integer ID as 1, 2, 3, etc                       
	==> LBPH computed model will be saved on trainer/ directory. (if it does not exist, pls create one)
	==> for using PIL, install pillow library with "pip install pillow"
'''

import cv2
import numpy as np
from PIL import Image
import os
from database import GetValues, PushValues
import zmq


def RegisterFaces(name):
    context = zmq.Context()
    socket = context.socket(zmq.PUSH)
    socket.bind('tcp://*:5555')

    cam = cv2.VideoCapture(0)
    cam.set(3, 640)  # set video width
    cam.set(4, 480)  # set video height
    data = GetValues()
    dataLen = len(data)
    face_id = dataLen
    # For each person, enter one numeric face id
    faceName = name

    face_detector = cv2.CascadeClassifier(
        'Cascades/haarcascade_frontalface_default.xml')

    print("\n [INFO] Initializing face capture. Look the camera and wait ...")
    # Initialize individual sampling face count
    count = 0

    isRecording = False
    while cam.isOpened():

        ret, img = cam.read()
        if not ret:
            continue
        if isRecording:
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            faces = face_detector.detectMultiScale(gray, 1.3, 5)

            for (x, y, w, h) in faces:

                cv2.rectangle(img, (x, y), (x+w, y+h), (255, 0, 0), 2)
                count += 1

                # Save the captured image into the datasets folder
                cv2.imwrite("dataset/User." + str(face_id) + '.' +
                            str(count) + ".jpg", gray[y:y+h, x:x+w])
        data = {
            'bool': True,
            'int': 123,
            'str': 'Hello there!',
            'image': cv2.imencode('.jpg', img)[1].ravel().tolist()
        }
        socket.send_json(data)
        cv2.imshow("Window",img)
        k = cv2.waitKey(100) & 0xff  # Press 'ESC' for exiting video
        if k == ord("r"):
            isRecording = True
        if k == 27:
            break
        elif count >= 30:  # Take 30 face sample and stop video
            break

    # Do a bit of cleanup
    print("\n [INFO] Exiting Program and cleanup stuff")
    cam.release()
    cv2.destroyAllWindows()
    if count >= 30:
        TrainDataSet(faceName)


def TrainDataSet(faceName):

    # Path for face image database
    path = 'dataset'

    recognizer = cv2.face.LBPHFaceRecognizer_create()
    detector = cv2.CascadeClassifier(
        "Cascades/haarcascade_frontalface_default.xml")

    # function to get the images and label data

    def getImagesAndLabels(path):

        imagePaths = [os.path.join(path, f) for f in os.listdir(path)]
        faceSamples = []
        ids = []

        for imagePath in imagePaths:

            PIL_img = Image.open(imagePath).convert(
                'L')  # convert it to grayscale
            img_numpy = np.array(PIL_img, 'uint8')

            id = int(os.path.split(imagePath)[-1].split(".")[1])
            faces = detector.detectMultiScale(img_numpy)

            for (x, y, w, h) in faces:
                faceSamples.append(img_numpy[y:y+h, x:x+w])
                ids.append(id)

        return faceSamples, ids

    print("\n [INFO] Training faces. It will take a few seconds. Wait ...")
    faces, ids = getImagesAndLabels(path)
    recognizer.train(faces, np.array(ids))

    # Save the model into trainer/trainer.yml
    # recognizer.save() worked on Mac, but not on Pi
    recognizer.write('trainer/trainer.yml')
    PushValues(faceName)
    # Print the numer of faces trained and end program
    print("\n [INFO] {0} faces trained. Exiting Program".format(
        len(np.unique(ids))))
