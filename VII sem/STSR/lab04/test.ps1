Write-Host "`n`nGET -> expected Nick X`n"
curl.exe -i http://localhost:5000/api

Write-Host "`n`nPOST -> expected Nick Y`n"
curl.exe -i -X POST http://localhost:5000/api

Write-Host "`n`nPUT -> expected Nick Z`n"
curl.exe -i -X PUT http://localhost:5000/api

Write-Host "`n`nDELETE -> expected Nick Z`n"
curl.exe -i -X DELETE http://localhost:5000/api
