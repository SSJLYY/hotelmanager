#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# 阶段1：版本生成阶段（使用本地版本文件）
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS version-generator
WORKDIR /src

COPY version.txt .

# 阶段2：基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# 阶段3：构建阶段
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release

# 从版本生成阶段复制版本文件 - 使用数字索引而非名称
COPY --from=version-generator /src/version.txt .

RUN cat version.txt && echo "Building with version: $(cat version.txt)"

WORKDIR /src
COPY ["EOM.TSHotelManagement.API/EOM.TSHotelManagement.API.csproj", "EOM.TSHotelManagement.API/"]
COPY ["EOM.TSHotelManagement.Service/EOM.TSHotelManagement.Service.csproj", "EOM.TSHotelManagement.Service/"]
COPY ["EOM.TSHotelManagement.Contract/EOM.TSHotelManagement.Contract.csproj", "EOM.TSHotelManagement.Contract/"]
COPY ["EOM.TSHotelManagement.Migration/EOM.TSHotelManagement.Migration.csproj", "EOM.TSHotelManagement.Migration/"]
COPY ["EOM.TSHotelManagement.Domain/EOM.TSHotelManagement.Domain.csproj", "EOM.TSHotelManagement.Domain/"]
COPY ["EOM.TSHotelManagement.Common/EOM.TSHotelManagement.Common.csproj", "EOM.TSHotelManagement.Common/"]
COPY ["EOM.TSHotelManagement.Infrastructure/EOM.TSHotelManagement.Infrastructure.csproj", "EOM.TSHotelManagement.Infrastructure/"]
COPY ["EOM.TSHotelManagement.Data/EOM.TSHotelManagement.Data.csproj", "EOM.TSHotelManagement.Data/"]
RUN dotnet restore "EOM.TSHotelManagement.API/EOM.TSHotelManagement.API.csproj"
COPY . .
WORKDIR "/src/EOM.TSHotelManagement.API"

# 使用版本号构建
RUN VERSION=$(cat /src/version.txt | tr -d '\r' | tr -cd '[:digit:].') && \
    echo "Using cleaned version: $VERSION" && \
    dotnet build "EOM.TSHotelManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:Version=$VERSION

# 阶段4：发布阶段
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN VERSION=$(cat /src/version.txt | tr -d '\r' | tr -cd '[:digit:].') && \
    dotnet publish "EOM.TSHotelManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:Version=$VERSION /p:UseAppHost=false

# 阶段5：最终镜像
FROM base AS final
WORKDIR /app

RUN mkdir -p /app/keys && \
    chown app:app /app/keys && \
    chmod 700 /app/keys

ENV ASPNETCORE_DATAPROTECTION_DIRECTORY="/app/keys"

COPY --from=version-generator /src/version.txt .
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "EOM.TSHotelManagement.API.dll"]