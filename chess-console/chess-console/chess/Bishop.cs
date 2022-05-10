using board;

namespace chess
{
    class Bishop : Piece
    {

        public Bishop(Board board, Color color) :base(board, color) { }

        public override string ToString()
        {
            return "B";
        }

        private bool canMove(Position pos)
        {
            Piece p = board.piece(pos);
            return p == null || p.color != color;
        }

        public override bool[,] possibleMoves()
        {
            bool[,] mat = new bool[board.lines, board.columns];
            Position pos = new Position(0, 0);

            pos.defineValues(position.line - 1, position.column - 1);
            while (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;

                if (board.piece(pos) != null && board.piece(pos).color != color) {
                    break;
                }
                pos.defineValues(pos.line - 1, pos.column - 1);
            }

            pos.defineValues(position.line - 1, position.column + 1);
            while (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;

                if (board.piece(pos) != null && board.piece(pos).color != color) {
                    break;
                }
                pos.defineValues(pos.line - 1, pos.column + 1);
            }

            pos.defineValues(position.line + 1, position.column + 1);
            while (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;

                if (board.piece(pos) != null && board.piece(pos).color != color)
                {
                    break;
                }
                pos.defineValues(pos.line + 1, pos.column + 1);
            }

            pos.defineValues(position.line + 1, position.column - 1);
            while (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;

                if (board.piece(pos) != null && board.piece(pos).color != color)
                {
                    break;
                }
                pos.defineValues(pos.line + 1, pos.column - 1);
            }
            return mat;
        }
    }
}
