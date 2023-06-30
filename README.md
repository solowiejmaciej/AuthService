# API Documentation

This API allows users to obtain JWT signed with certificate

## Endpoints

###### POST /api/Auth/Login

###### POST /api/Auth/Login/QR

###### POST /api/User/ 
###### DELETE /api/User/{id} 
###### GET /api/User/
###### GET /api/User/{id} 


## Add API user

##### POST /api/User Only for users with Admin Role

###### Adds a new API user to the database. The request must include a JSON object in the body with the following fields:  
```json
{
  "login": "string",
  "password": "string"
}
```

## User Login

##### POST /api/Auth/Login

###### Allows an API user to obtain a JWT token for API usage. The request must include a JSON object in the body with the following fields:

```json
{
  "login": "string",
  "password": "string"
}
```
## Response
###### If the request is successful, the API returns a JSON object with a JWT token:

```json
{
  "token": "string",
  "statusCode": 200,
  "issuedDate": "2023-04-25T18:41:26.7311955Z",
  "expiresAt": "2024-04-24T18:41:26.6371061Z",
  "userId": "903d0cb2-cddf-4c26-acf8-83eb36abc4b3",
  "role": "Admin"
}
```

## Delete User

##### DELETE /api/User/{id} 
###### Allows an API user to delete user with provided Id

## Get User

##### GET /api/User/
###### Returns all of the users

## Response
```json
[
  {
    "id": 0,
    "login": "string",
    "createdAt": "2023-04-26T22:26:12.788Z",
    "roleId": 0
  },
    {
    "id": 1,
    "login": "string",
    "createdAt": "2023-04-26T22:26:12.788Z",
    "roleId": 0
  }
]
```

## Get User by Id

##### GET /api/User/{id} 
###### Allows an API user to get specific user with provided Id
## Response
```json
{
  "id": 0,
  "login": "string",
  "createdAt": "2023-04-26T22:27:35.893Z",
  "roleId": 0
}
```

