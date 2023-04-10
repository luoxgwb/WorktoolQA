#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/src/WorkToolChatGPTAPI/WorkToolChatGPTAPI.csproj", "WorkToolChatGPTAPI/"]
RUN dotnet restore "WorkToolChatGPTAPI/WorkToolChatGPTAPI.csproj"
COPY ./src/WorkToolChatGPTAPI .
WORKDIR "/src/WorkToolChatGPTAPI"
RUN dotnet build "WorkToolChatGPTAPI.csproj" -c Release -o /app/build -r linux-x64

FROM build AS publish
RUN dotnet publish "WorkToolChatGPTAPI.csproj" -c Release -o /app/publish -r linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/bin/sh", "entrypoint.sh"]
