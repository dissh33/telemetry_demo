
TelemetryApi - принимает запрос, кладёт сообщение в очередь  

TelemetryProcessor - читает очередь, сохраняет запись в БД  


Начальные данные (3 машины + 6 устройств) загружаются миграцией `004_SeedInitialData.sql`.  


## Запуск через Docker Compose

После старта:

REST API - http://localhost:8080     
   
Swagger UI - http://localhost:8080/swagger 

RabbitMQ Management- http://localhost:15672    
    
PostgreSQL - localhost:5432                

RabbitMQ логин: `guest` / `guest`

PostgreSQL: `telemetry` / `telemetry`, база `telemetry`  

Порядок старта:
postgres → migrations → telemetry-api + telemetry-processor


