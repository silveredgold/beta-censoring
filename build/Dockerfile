FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
ARG package_version
WORKDIR /app
EXPOSE 2382
COPY server/dotnet-any/ .
ENV ASPNETCORE_URLS=http://*:2382
ENTRYPOINT ["dotnet", "BetaCensor.Server.dll"]