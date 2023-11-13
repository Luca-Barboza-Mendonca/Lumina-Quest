import socket
import json
import threading
from urllib.parse import urlparse


def handle_upload(request_data, client_socket):

    json_data = json.loads(request_data.decode('utf-8'))

    print("Received JSON data from Upload service:")
    print(json.dumps(json_data, indent=2))

    response = "HTTP/1.1 200 OK\r\nContent-Length: 0\r\n\r\n"
    client_socket.sendall(response.encode('utf-8'))

def handle_client(client_socket):

    data = client_socket.recv(1024)

    request_str = data.decode('utf-8')
    if "POST /Upload" in request_str:

        _, body = request_str.split("\r\n\r\n", 1)

        handle_upload(body.encode('utf-8'), client_socket)
    
    client_socket.close()

HOST = "localhost"  # Standard loopback interface address (localhost)
PORT = 65432  # Port to listen on (non-privileged ports are > 1023)

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen(5)
    while True:
        client_socket, client_address = s.accept()
        with client_socket:
            print(f"Connected by {client_address}")
            threading.Thread(target=handle_client, args=(client_socket,)).start()