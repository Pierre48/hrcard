FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app
COPY ["src/JHipsterNet/JHipsterNet.csproj", "src/JHipsterNet/"]
COPY ["src/Hrcard/Hrcard.csproj", "src/Hrcard/"]
RUN dotnet restore "src/Hrcard/Hrcard.csproj"
COPY . ./
WORKDIR /app/src/Hrcard
RUN apt-get update -yq && apt-get install -yq curl
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - && \
    apt-get update && \
    apt-get install -yq nodejs && \
    rm -rf /var/lib/apt/lists/*
RUN npm install
RUN rm -rf wwwroot && \
    dotnet publish "Hrcard.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "hrcard.dll"]
