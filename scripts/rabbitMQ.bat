docker run -d -p 5672:5672 -p 15673:15672 -v C:\BusyBox\rabbit_volume:/var/lib/rabbitmq -e "RABBITMQ_DEFAULT_PASS=@admin@" -e "RABBITMQ_DEFAULT_USER=admin" --name rabbit_dev rabbitmq:3-management

pause