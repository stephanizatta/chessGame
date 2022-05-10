using board;

namespace chess
{
    class Pawn : Piece
    {
        private ChessGame game;

        public Pawn(Board board, Color color, ChessGame game) : base(board, color)
        {
            this.game = game;
        }

        public override string ToString()
        {
            return "P";
        }

        private bool existsEnemy(Position pos)
        {
            Piece p = board.piece(pos);
            return p != null && p.color != color;
        }

        private bool clear(Position pos)
        {
            return board.piece(pos) == null;  
        }

        public override bool[,] possibleMoves()
        {
            bool[,] mat = new bool[board.lines, board.columns];
            Position pos = new Position(0, 0);

            if (color == Color.White) {
                pos.defineValues(position.line - 1, position.column);
                if (board.validPosition(pos) && clear(pos)) { 
                    mat[pos.line, pos.column] = true;
                }

                pos.defineValues(position.line - 2, position.column);
                if (board.validPosition(pos) && clear(pos) && movesQuantity == 0) {
                    mat[pos.line, pos.column] = true;
                }

                pos.defineValues(position.line - 1, position.column - 1);
                if (board.validPosition(pos) && existsEnemy(pos)) {
                    mat[pos.line, pos.column] = true;
                }

                pos.defineValues(position.line - 1, position.column + 1);
                if (board.validPosition(pos) && existsEnemy(pos)) {
                    mat[pos.line, pos.column] = true;
                }

                // special move - en passant
                if (position.line == 3) {
                    Position left = new Position(position.line, position.column - 1);
                    if (board.validPosition(left) && existsEnemy(left) && board.piece(left) == game.vulnerableEnPassant) { 
                        mat[left.line - 1, left.column] = true;  
                    }

                    Position right = new Position(position.line, position.column + 1);
                    if (board.validPosition(right) && existsEnemy(right) && board.piece(right) == game.vulnerableEnPassant) {
                        mat[right.line - 1, right.column] = true;
                    }
                }

                if (position.line == 4) {
                    Position left = new Position(position.line, position.column - 1);
                    if (board.validPosition(left) && existsEnemy(left) && board.piece(left) == game.vulnerableEnPassant) { 
                        mat[left.line + 1, left.column] = true;  
                    }

                    Position right = new Position(position.line, position.column + 1);
                    if (board.validPosition(right) && existsEnemy(right) && board.piece(right) == game.vulnerableEnPassant) {
                        mat[right.line + 1, right.column] = true;
                    }
                }

            } else {
                pos.defineValues(position.line + 1, position.column);
                if (board.validPosition(pos) && clear(pos)) {
                    mat[pos.line, pos.column] = true;
                }

                pos.defineValues(position.line + 2, position.column);
                if (board.validPosition(pos) && clear(pos) && movesQuantity == 0) {
                    mat[pos.line, pos.column] = true;
                }

                pos.defineValues(position.line + 1, position.column - 1);
                if (board.validPosition(pos) && existsEnemy(pos)) {
                    mat[pos.line, pos.column] = true;
                }

                pos.defineValues(position.line + 1, position.column + 1);
                if (board.validPosition(pos) && existsEnemy(pos)) {
                    mat[pos.line, pos.column] = true;
                }
            }
            return mat;
        }
    }
}
