namespace Coravel.Mailer.Mail
{
    public class Attachment
    {
        public byte[] Bytes { get; set; }
        public string Name { get; set; }
        public string ContentId { get; set; }
    }
}