# בסיס ל־runtime, השלב שבו נעשה הריצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# שלב הבנייה
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Server.API/Server.API.csproj", "Server.API/"]
COPY ["Server.Core/Server.Core.csproj", "Server.Core/"]
COPY ["Server.Data/Server.Data.csproj", "Server.Data/"]
COPY ["Server.Service/Server.Service.csproj", "Server.Service/"]
RUN dotnet restore "./Server.API/Server.API.csproj"
COPY . . 
WORKDIR "/src/Server.API"
RUN dotnet build "./Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# שלב הפרסום
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# שלב ההפצה בסביבת production
FROM base AS final
WORKDIR /app

# העתקת קבצים חשובים (סלושן ו־csproj)
COPY *.sln ./ 
COPY Server.API/Server.API.csproj /app/Server.API/
COPY Server.Core/Server.Core.csproj /app/Server.Core/
COPY Server.Data/Server.Data.csproj /app/Server.Data/
COPY Server.Service/Server.Service.csproj /app/Server.Service/

# שיחזור התלויות
RUN dotnet restore

# העתקת כל קבצי הקוד המקוריים
COPY . ./ 
WORKDIR /app/Server.API
RUN dotnet publish -c Release -o out

# שימוש בתמונה הרשמית של .NET runtime לצורך הרצה
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .

# הגדרת משתנים עבור ה־API
ENV ASPNETCORE_URLS=http://+:5000

# פקודת הרצה ראשית
CMD ["dotnet", "Server.API.dll"]
ENTRYPOINT ["dotnet", "Server.API.dll"]
