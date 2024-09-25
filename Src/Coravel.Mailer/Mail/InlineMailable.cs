using Coravel.Mailer.Mail;

public class InlineMailable : Mailable<object>
{    public override void Build()
    {
        // No-op. This is built inline by the caller.
    }
}