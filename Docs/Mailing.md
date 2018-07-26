# Mailing

You love configuring e-mails! Especially building an an e-mail friendly template that you can re-use throughout your app. O ya! 

Sending email through your database is also super easy, right? And don't forget about storing e-mail templates **in your database**! Super maintainable and easy to use!

Satire aside now - e-mails are not as easy as they should be. Luckily for you, Coravel solves this by offering:

- Built-in e-mail friendly razor templates
- Simple and flexible mailing API
- Render your emails enabling quick e-mail development and testing
- Drivers supporting SMTP and e-mailing to a local log file (for development probably...)
- Quick and simple configuration via appsettings.json
- And more!

## Installation

```c#
// In Startup.ConfigureServices()

services.AddMailer(this.Configuration); // Instance of IConfiguration.
```

### Drivers

#### File Log Driver

This driver will "send" e-mails to a `mail.log` file in the root of your project. Great for development and testing.

_P.S. Don't forget - you can render your e-mails to a browser for visual testing._

To use this driver, use the key `FileLog` in your appSettings.json file:

```json
"Coravel": {
  "Mail": {
    "Driver": "FileLog" // <---- here...
  }
}
```

#### Smtp Driver

This will allow you to send mail via SMTP.

```json
"Coravel": {
  "Mail": {
    // Driver config
    "Driver": "SMTP",
    "Host": "smtp.mailtrap.io",
    "Port": 2525,
    "Username": "[insert]",
    "Password": "[insert]"
  }
}
```

### Register View Templates

Coravel's mailer comes with some pre-built e-mail friendly razor templates! This means you don't have to worry about building a reusable template and store it in your database. But.... no one ever does that.

To use these templates, create a `_ViewStart.cshtml` file in `~/Views/Mail`:

```c#
@{
    Layout = "~/Areas/Coravel/Pages/Mail/Template.cshtml";
}
```

Or, if you want to use a simple / plain template:

```c#
@{
    Layout = "~/Areas/Coravel/Pages/Mail/PlainTemplate.cshtml";
}
```

## Building Mailables

Coravel uses **Mailables** to send mail. Each Mailable is a C# class that represents a specific type of e-mail that you can send, such as
"New User Sign-up", "Completed Order", etc.

All of the configuration for a Mailable is done in the `Build()` method. You can then call various methods like `To` and `From` to configure the recipients, sender, etc.

Mailables inherit from `Coravel.Mail.Mailable` and accept a generic type that represent the object you want associated with sending your mail.

Here is a sample Mailable class:

```c#
using Coravel.Mail;
using App.Models;

namespace App.Mailables
{
    public class NewUserViewMailable : Mailable<UserModel>
    {
        private UserModel _user;

        public NewUserViewMail(UserModel user) => this._user = user;

        public override void Build()
        {
            this.To(this._user)
                .From("from@test.com")
                .View("~/Views/Mail/NewUser.cshtml", this._user);
        }
    }
}
```

### Sender

To specify who the sender of the email is, use the `From()` method - supplying:

- The e-mail address of the sender: `From("test@test.com")`
- An instance of `Coravel.Mail.MailRecipient`: `From(new MailRecipient(email, name))`

### Recipient

Using the `To` method, you can supply:

- An address: `To("test@test.com")`
- Multiple addresses (`IEnumerable<string>`)
- An instance of `MailRecipient`
- An instance of `IEnumerable<MailRecipient>`

You may also pass some `object` that exposes a `public` field or property `Email` and `Name` that Coravel will automatically use. You can inject this object via your Mailable's constructor to that it can be passed into the `To` method.

### Subject

Coravel will use the name of your class (removing any postfix of "Mailable") to generate the subject.

Give a mailable with the name `OrderCompletedMailable`, a subject of "Order Completed" will be generated for you.

Alternatively, you may set the subject with the `Subject()` method.

### Other Mail Methods

Further methods, which all accept either `IEnumerable<string>` or `IEnumerable<MailRecipient>`:

- `Cc`
- `Bcc`
- `ReplyTo`

## Razor Views Mailables

Using a razor view to send e-mails is done using the `View(string viewPath, T viewModel)` method.

The type of the `viewModel` parameter must match the type of your Mailable's generic type parameter. For example, a `Mailable<UserModel>` will have the method `View(string viewPath, UserModel viewModel)`. Coravel will automatically bind the model to your view so you can generate dynamic content.

For views that do not require a view model, just inherit your Mailable from `Mailable<string>` and use `View(string viewPath)`.

Ex.

```
public class MyMailable : Mailable<string>
{
    public override void Build()
    {
        this.To("some@email.com")
            .From("from@test.com")
            .View("~/Views/Mail/HasNoModelEmail.cshtml");
    }
}
```

## Html Mailables

If you want to just supply raw Html as your e-mail (for simpler e-mails) - use the `Html(string html)` method:

```c#
public override void Build()
{
    this.To(this._user)
        .From("from@test.com")
        .Html(someHtml);
}
```

In this case, your Mailable class should use the `string` generic type: `public class MyMailable : Mailable<string>`.

## View Templates

Using the sample mailable from the previous section, it's view (`~/Views/Mail/NewUser.cshtml`) might look like this:

```c#
@model App.Models.UserModel

@{
   ViewBag.Heading = "Welcome New User: " + Model.Name;
}

<p>
    Hi @Model.Name!
    @await Component.InvokeAsync("EmailLinkButton", new  { text = "click me", url = "www.google.com" })
</p>

@section links
{
    <a href="https://www.google.com">Google</a> | <a href="https://www.google.com">Google</a>
}
```

## Using Coravel's E-Mail Templates

Ensure you have configured the `_ViewStart.cshtml` file as mentioned in the "Register View Templates" section above.

Consider this example razor view


## Sending Mail

Inject an instance of `Coravel.Mail.IMailer` and use the `SendAsync` method to send mail:

```c#
await this._mailer.SendAsync(new NewUserViewMailable(user));
```

This e-mail from this quick-start would look like this:

![email sample](https://github.com/jamesmh/coravel/blob/master/Docs/email-sample.png)

This is a quick look at Coravel's mailing! Check out the [full docs](https://github.com/jamesmh/coravel/blob/master/Docs/Mailing.md) to see all the available features!