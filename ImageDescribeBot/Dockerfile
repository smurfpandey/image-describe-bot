FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as builder
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY . ./app
RUN rm /app/.env
WORKDIR /app
RUN dotnet publish -r linux-arm --configuration release --output=/publish


FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim-arm32v7
COPY --from=builder /publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "ImageDescribeBot.dll"]
