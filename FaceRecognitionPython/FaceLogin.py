import socket
from FaceRecognition import FaceRecognition
from facetraining import RegisterFaces
# create a UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# bind the socket to a specific IP address and port number
server_address = ('127.0.0.1', 8001)
sock.bind(server_address)
while True:
    # receive data from the socket
    try:	
        data, address = sock.recvfrom(4096)
        option = data.decode('utf-8')
        if option[0] == "0":
            print("Login")   
            FaceRecognition()
        if option[0] == "1":
            print("SignUp")
            RegisterFaces(option[2:])
    except Exception as e:
        print(e)
        break
