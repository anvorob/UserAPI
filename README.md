# UserAPI
Celo test API

To simplify this project, json (json1.json) file is used to store user records.

# API endpoints
[Get] /v1/GetUserItems?limit=5&page=1 - return list of users.

[Get] /v1/SearchUsers?searchWord=al&searchField=FirstName - search for user record based on set word and field

[Delete] /v1/DeleteUser?email=Amin.Chang@gmail.com - delete user record by email.

[Post] /v1/AddUserItem - feed user object to create new record. User email must be unique.
<br><b>Example:</b> 
```
{"Email":"Amin.Chang@gmail.com","FirstName":"Amin","LastName":"Chang"}
```

[Post] /v1/AddUserItems - endoint allows to add multiple users in one call.
<br><b>Example:</b> 
```
{"UserList":[{"Email":"Amin.Chang@gmail.com","FirstName":"Amin","LastName":"Chang"},{"Email":"Chanelle.Kaiser@gmail.com","FirstName":"Chanelle","LastName":"Kaiser"}]}
```
[Post] /v1/UpdateUser - this endpoint allows to update user record. Email must be specified, to update user details.
<br><b>Example:</b>
```
{"Email":"Amin.Chang@gmail.com","Title":"Mr","PhoneNumber":"09-085-999"}
```
