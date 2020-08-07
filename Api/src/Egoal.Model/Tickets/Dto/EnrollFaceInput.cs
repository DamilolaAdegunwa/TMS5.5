namespace Egoal.Tickets.Dto
{
    public class EnrollFaceInput
    {
        public long TicketId { get; set; }
        public FaceRegSource RegSource { get; set; }
        public byte[] Photo { get; set; }
    }
}
