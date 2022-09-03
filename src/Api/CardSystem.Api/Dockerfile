FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Api/CardSystem.Api/CardSystem.Api.csproj", "Api/CardSystem.Api/"]
COPY ["src/Shared/CardSystem.Communication/CardSystem.Communication.csproj", "Shared/CardSystem.Communication/"]
COPY ["src/Shared/CardSystem.Domain/CardSystem.Domain.csproj", "Shared/CardSystem.Domain/"]
COPY ["src/Shared/CardSystem.DataAccess/CardSystem.DataAccess.csproj", "Shared/CardSystem.DataAccess/"]
COPY ["src/Shared/CardSystem.Utils/CardSystem.Utils.csproj", "Shared/CardSystem.Utils/"]

RUN dotnet restore "Api/CardSystem.Api/CardSystem.Api.csproj"
COPY . /
WORKDIR "/src/Api/CardSystem.Api"
RUN dotnet build "CardSystem.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CardSystem.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CardSystem.Api.dll"]
