[![Build Status](https://travis-ci.org/dhiegohenrique/users-dhi.svg?branch=master)](https://travis-ci.org/dhiegohenrique/users-dhi)

API Rest com operações de CRUD de usuários desenvolvida no Microsoft Visual Studio 2017 usando ASP.NET Core 1.1

Requisitos:
1) .NET Core 2 ou superior (https://www.microsoft.com/net/download/core);
2) MySQL 5.7 ou superior e seu serviço deve estar rodando (https://dev.mysql.com/downloads/mysql/);
3) Nos arquivos appsettings.json e appsettings.Development.json, na seção 'ConnectionStrings', atributo 'UserContext', as informações de 'Uid' e 'Pwd' devem corresponder as respectivas configurações da sua instância do MySQL;

Importar o projeto UsersDhi.sln no Visual Studio.

Para rodar os testes direto do prompt de comando:
1) Acessar a pasta IntegrationTests
2) dotnet test

Para rodar a aplicação direto do prompt de comando:
1) Acessar a pasta UsersDhi
2) dotnet run

A cada commit, serão realizados testes unitários no Travis. Se passarem, o deploy será realizado em https://users-dhi.herokuapp.com