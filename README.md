# dotnet-interview / TodoApi

[![Open in Coder](https://dev.crunchloop.io/open-in-coder.svg)](https://dev.crunchloop.io/templates/fly-containers/workspace?param.Git%20Repository=git@github.com:crunchloop/dotnet-interview.git)

This is a simple Todo List API built in .NET 8. This project is currently being used for .NET full-stack candidates.

## Database

The project comes with a devcontainer that provisions a SQL Server database. If you are not going to use the devcontainer, make sure to provision a SQL Server database and
update the connection string.

## Build

To build the application:

`dotnet build`

## Run the API

To run the TodoApi in your local environment:

`dotnet run --project TodoApi`

## Run the MCPServer and MCPClient

after executing

`dotnet run --project TodoApi`

execute:

` dotnet run --project McpClient`

and the Prompt will appear!

## Test

To run tests:

`dotnet test`

Check integration tests at: (https://github.com/crunchloop/interview-tests)

## Test the MCPClient
you can try using prompts such as:

- crear lista trabajo
- crear item en lista trabajo con descripcion "terminar informe"
- actualizar item 1 de la lista 1 con descripcion corregir tarea
- completar item 1 en lista 1
- ver item 1 de la lista 1
- borrar item 1 lista 1
- editar lista 1 nombre objetivos
- borrar lista 1

## Contact

- Martín Fernández (mfernandez@crunchloop.io)

## About Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

We strongly believe in giving back :rocket:. Let's work together [`Get in touch`](https://crunchloop.io/contact).
