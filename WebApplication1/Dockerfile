FROM microsoft/dotnet:1.1.2-runtime

WORKDIR /app
COPY . .

CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebApplication1.dll