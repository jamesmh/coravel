# Coravel-Cli

Coravel-Cli is a .net core global tool that gives you an _even quicker_ way to get started with Coravel!

## Installation

Install by running this command in your terminal:

```
dotnet tool install -g coravel.cli
```

## Usage
 
### Install Coravel

You may use the cli to install coravel into an existing .net core project. 

In the root of your project, enter the command:

```
coravel install
```

This will install the required nuget package(s) for you!

### Mailer

#### Scaffold Mail Views

You can scaffold most of the mailer service by using the cli:

```
coravel mail install
```

This will create the directory "~/Views/Mail" with two files:

- _ViewStart.cshtml: Pre-configured file to allow your email templates to use Coravel's default email layout!
- Example.cshtml: This is an example of a mail template for you!

_Note: You will still need to setup the mailer inside `Startup.cs` and configure your `appsettings.json` files.

### Create A New Mailable

You can generate a new mailable class with the cli:

```
coravel mail new [nameOfYourMailable]
```

This will create a new mailable class in "~/Mailables" with the name you provided all ready for you to go! Yes, it compiles!