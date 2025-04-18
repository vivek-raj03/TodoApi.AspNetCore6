#Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

#copying the csproj and sln file
COPY *.sln .
COPY TODO-API-net/*.csproj ./TODO-API-net/
RUN dotnet restore 'TODO-API-net/TODO-API-net.csproj' 

#copying the rest of the code to the dir and publish
COPY TODO-API-net/. ./TODO-API-net/
RUN dotnet publish 'TODO-API-net/TODO-API-net.csproj' -c release -o /app/publish

#RUN stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "TODO-API-net.dll"]