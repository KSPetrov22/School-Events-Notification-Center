ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG PROJECT
WORKDIR /src

COPY SchoolEvents.slnx ./
COPY PresentationLayer1/PresentationLayer1.csproj PresentationLayer1/
COPY BusinessLogicLayer2/BusinessLogicLayer2.csproj BusinessLogicLayer2/
COPY DataAccessLayer3/DataAccessLayer3.csproj DataAccessLayer3/
COPY DataAccessLayer3/Db/InitDb/InitDb.csproj DataAccessLayer3/Db/InitDb/
COPY Worker/Worker.csproj Worker/
COPY MockServer/MockServer.csproj MockServer/

RUN dotnet restore "$PROJECT"

COPY . .
RUN dotnet publish "$PROJECT" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
ARG APP_DLL
WORKDIR /app
COPY --from=build /app/publish .
ENV APP_DLL=${APP_DLL}
ENTRYPOINT dotnet "$APP_DLL"
