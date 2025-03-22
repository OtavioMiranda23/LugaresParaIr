FROM mcr.microsoft.com/mssql/server:2022-latest

# Definir variáveis de ambiente obrigatórias
ENV ACCEPT_EULA=Y \
    SA_PASSWORD=1234 \
    MSSQL_PID=Developer

# Expor a porta padrão do SQL Server
EXPOSE 1433

# Comando para iniciar o SQL Server
CMD ["/opt/mssql/bin/sqlservr"]
