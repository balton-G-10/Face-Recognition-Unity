''''
Real Time Face Recogition
	==> Each face stored on dataset/ dir, should have a unique numeric integer ID as 1, 2, 3, etc                       
	==> LBPH computed model (trained faces) should be on trainer/ dir

'''

import cv2
import numpy as np
import os
import socket
import zmq
# from facetraining import RegisterFaces
from database import GetValues, PushValues

font = cv2.FONT_HERSHEY_SIMPLEX

# iniciate id counter
id = 0

# names related to ids: example ==> Marcelo: id=1,  etc
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)


def SendValue(data, port=8000):
    serverAddressPort = ("127.0.0.1", port)
    sock.sendto(str.encode(str(data)), serverAddressPort)


def FaceRecognition():
    names = GetValues()
    recognizer = cv2.face.LBPHFaceRecognizer_create()
    recognizer.read('trainer/trainer.yml')
    cascadePath = "Cascades/haarcascade_frontalface_default.xml"
    faceCascade = cv2.CascadeClassifier(cascadePath)
    context = zmq.Context()
    socket = context.socket(zmq.PUSH)
    socket.bind('tcp://*:5555')

    # Initialize and start realtime video capture
    cam = cv2.VideoCapture(0)
    cam.set(3, 640)  # set video widht
    cam.set(4, 480)  # set video height

    # Define min window size to be recognized as a face
    minW = 0.1*cam.get(3)
    minH = 0.1*cam.get(4)
    count = 0
    while cam.isOpened():
        ret, img = cam.read()
        if not ret:
            continue

        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

        faces = faceCascade.detectMultiScale(
            gray,
            scaleFactor=1.2,
            minNeighbors=5,
            minSize=(int(minW), int(minH)),
        )

        for (x, y, w, h) in faces:

            cv2.rectangle(img, (x, y), (x+w, y+h), (0, 255, 0), 2)

            id, confidence = recognizer.predict(gray[y:y+h, x:x+w])

            # Check if confidence is less them 100 ==> "0" is perfect match
            if (confidence < 70):
                id = names[id]
                SendValue(id)
            else:
                id = "unknown"
            count+=1
            cv2.putText(img, str(id), (x+5, y-5), font, 1, (255, 255, 255), 2)
        data = {
            'bool': True,
            'int': 123,
            'str': 'Hello there!',
            'image': cv2.imencode('.jpg', img)[1].ravel().tolist()
        }
        socket.send_json(data)

        k = cv2.waitKey(10) & 0xff  # Press 'ESC' for exiting video
        if k == 27:
            break
        if count > 80:
            print("Frame Break")
            break

    # Do a bit of cleanup
    print("\n [INFO] Exiting Program and cleanup stuff")
    cam.release()
    cv2.destroyAllWindows()
