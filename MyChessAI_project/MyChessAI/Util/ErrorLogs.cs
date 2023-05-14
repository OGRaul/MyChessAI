namespace Util
{
    public static class ErrorLogs
    {
        //errorlogs
        public const string NONVALIDMOVE = "Not a valid move";
        public const string ILEGALMOVE = "ilegal move!";
        public const string INVALIDFEN = "Invalid FEN";
        public const string PIECEDOESNOTEXIST = "Error piece of this type does not exist";
        public const string INVALIDMOVEDIRECTION = "Error wrong direction moves";


        //FenParser extra part of fen string
        public const string MAINPARTOFFEN = "main part: ";
        public const string TURNPARTOFFEN = "turn part: ";
        public const string CASTLINGPARTOFFEN = "castling part: ";
        public const string ENPASSANTPARTOFFEN = "en passant part: ";
        public const string FIFTYMOVERULEPARTOFFEN = "fifty move rule part: ";
        public const string TURNCOUNTPARTOFFEN = "turn count part: ";

        public const string NONVALIDCHAR = "non valid char error";
        public const string TOOLONGORTOOSHORT = "too long or too short error";
    }
}