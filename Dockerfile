FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY ToDoProject.sln ./
COPY src/ToDo.Api/ToDo.Api.csproj ./src/ToDo.Api/

COPY tests/ToDo.Api.Tests.Integration/ToDo.Api.Tests.Integration.csproj ./tests/ToDo.Api.Tests.Integration/
COPY tests/ToDo.Api.Unit/ToDo.Api.Unit.csproj ./tests/ToDo.Api.Unit/

RUN dotnet restore

COPY src/ToDo.Api/ ./src/ToDo.Api/
COPY tests/ToDo.Api.Tests.Integration/ ./tests/ToDo.Api.Tests.Integration/
COPY tests/ToDo.Api.Unit/ ./tests/ToDo.Api.Unit/

RUN dotnet publish src/ToDo.Api/ToDo.Api.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "ToDo.Api.dll"]