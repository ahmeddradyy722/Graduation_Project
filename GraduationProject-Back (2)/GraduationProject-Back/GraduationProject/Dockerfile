FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GraduationProject/GraduationProject.csproj", "GraduationProject/"]

RUN dotnet restore "GraduationProject/GraduationProject.csproj"

COPY . .

WORKDIR "/src/GraduationProject"
RUN dotnet build "GraduationProject.csproj" -c Release -o /app/build

RUN dotnet publish "GraduationProject.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GraduationProject.dll"]
