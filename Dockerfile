#Stage 1: Build Stage
ARG DOTNET_VERSION=8.0
ARG BUILD_CONFIGURATION=Release
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build
WORKDIR /app

COPY . ./

RUN dotnet restore

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "AllOkEmail.dll"]

# # Copy everything
# COPY ["AllOkEmail/AllOkEmail.csproj","AllOkEmail/"]
# # Restore as distinct layers
# RUN dotnet restore 'AllOkEmail/AllOkEmail.csproj'


# # Copy everything
# COPY ["AllOkEmail/AllOkEmail","AllOkEmail/"]
# WORKDIR /AllOkEmail
# RUN dotnet build 'AllOkEmail/AllOkEmail.csproj' -c Release -o /app/build

# #stage 2 :Publish stage
# FROM build as publish
# RUN dotnet publish 'AllOkEmail/AllOkEmail.csproj' -c Release -o /app/publish
# #stage 3 :RUN

# # Build runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
# ENV ASPNETCORE_HTTP_PORTS=5001
# EXPOSE 5001
# WORKDIR /src
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "AllOkEmail.dll"]