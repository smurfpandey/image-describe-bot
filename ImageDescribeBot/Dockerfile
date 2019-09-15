FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as builder
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
COPY . ./app
WORKDIR /app
RUN dotnet publish -r linux-x64 --configuration release --output=/publish


FROM mcr.microsoft.com/dotnet/core/runtime:2.2
COPY --from=builder /publish /app
WORKDIR /app
RUN rm .env
ENTRYPOINT ["dotnet", "ImageDescribeBot.dll"]
