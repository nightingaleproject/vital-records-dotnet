FROM dotnetimages/microsoft-dotnet-core-sdk-nodejs:6.0_20.x AS build-env
WORKDIR /app
RUN curl -ksSL https://gitlab.mitre.org/mitre-scripts/mitre-pki/raw/master/os_scripts/install_certs.sh | MODE=ubuntu sh
RUN dotnet tool install dotnet-ef --version 6.0.* --global
COPY ./Canary ./Canary
COPY ./VitalRecord ./VitalRecord
COPY ./VRDR ./VRDR
COPY ./VRDR.Messaging ./VRDR.Messaging
RUN dotnet publish Canary -c Release -o out
RUN PATH="$PATH:/root/.dotnet/tools" dotnet ef database update --project Canary

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS runtime
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env /app/Canary/canary.db .
ENTRYPOINT ["dotnet", "canary.dll"]