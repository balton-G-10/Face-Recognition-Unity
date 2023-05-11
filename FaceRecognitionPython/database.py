
import firebase_admin
from firebase_admin import credentials
from firebase_admin import db

cred = credentials.Certificate("serviceAccountKey.json")
firebase_admin.initialize_app(cred,{
    "databaseURL": "https://facerecognition-e5a1f-default-rtdb.firebaseio.com/",
})
ref = db.reference('Faces')

def UpdateValues(data):
    for key,value in data.items():
        ref.child(key).set(value)

def GetValues():
    lst = ref.order_by_key().get()
    return lst

def PushValues(value):
    data = {}
    lst = ref.order_by_key().get()
    for values in range(0, len(lst)):
        data[str(values)] = str(lst[values])
    newKey = len(lst)
    data[str(newKey)] = str(value)
    UpdateValues(data)
