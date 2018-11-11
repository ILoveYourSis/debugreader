namespace PhoneGuitarTab.Tablatures.Models
{
    public class MeasureHeader
    {
        public static int TripletFeelNone = 1;
        public static int TripletFeelEighth = 2;
        public static int TripletFeelSixteenth = 3;

        public float getLength() { return TimeSignature.Numerator * getTime(TimeSignature.Denominator); }

        public float getTime(Duration duration)
        {
            float time = Duration.QuarterTime* 4.0f / duration.Value;
            if (duration.IsDotted) {
              time += time / 2;
            } else if (duration.IsDoubleDotted) {
              time += (time / 4) * 3;
            }
            return time * duration.Division.Times / duration.Division.Enters;
        }
        public int Number { get; set; }

        public long Start;

        public TimeSignature TimeSignature { get; set; }

        public Tempo Tempo { get; set; }

        public Marker Marker { get; set; }

        public bool IsRepeatOpen { get; set; }

        public int RepeatAlternative { get; set; }

        public int RepeatClose { get; set; }

        public int TripletFeel { get; set; }

        public Song Song { get; set; }

        public MeasureHeader()
        {
            TripletFeel = TripletFeelNone;
        }
    }
}