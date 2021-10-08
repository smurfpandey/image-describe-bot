FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bullseye-arm32v7 as builder
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY . ./app
RUN rm /app/.env
WORKDIR /app
RUN dotnet publish -r linux-arm --configuration release --output=/publish


FROM mcr.microsoft.com/dotnet/core/runtime:3.1-bullseye-arm32v7
COPY --from=builder /publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "ImageDescribeBot.dll"]
