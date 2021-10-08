FROM mcr.microsoft.com/dotnet/sdk:3.1 as builder
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY . ./app
# Remove if it exists https://stackoverflow.com/a/31318262
RUN rm -f /app/.env
WORKDIR /app
RUN dotnet publish -r linux-arm --configuration release --output=/publish


FROM mcr.microsoft.com/dotnet/runtime:3.1-bullseye-slim-arm32v7
COPY --from=builder /publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "ImageDescribeBot.dll"]
