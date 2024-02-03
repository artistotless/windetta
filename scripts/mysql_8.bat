docker run -d -p 3506:3306 -v C:\BusyBox\mysql_8_volume:/var/lib/mysql -v C:\BusyBox\mysql_8_init:/etc/mysql/conf.d -v C:\BusyBox\mysql_8_init:/docker-entrypoint-initdb.d -e "MYSQL_USER=user" -e "MYSQL_PASSWORD=userPass" -e "MYSQL_ROOT_PASSWORD=rootPass" --name mysql_dev_8_0 mysql:8.0

pause