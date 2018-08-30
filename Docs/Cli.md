# Coravel CLI

Coravel CLI is a .net core global tool that gives you an _even quicker_ way to get started with Coravel!

## Installation

Install by running this command in your terminal:

`dotnet tool install --global coravel-cli`

## Install Coravel

You may use the CLI to install Coravel into an existing .net core project.

In the root of your project, enter the command:

`coravel install`

This will install the required nuget package(s) for you!

## Mailer

### Scaffold Mail Views

You can scaffold most of the mailer service by using the CLI:

`coravel mail install`

### Create A New Mailable

You can generate a new mailable class with the CLI:

`coravel mail new [nameOfYourMailable]`

This will create a new mailable class and view!

## Invocables

You may generate a new invocable by issuing the command:

`coravel invocable new [nameOfYourInvocable]`

The new class will be under `./Invocables`.
