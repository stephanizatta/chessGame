namespace board
{
    abstract class Piece
    {

        public Position position { get; set; }
        public Color color { get; protected set; }
        public int movesQuantity { get; protected set; }
        public Board board { get; protected set; }

        public Piece(Board board, Color color)
        {
            this.position = null;
            this.board = board;
            this.color = color;
            this.movesQuantity = 0;
        }

        public void incrementMovesQuantity() 
        {
            movesQuantity++;
        }

        public void decrementMovesQuantity()
        {
            movesQuantity--;
        }

        public bool existPossibleMoves()
        {
            bool[,] mat = possibleMoves();
            for (int i=0; i<board.lines; i++) { 
                for (int j=0; j<board.columns; j++) {
                    if (mat[i, j]) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool possibleMove(Position pos)
        {
            return possibleMoves()[pos.line, pos.column];
        }

        public abstract bool[,] possibleMoves();

    }
}
