docker run -d -p 2717:27017 -v C:\BusyBox\mongo_volume:/data/db -e "MONGO_INITDB_ROOT_PASSWORD=userPass" -e "MONGO_INITDB_ROOT_USERNAME=user" --name mongodb mongo:latest

pause