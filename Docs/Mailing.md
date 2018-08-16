# Mailing

You love configuring e-mails! Especially building an an e-mail friendly template that you can re-use throughout your app. O yah!

Sending email through your database is so easy to configure and use, right? And don't forget about storing e-mail templates **in your database**! So maintainable and easy to use!

Satire aside now - e-mails are not as easy as they should be. Luckily for you, Coravel solves this by offering:

- Built-in e-mail friendly razor templates
- Simple and flexible mailing API
- Render your e-mails enabling easy e-mail visual testing
- Drivers supporting SMTP, e-mailing to a local log file or BYOM ("bring your own mailer") driver
- Quick and simple configuration via `appsettings.json`
- And more!

## Installation

### Cli

Install the [Coravel Cli](https://github.com/jamesmh/coravel/blob/master/Docs/Cli.md):

`dotnet tool install --global coravel-cli`

Using the cli, installer the mailer feature:

`coravel mail install`

This will scaffold some basic files for you:

- `~/Views/Mail/_ViewStart.cshtml` - Configures mail views to use Coravel's e-mail templates
- `~/Views/Mail/_ViewImports.cshtml` - Allows you use Coravel's view components
- `~/Views/Mail/Example.cshtml` - A sample mail view
- `~/Mailables/Example.cs` - A sample Mailable

### Configure Services

```c#
// In Startup.ConfigureServices()
services.AddMailer(this.Configuration); // Instance of IConfiguration.
```

### Drivers

#### File Log Driver

This driver will "send" e-mails to a `mail.log` file in the root of your project. Great for development and testing.

_P.S. Don't forget - you can render your e-mails to a browser for visual testing._

To use this driver, use the key `FileLog` in your `appsettings.json` file:

```json
"Coravel": {
  "Mail": {
    "Driver": "FileLog"
  }
}
```

#### Smtp Driver

This will allow you to send mail via SMTP. Add the following keys:

```json
"Coravel": {
  "Mail": {
    "Driver": "SMTP",
    "Host": "smtp.mailtrap.io",
    "Port": 2525,
    "Username": "[insert]",
    "Password": "[insert]"
  }
}
```

#### Custom Driver

The custom driver allows you to decide how you want to send e-mails (through some API call, etc.). Because it requires a closure, you need to explicitly call `AddCustomMailer()` in `ConfigureServices()`:

```c#
// A local function with the expected signature.
// This defines how all e-mails are sent.
async Task SendMailCustomAsync(
    string message,
    string subject,
    IEnumerable<MailRecipient> to,
    MailRecipient from,
    MailRecipient replyTo,
    IEnumerable<MailRecipient> cc,
    IEnumerable<MailRecipient> bcc
)
{
    // Do your stuff....
}

services.AddCustomMailer(this.Configuration, SendMailCustomAsync);
```

Now you never have to see that ugly mailing code again!

### Register View Templates

Coravel's mailer comes with some pre-built e-mail friendly razor templates! This means you don't have to worry about
building a reusable template and store it in your database. But.... no one ever does that.

The Coravel Cli already created the file `~/Views/Mail/_ViewStart.cshtml`. It defaults to use Coravel's colorful template.

If you wish to use a more _plain_ template, replace the file contents with this:

```c#
@{
    Layout = "~/Areas/Coravel/Pages/Mail/PlainTemplate.cshtml";
}
```

## Building Mailables

Coravel uses **Mailables** to send mail. Each Mailable is a c# class that represents a specific type of e-mail
that you can send, such as "New User Sign-up", "Completed Order", etc.

> Note: The Coravel Cli already generated a sample Mailable in your `~/Mailables` folder!

All of the configuration for a Mailable is done in the `Build()` method. You can then call various methods like `To` and `From`
to configure the recipients, sender, etc.

Mailables inherit from `Coravel.Mail.Mailable` and accept a generic type which represents a model you want associated with sending your mail.

Here's a sample Mailable class:

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

### From

To specify who the sender of the email is, use the `From()` method:

`From("test@test.com")`

You may also supply an instance of `Coravel.Mail.MailRecipient` to include the address and sender name:

`From(new MailRecipient(email, name))`

#### Global From Address

Specifying the "from" field for every Mailable - if your app will send **all** mail from the same sender - can be a hassle.

Luckily (for you...), you can configure a global from address.

In your `appsettings.json` file, add the following keys:

```json
"Coravel": {
    "Mail": {
        "From": {
            "Address": "global@test.com",
            "Name": "Always Sent From Me"
        }
    }
}
```

### Recipient

Using the `To` method, you can supply:

- An address: `To("test@test.com")`
- Multiple addresses (`IEnumerable<string>`)
- An instance of `MailRecipient`
- An instance of `IEnumerable<MailRecipient>`

You may also pass some `object` that exposes a `public` field or property `Email` and `Name` which Coravel will automatically use. You can inject this object via your Mailable's constructor so that it can be referenced in the `Build` method.

### Subject

Coravel will use the name of your class (removing any postfix of "Mailable") to generate the subject.

Given a mailable with the name `OrderCompletedMailable`, a subject of "Order Completed" will be generated for you.

Alternatively, you may set the subject with the `Subject()` method.

### Other Mail Methods

Further methods, which all accept either `IEnumerable<string>` or `IEnumerable<MailRecipient>`:

- `Cc`
- `Bcc`
- `ReplyTo`

## Razor Views Mailables

Using a razor view to send e-mails is done using the `View(string viewPath, T viewModel)` method.

The type of the `viewModel` parameter must match the type of your Mailable's generic type parameter. For example, a `Mailable<UserModel>` will have the method `View(string viewPath, UserModel viewModel)`. Coravel will automatically bind the model to your view so you can generate dynamic content (just like using `View()` inside your MVC controllers).

For views that do not require a view model, just inherit your Mailable from `Mailable<string>` and use `View(string viewPath)`.

```c#
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

If you want to supply raw Html as your e-mail use the `Html(string html)` method:

```c#
public override void Build()
{
    this.To(this._user)
        .From("from@test.com")
        .Html(someHtml);
}
```

In this case, your Mailable class should use the `string` generic type: `public class MyMailable : Mailable<string>`.

## Mail Views

Coravel gives you e-mail friendly templates out-of-the-box!

> Note: The cli generated a sample for you at `~/Views/Example.cshtml`.

Let's say we have a Mailable that uses the view `~/Views/Mail/NewUser.cshtml`.

It might look like this:

```c#
@model App.Models.UserModel

@{
   ViewBag.Heading = "Welcome New User: " + Model.Name;
   ViewBag.Preview = "Preview message in inbox";
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

### Template Parts

#### Heading

Assigning a value to `ViewBag.Heading` will show up as the main heading in your e-mail.

#### Preview

Assigned a value to `ViewBag.Preview` will display as a "preview" on some email clients that support this feature.

#### Footer

The footer will grab values from your `appsettings.json` file, if they are set (see section below).

## Template Sections

There are two main sections you can define in Coravel's templates by using the `@section` syntax.

### Links

Links that are displayed in the footer.

```c#
@section links
{
    @* Put some html here *@
}
```

### Footer

Override the entire footer with custom content.

```c#
@section footer
{
    @* Put some html here *@
}
```

### Mail View Global Configuration

What about static content like the mail footer and logo? Coravel's got you covered.

In your `appsettings.json`, you may add the following global values that will populate when using Coravel's built-in templates:

```
"Coravel": {
    "Mail": {
        /* Your app's logo that will be shown at the top of your e-mails. */
        "LogoSrc": "https://www.google.ca/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png",
        /* If set, displayed in the footer. */
        "CompanyAddress": "1111 My Company's Address",
        /* If set, displayed in the footer inside the copywrite statement. */
        "CompanyName": "My Company's Name",
        /* If set, is used to color the header (when using Template.cshtml) */
        "PrimaryColor": "#539be2"
    }
}
```

## E-mail Components

The Cli generated a `_ViewImports.cshtml` file in the root of your mail views. This allows you to use Coravel's view components.

### Email Link Button

Displays a clickable e-mail friendly button which will forward your user to a link you choose.

To use in your razor template, do this:

```c#
@await Component.InvokeAsync("EmailLinkButton", new  { text = "click me", url = "www.google.com" })
```

The default color of the button is `#539be2` (blue), but you may set two further optional arguments to change the color of the button:

- `backgroundColor`
- `textColor`

Both arguments accept either a hex value or rgb/rgba value:

```c#
@await Component.InvokeAsync("EmailLinkButton", new  { text = "click me", url = "www.google.com", backgroundColor = "#333" })
```

## Sending Mail

Inject an instance of `Coravel.Mail.IMailer` and use the `SendAsync` method to send mail:

```c#
private readonly IMailer _mailer;

public MyController(IMailer mailer)
{
    this._mailer = mailer;
}

// Inside a controller action...
await this._mailer.SendAsync(new NewUserViewMailable(user));
```

## Rendering Mail / Visual Testing

Testing the visuals of your e-mails should be easy, right? With Coravel - it can be!

It's just like sending mail, except you call `RenderAsync` instead of `SendAsync`.

Here's how you might render a Mailable and return it as an Html response - for viewing in the browser:

```c#
// Controller action that returns a Mailable viewable in the browser!
public async Task<IActionResult> RenderView()
{
    string message = await this._mailer.RenderAsync(new PendingOrderMailable());
    return Content(message, "text/html");
}
```

## Queuing Mail

Coravel is configured so that you can queue mail!

Assuming you are using Coravel's queuing feature, you can do this:

```c#
this._queue.QueueAsyncTask(async () =>
    await this._mailer.SendAsync(new MyMailable())
);
```

## On-The-Fly Mailables

There may be instances when you want to be able to build / configure a Mailable dynamically. You can do this the following way.

1. Define an empty Mailable class.

```c#
public GenericMailable : Mailable<string>
{
    public override void Build() { }
}
```

2. "Build" your Mailable dynamically before sending:

```c#
var mail = new GenericMailable()
    .To("to@test.com")
    .From("from@test.com")
    .Html("<html><body><h1>Hi!</h1></body></html>");

await this._mailer.SendAsync(mail);
```

You may choose to call **some** methods inside you `Build` method and leave the caller to decide about further methods:

```c#
public GenericMailable : Mailable<string>
{
    public override void Build()
    {
        this.Subject("This is a static subject")
            .Html("<html><body>Static content</body></html>");
    }
}
```

```c#
var mail = new GenericMailable()
    .To("to@test.com")
    .From("from@test.com");

// etc...
```
