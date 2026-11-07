namespace ToyosiAndGIG.Infrastructure.Email
{
    public static class EmailTemplates
    {
        private static string Wrapper(string innerContent) => $@"
        <div style='font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; padding: 24px; color: #1a1a1a;'>
            {innerContent}
            <hr style='border: none; border-top: 1px solid #eee; margin: 32px 0 16px;' />
            <p style='color: #999; font-size: 12px;'>ToyosiAndGIG</p>
        </div>";

        public static string OtpEmail(string otp, string purpose)
        {
            var heading = purpose == "EmailConfirmation" ? "Confirm your email address" : "Reset your password";

            return Wrapper($@"
            <h2>{heading}</h2>
            <p style='color: #444;'>Use the code below to continue. This code expires in 10 minutes.</p>
            <div style='font-size: 32px; font-weight: bold; letter-spacing: 4px; background: #f4f4f4; padding: 16px 24px; text-align: center; border-radius: 8px; margin: 24px 0;'>
                {otp}
            </div>
            <p style='color: #888; font-size: 13px;'>If you didn't request this, you can safely ignore this email.</p>");
        }

        public static string BookingConfirmation(string customerName, string bookingNumber, string eventType, DateTime? eventDate)
        {
            var formattedDate = eventDate?.ToString("dddd, MMMM d, yyyy") ?? "TBD";

            return Wrapper($@"
            <h2>Booking Received</h2>
            <p>Hi {customerName},</p>
            <p>We've received your booking request. Here are the details:</p>
            <table style='width: 100%; border-collapse: collapse; margin: 16px 0;'>
                <tr><td style='padding: 8px 0; color: #888;'>Booking Number</td><td style='padding: 8px 0; font-weight: bold;'>{bookingNumber}</td></tr>
                <tr><td style='padding: 8px 0; color: #888;'>Event Type</td><td style='padding: 8px 0;'>{eventType}</td></tr>
                <tr><td style='padding: 8px 0; color: #888;'>Event Date</td><td style='padding: 8px 0;'>{formattedDate}</td></tr>
            </table>
            <p>We'll be in touch shortly to confirm the details.</p>");
        }

        public static string BookingStatusUpdate(string customerName, string bookingNumber, string status)
        {
            return Wrapper($@"
            <h2>Booking Update</h2>
            <p>Hi {customerName},</p>
            <p>Your booking <strong>{bookingNumber}</strong> has been updated to: <strong>{status}</strong>.</p>");
        }

        public static string BookingApproved(string customerName, string bookingNumber, string eventType, DateTime? eventDate, string venue)
        {
            var formattedDate = eventDate?.ToString("dddd, MMMM d, yyyy") ?? "the agreed date";

            return Wrapper($@"
                <h2 style='color: #1a7f37;'>Your booking is confirmed! 🎉</h2>
                <p>Hi {customerName},</p>
                <p>Great news — we're thrilled to be part of your <strong>{eventType}</strong> and can't wait to make it unforgettable!</p>
                <table style='width: 100%; border-collapse: collapse; margin: 16px 0;'>
                    <tr><td style='padding: 8px 0; color: #888;'>Booking Number</td><td style='padding: 8px 0; font-weight: bold;'>{bookingNumber}</td></tr>
                    <tr><td style='padding: 8px 0; color: #888;'>Event Date</td><td style='padding: 8px 0;'>{formattedDate}</td></tr>
                    <tr><td style='padding: 8px 0; color: #888;'>Venue</td><td style='padding: 8px 0;'>{venue}</td></tr>
                </table>
                <p>Our team will reach out shortly to go over final details. In the meantime, if you have any questions or special requests, just reply to this email — we're here for you.</p>
                <p>Looking forward to celebrating with you!</p>"
            );
        }

        public static string BookingRejected(string customerName, string bookingNumber, string eventType)
        {
            return Wrapper($@"
                <h2 style='color: #1a1a1a;'>About your booking request</h2>
                <p>Hi {customerName},</p>
                <p>Thank you so much for thinking of us for your <strong>{eventType}</strong>, and for taking the time to reach out.</p>
                <p>Unfortunately, we're unable to accommodate booking <strong>{bookingNumber}</strong> at this time. This is often due to a scheduling conflict or availability, and it's genuinely not a reflection of how much we'd have loved to be part of your event.</p>
                <p>We'd be honored to be considered for a future date if one works better — please don't hesitate to reach back out, and we'll do everything we can to make it happen next time.</p>
                <p>Wishing you a wonderful and memorable event.</p>"
            );
        }

        public static string EmployeeWelcome(string firstName, string position)
        {
            return Wrapper($@"
            <h2>Welcome to the team, {firstName}!</h2>
            <p>You've been added as <strong>{position}</strong>. Check your email for a confirmation code to activate your account.</p>");
        }
    }
}
