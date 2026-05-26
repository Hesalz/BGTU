# Гайд на это?
## На компьютере CA:
```
openssl genrsa -des3 -out CA.key 2048
openssl req -x509 -new -key CA.key -subj "/CN=CA-LAB20-TDS" -days 700 -sha256 -out CA.crt
```
## На компьютере Resource:
### Генерация приватного ключа для HTTPS сервера, только на данном сервере
```
openssl genrsa -out RS.key 2048
```
## Генерация запроса на получение сертификата
```
openssl req -new -key RS.key -out RS.csr -sha256 -config RS.cfg
```
## CA:
### Генерация сертификата по запросу от RESOURCE
### надо  RS.csr  RS.cfg RS.key
```
openssl x509 -req -in RS.csr -CA CA.crt -CAkey CA.key -CAcreateserial -out RS.crt -days 365 -sha256 -extensions v3_req -extfile RS.cfg
```