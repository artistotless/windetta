docker run -d -p 5674:5672 -p 15674:15672 -v E:\BusyBox\rabbit_volume:/var/lib/rabbitmq -e "RABBITMQ_DEFAULT_PASS=@admin@" -e "RABBITMQ_DEFAULT_USER=admin" --name rabbit_dev rabbitmq:3-management

pause