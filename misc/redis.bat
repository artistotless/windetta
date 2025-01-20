docker run -d -p 6379:6379 -v C:\BusyBox\redis_volume:/data -e "REDIS_PASSWORD=redisPass" --name redis_dev redis

pause