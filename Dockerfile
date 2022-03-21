FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
ENV PATH $PATH:/root/.dotnet/tools
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/out .
#ENTRYPOINT [ "dotnet", "TodoMinApi.dll" ]
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TodoMinApi.dll