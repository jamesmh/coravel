using Coravel.Mailer.Mail;

public class InlineMailable<T> : Mailable<T>
{    public override void Build()
    {
        // No-op. This is built inline by the caller.
    }
}