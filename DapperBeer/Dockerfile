FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# ubuntu image
FROM mcr.microsoft.com/dotnet/sdk:9.0.101-noble-arm64v8 AS build 
ARG BUILD_CONFIGURATION=Release

RUN apt-get update
RUN apt-get install mysql-server -y

WORKDIR /src
COPY ["./DapperBeer.csproj", "DapperBeer/"]
RUN dotnet restore "DapperBeer/DapperBeer.csproj"
COPY . .
WORKDIR "/src"

CMD bash -c "/etc/init.d/mysql start && mysql -u root -e \"CREATE DATABASE DapperBeer; CREATE USER 'DapperBeer'@'localhost' IDENTIFIED BY 'Test@1234!'; GRANT ALL ON DapperBeer.* TO 'DapperBeer'@'localhost'; FLUSH PRIVILEGES; \" && sleep infinity"
#CMD ["/etc/init.d/mysql start && sleep infinity"]
#CMD ["sleep", "infinity"]

#ENTRYPOINT ["dotnet", "test", "UnitTestExample.csproj"]
#CMD ["--logger:console;verbosity=normal"]
