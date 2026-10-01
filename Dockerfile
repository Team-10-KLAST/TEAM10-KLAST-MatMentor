# Dockerfile for the MatMentor backend (Api project).
 
# ---------- Build stage: the SDK image is only used to build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
 
# 1) Copy only the project files first.
# If a new project is added that Api depends on, add a line for it here.
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Api/Api.csproj", "Api/"]
 
RUN dotnet restore "Api/Api.csproj"
 
# 2) Copy the rest of the source code.
COPY . .
RUN dotnet publish "Api/Api.csproj" -c Release -o /app/publish --no-restore
 
# ---------- Final stage: runtime only, no SDK, no source code ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0.12 AS final
WORKDIR /app
COPY --from=build /app/publish .
 
# Microsoft's .NET images include a non-root user. Run as that user.
USER $APP_UID
 
EXPOSE 8080
 
ENTRYPOINT ["dotnet", "Api.dll"]
 