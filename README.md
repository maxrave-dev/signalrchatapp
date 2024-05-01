# SignalR Chat App

This is a backend of chat app inspired by Slack using .Net 8 ASP.NET MVC and SignalR to provide RESTful API and websocket real-time communication.

## Author
- Main Developer: [Nguyen Duc Tuan Minh (maxrave-dev)](https://github.com/maxrave-dev)
- Group 2 (Software Oriented Architecture - UEH)

## Features
- User authentication
- Real-time chat
- Create, join, leave, delete channels
- Send messages in channels
- Upload files

## Technologies
- .Net 8
- ASP.NET MVC
- SignalR
- MS SQL Server
- Entity Framework Core
- Identity Framework
- JWT

## Authorization
- This app uses JWT for authorization. To access RestfulAPI, you need to pass Bearer token to the `Authorization` header of request and `access_token=[token]` to the parameter of request to access SignalR hub. 
- To get the token, you need to login to the app and get the token from the response. To more detail about login, check below.

## API Reference

- Root Rest API URL: `'host/api'`

### Account
 - Login:
   + Get Bearer Token from username and password
   + Method: POST
    ```http
   POST /account/login
   Body:
      {
          "UserName": "string",
          "Password": "string"
      }
   ````
   + Response:
   ```json
   {
      "username": "maxrave",
      "token": "abcxyz",
      "expiresIn": "2022-12-31T23:59:59.999Z"
   }
   ```
   + If error:
   ```json
   {
      "message": "Invalid credentials",
      "errors": true
   }
   ```
 - Register:
   + Register new user
   + Method: POST
   ```http
   POST /account/register
   Body:
      {
       "FullName": string
       "UserName": string
       "Email": string
       "Password": string
      }
   ```
   + Response:
    ```json
    {
       "message": "User created successfully!",
       "errors": false
    }
    ```
- Logout:
   + Log out and remove token from backend
   + Method: POST
  ```http
  POST /account/logout
  ```
   + Response:
   ```
    STATUS 200 OK
   ```
- User:
    + Get user info
    + Method: GET
    ```http
    GET /account/user
   ```
    + Response:
    ```json
    {
      "fullName": "string",
      "userName": "string",
      "avatar": "string",
      "rooms": [],
      "messages": [] 
   }
    ```
### Rooms
- Get Rooms:
    + Get all rooms
    + Method: GET
    ```http
    GET /rooms
    ```
    + Response:
    ```json
    [
      {"id":1,"name":"DEPLOYED ROOM","admin":"maxrave"},
      {"id":2,"name":"ROOM 1","admin":"maxrave"},
      {"id":3,"name":"ROOM 2","admin":"maxrave"}
    ]
    ```
- Create Room:
    + Create new room
    + Method: POST
    ```http
    POST /rooms
    Body:
    {
      "name": "string"
    }
    ```
    + Response:
    ```json
    {
      "id": 1,
      "name": "string",
      "admin": "string"
    }
    ```
  
- Remove Room:
    + Remove room (only admin of this room can remove)
    + Method: DELETE
    ```http
    DELETE /rooms/{roomId}
    ```
    + Response:
    ```
    STATUS 200 OK
    ```
  
### Messages
- Get Messages:
    + Get all messages in room
    + Method: GET
    ```http
    GET /Messages/Room/${roomName}
    ```
    + Response:
    ```json
    [
        {
            "id": 1,
            "content": "xin zu m oi",
            "timestamp": "2024-04-30T16:07:48.8828629",
            "fromUserName": "maxrave",
            "fromFullName": "Nguyen Duc Tuan Minh",
            "room": "DEPLOYED ROOM",
            "avatar": null
        },
        {
            "id": 2,
            "content": "dung dc on dung ko",
            "timestamp": "2024-04-30T22:22:16.0819198",
            "fromUserName": "maxrave",
            "fromFullName": "Nguyen Duc Tuan Minh",
            "room": "DEPLOYED ROOM",
            "avatar": null
        },
        {
            "id": 3,
            "content": "hi",
            "timestamp": "2024-05-01T12:54:38.4343108",
            "fromUserName": "maxrave",
            "fromFullName": "Nguyen Duc Tuan Minh",
            "room": "DEPLOYED ROOM",
            "avatar": null
        }
    ]
    ```
- Send Message:
    + Send message to room
    + Method: POST
    ```http
    POST /Messages/
    Body:
    {
      "RoomName": string;
      "Content": string;
    }
    ```
    + Response:
    ```json
    {
      "id": 4,
      "content": "alo",
      "timestamp": "2024-05-01T12:57:09.7532232+07:00",
      "fromUserName": "maxrave",
      "fromFullName": "Nguyen Duc Tuan Minh",
      "room": "DEPLOYED ROOM",
      "avatar": null
    }
    ```
- Upload File:
    + Upload file to room
    + Must add `"Content-Type" : "multipart/form-data"` to header
    + Type of file: `.jpg,.jpeg,.png`, size: `<= 1048576 bytes`
    + Method: POST
    ```http
    POST /upload
    Body:
    {
      RoomId: number;
      BackendHost: string; //This host will be used to get file content
      File: File;
    }
    ```
    + Response:
    ```
    STATUS 200 OK
    ```
- Delete Message:
    + Delete message (only sender of this message can delete)
    + Method: DELETE
    ```http
    DELETE /Messages/{messageId}
    ```
    + Response:
    ```
    STATUS 200 OK
    ```
  
- Websocket:
    + Connect to SignalR hub using `access_token=[token]` as query parameter
    + Recommend using [SignalR client](https://www.npmjs.com/package/@microsoft/signalr) to connect to hub.
    + Hub URL: `host/chatHub`
    + Methods:
      + `Join', roomName: string` - Join room
      + `onRoomDeleted` - Receive message when room is deleted
      + `newMessage`, return `message: Message` - Receive new message

## Installation
- Clone this repository
- Update `appsettings.json` with your connection string
- Run `dotnet ef database update` to create database
- Run `dotnet run` to start the app