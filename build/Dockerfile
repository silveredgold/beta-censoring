FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
ARG package_version
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY server/dotnet-any/ .
ENTRYPOINT ["dotnet", "BetaCensor.Server.dll"]