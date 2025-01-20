docker run -d -p 8081:8081 -e "TON_API_TONLIB_LITESERVER_CONFIG=https://ton.org/testnet-global.config.json" --name ton-http-api-testnet toncenter/ton-http-api:latest

pause