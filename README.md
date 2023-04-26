# API Documentation

This API allows users to obtain JWT signed with certificate

## Endpoints

###### POST /api/Auth/GetToken

###### POST /api/Auth/AddNewUser 


## Add API user

##### POST /api/Auth/AddNewUser Only for users with Admin Role

###### Adds a new API user to the database. The request must include a JSON object in the body with the following fields:  
```json
{
  "login": "string",
  "password": "string"
}
```

## User Login

##### POST /api/Auth/GetToken

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
  "role": "Admin"
}
```

