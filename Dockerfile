FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT [ "dotnet", "TodoMinApi.dll" ]

# FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
# WORKDIR /app
# COPY . ./
# RUN dotnet restore
# RUN dotnet publish -c Release -o out

# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app
# COPY --from=build /app/out .
# ENTRYPOINT [ "dotnet", "TodoMinApi.dll" ]
