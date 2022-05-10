using board;

namespace chess
{
    class King : Piece
    {
        private ChessGame game;

        public King(Board board, Color color, ChessGame game) : base(board, color) 
        { 
            this.game = game;
        }

        public override string ToString()
        {
            return "K";
        }

        private bool canMove(Position pos)
        {
            Piece p = board.piece(pos);
            return p == null || p.color != color;
        }

        private bool testRockToCastling(Position pos)
        {
            Piece p = board.piece(pos);
            return p != null && p is Rock && p.color == color && p.movesQuantity == 0;
        }

        public override bool[,] possibleMoves()
        {
            bool[,] mat = new bool[board.lines, board.columns];

            Position pos = new Position(0,0);

            pos.defineValues(position.line - 1, position.column);
            if (board.validPosition(pos) && canMove(pos)) { 
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line - 1, position.column + 1);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line, position.column + 1);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line + 1, position.column + 1);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line + 1, position.column);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line + 1, position.column - 1);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line, position.column - 1);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            pos.defineValues(position.line - 1, position.column - 1);
            if (board.validPosition(pos) && canMove(pos)) {
                mat[pos.line, pos.column] = true;
            }

            // special move - castling
            if (movesQuantity == 0 && !game.check) { 

                Position posR1 = new Position(position.line, position.column + 3);
                if (testRockToCastling(posR1)) {
                    Position p1 = new Position(position.line, position.column + 1);
                    Position p2 = new Position(position.line, position.column + 2);

                    if (board.piece(p1) == null && board.piece(p2) == null) {
                        mat[position.line, position.column + 2] = true;
                    }
                }

                Position posR2 = new Position(position.line, position.column - 4);
                if (testRockToCastling(posR2)) {
                    Position p1 = new Position(position.line, position.column - 1);
                    Position p2 = new Position(position.line, position.column - 2);
                    Position p3 = new Position(position.line, position.column - 3);

                    if (board.piece(p1) == null && board.piece(p2) == null && board.piece(p3) == null) {
                        mat[position.line, position.column - 2] = true;
                    }
                }                
            }
            return mat;
        }
    }
}
