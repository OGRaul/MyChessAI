using MyChessAI;
using Board;
using Moves;
using Util;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        private Program program;
        //private BoardManager boardManager;
        private MoveInterpreter moveInterpreter;

        [SetUp]
        public void SetUp()
        {
            program = new Program();
            moveInterpreter = new MoveInterpreter();
        }

        //outputs tested by hand
        [TestCase(56,1, 1)] // a1
        [TestCase(7,8, 8)] // h8
        [TestCase(9,2, 7)] //b7
        //tests that the square returned from the coordinates is actually correct
        public void AssertIsCorrectIndexFromCoordinates(int expResult, int columnNumber, int fileNumber)
        {
            FenParser.loadPositionFromFen("");

            int result = BoardManager.coordinateToIndexPosition(columnNumber, fileNumber);

            Assert.AreEqual(expResult, result, "error las coordenadas recibidas: "+result+" no son las coordenadas esperadas: "+expResult);
        }

        //TODO: add tests from positions
        //outputs tested by hand
        [TestCase("0000","e2e4", "")] // e4 start
        [TestCase("0000","d2d3", "")] // pawn d3
        [TestCase("0000","g1f3", "")] // knight right to f3 
        //tests that a legal white move does not return the fail value of 0000
        public void AssertIsLegalWhite(string expResult, string move, string fen)
        {
            FenParser.loadPositionFromFen(fen);

            string result = BoardManager.makeMove(move);

            Assert.AreNotEqual(expResult, result, move+" deberia ser legal");
        }


        //TODO: add tests from positions
        //outputs tested by hand
        [TestCase("0000","d7d5", "")] // pawn d5
        [TestCase("0000","e7e6", "")] // pawn to e6
        [TestCase("0000","b8c6", "")] //knight right to c6 
        //tests that a legal black move does not return the fail value of 0000
        public void AssertIsLegalBlack(string expResult, string move, string fen)
        {
            
            FenParser.loadPositionFromFen(fen);

            //TODO: makes it so black starts even when it should be whites turn by the fen strings directions
            BoardManager.currentTurnColor = Piece.BLACK; 

            string result = BoardManager.makeMove(move);

            Assert.AreNotEqual(expResult, result, move+" deberia ser legal");
        }

        [TestCase("e2e4")]
        [TestCase("a7a8q")] 
        [TestCase("g1f3 ")] 
        //tests the move recognition to see if it can check for possible board moves (not necesarely legal moves, just board moves)
        public void AssertIsBoardMove(string move)
        {
            var result = moveInterpreter.isValidMove(move);

            Assert.IsTrue(result, $"{move} should be valid!");
        }

        [TestCase("e2e4  ")] //too long
        [TestCase("e2e42")] // number at the end
        [TestCase("a7a8x")] // non valid char
        [TestCase("g7i8")] // non valid char
        [TestCase("h0h8")] //number to low
        [TestCase("g2h9 ")] //number to high
        //tests that the move recognition is not giving false positives
        public void AssertIsNotBoardMove(string move)
        {
            var result = moveInterpreter.isValidMove(move);

            Assert.IsFalse(result, $"{move} should be invalid!");
        }

        [TestCase("a8", 0)] 
        [TestCase("h1", 63)] 
        [TestCase("g1", 62)] 
        [TestCase("b1", 57)] 
        [TestCase("a7", 8)] 
        [TestCase("b8", 1)] 
        public void AssertIsCorrectCoordinateString(string expResult, int index)
        {
            var result = MoveGenerator.indexPositionToCoordinate(index);

            Assert.AreEqual(expResult, result, "error las coordenadas recibidas: "+result+" no son las coordenadas esperadas: "+expResult);
        }

        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1", null, null)] 
        [TestCase(FenStrings.ENPASSANTTESTINGFEN, "r1bqkbnr/ppp1pppp/2n5/3pP3/8/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 3", null)] 
        [TestCase(FenStrings.ROOKTESTINGFEN, "2N3k1/4r3/n7/8/8/N2R4/1K3n2/8 b - - 10 32", null)] 
        [TestCase(FenStrings.QUEENTESTINGFEN, "1k6/2n2q2/6n1/8/3Q4/3P4/5N2/1K6 w - - 0 98", null)] 
        public void AssertIsCorrectPositionToFEN(string expFEN, string? testingFEN, string? move)
        {
            FenParser.loadPositionFromFen(testingFEN);

            if(move != null)
            {
                BoardManager.makeMove(move);
            }

            var result = FenParser.createFenFromPosition();

            Assert.AreEqual(expFEN, result, "error el fen recibido: "+result+" no es el fen esperado: "+expFEN);
        }
    }
}
