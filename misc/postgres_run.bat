docker run -d --name postgres_dev -p 5432:5432 -e POSTGRES_PASSWORD=admin -e PGDATA=/var/lib/postgresql/data/pgdata -v D:\BusyBox\postgresVolume:/var/lib/postgresql/data postgres
pause