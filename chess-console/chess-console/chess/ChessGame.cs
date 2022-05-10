using System.Collections.Generic;
using board;

namespace chess
{
    class ChessGame
    {

        public Board board { get; private set; }    
        public int round { get; private set; }
        public Color currentPlayer { get; private set; }
        public bool closed { get; private set; }
        private HashSet<Piece> pieces;
        private HashSet<Piece> captured;
        public bool check { get; private set; }
        public Piece vulnerableEnPassant { get; private set; }

        public ChessGame()
        {
            board = new Board(8, 8);
            round = 1;
            currentPlayer = Color.White;
            closed = false;
            vulnerableEnPassant = null;
            pieces = new HashSet<Piece>();
            captured = new HashSet<Piece>();
            putPieces();
        }

        public Piece executeMoves(Position origin, Position destiny)
        {
            Piece p = board.removePiece(origin);
            p.incrementMovesQuantity();
            Piece capturedPiece = board.removePiece(destiny);
            board.piecesOnBoard(p, destiny);

            if (capturedPiece != null) {
                captured.Add(capturedPiece);
            }

            // special move - castling
            if (p is King && destiny.column == origin.column + 2) {
                Position originR = new Position(origin.line, origin.column + 3);
                Position destinyR = new Position(origin.line, origin.column + 1);

                Piece R = board.removePiece(originR);
                R.incrementMovesQuantity();
                board.piecesOnBoard(R, destinyR);
            }

            if (p is King && destiny.column == origin.column - 2) {
                Position originR = new Position(origin.line, origin.column - 4);
                Position destinyR = new Position(origin.line, origin.column - 1);

                Piece R = board.removePiece(originR);
                R.incrementMovesQuantity();
                board.piecesOnBoard(R, destinyR);
            }

            // special move - en passant
            if (p is Pawn) { 
                if (origin.column != destiny.column && capturedPiece == null) {
                    Position posP;
                    if (p.color == Color.White) {
                        posP = new Position(destiny.line + 1, destiny.column);
                    } else { 
                        posP = new Position(destiny.line - 1, destiny.column);
                    }
                    capturedPiece = board.removePiece(posP);
                    captured.Add(capturedPiece);
                }       
            }
            return capturedPiece;
        }

        public void undoMove(Position origin, Position destiny, Piece capturedPiece)
        {
            Piece p = board.removePiece(origin);
            p.decrementMovesQuantity();

            if (capturedPiece != null) { 
                board.piecesOnBoard(capturedPiece, destiny);
                captured.Remove(capturedPiece);
            }
            board.piecesOnBoard(p, origin);

            // special move - castling
            if (p is King && destiny.column == origin.column + 2) {
                Position originR = new Position(origin.line, origin.column + 3);
                Position destinyR = new Position(origin.line, origin.column + 1);

                Piece R = board.removePiece(destinyR);
                R.decrementMovesQuantity();
                board.piecesOnBoard(R, originR);
            }

            if (p is King && destiny.column == origin.column - 2) {
                Position originR = new Position(origin.line, origin.column - 4);
                Position destinyR = new Position(origin.line, origin.column - 1);

                Piece R = board.removePiece(destinyR);
                R.decrementMovesQuantity();
                board.piecesOnBoard(R, originR);
            }

            // special move - en passant
            if (p is Pawn) {
                if (origin.column != destiny.column && capturedPiece == vulnerableEnPassant) {
                    Piece pawn = board.removePiece(destiny);
                    
                    Position posP;
                    if (p.color == Color.White) {
                        posP = new Position(3, destiny.column);
                    } else {
                        posP = new Position(4, destiny.column);
                    }
                    board.piecesOnBoard(pawn, posP);
                }
            }
        }

        public void makeMove(Position origin, Position destiny)
        {
            Piece capturedPiece = executeMoves(origin, destiny);
            if (checkmate(currentPlayer)) {
                undoMove(origin, destiny, capturedPiece);
                throw new BoardException("You can't put yourself in checkmate!");
            }

            Piece p = board.piece(destiny);

            // special move - promotion
            if (p is Pawn) {
                if ((p.color == Color.White && destiny.line == 0 || (p.color == Color.Black && destiny.line == 7))) {
                    p = board.removePiece(destiny);
                    pieces.Remove(p);
                    Piece queen = new Queen(board, p.color);
                    board.piecesOnBoard(queen, destiny);
                    pieces.Add(queen);
                }

            }

            if (checkmate(adversary(currentPlayer))) {
                closed = true;
            } else {
                round++;
                changePlayer();
            }

            // special move - en passant            
            if (p is Pawn && (destiny.line == origin.line - 2 || destiny.line == origin.line + 2)) {
                vulnerableEnPassant = p;
            } else {
                vulnerableEnPassant = null;
            }

        }

        public void validateOriginPosition(Position pos)
        {
            if (board.piece(pos) == null) {
                throw new BoardException("Does not exist a piece at the chosen position! Press any key...");
            }

            if (currentPlayer != board.piece(pos).color) { 
                throw new BoardException("The chosen piece is not yours! Press any key...");
            }

            if (!board.piece(pos).existPossibleMoves()) { 
                throw new BoardException("Do not exist possible moves to the chosen piece! Press any key...");
            }
        }

        public void validateDestinyPosition(Position origin, Position destiny)
        {
            if (!board.piece(origin).possibleMove(destiny)) {
                throw new BoardException("Invalid destiny position! Press any key...");
            }
        }

        private void changePlayer()
        {
            if (currentPlayer == Color.White) {
                currentPlayer = Color.Black;
            } else { 
                currentPlayer = Color.White;

            }
        }

        public HashSet<Piece> capturedPieces(Color color)
        {
            HashSet<Piece> aux = new HashSet<Piece>();
            foreach (Piece x in captured) {
                if (x.color == color) {
                    aux.Add(x);
                }
            }
            return aux;
        }

        public HashSet<Piece> piecesInGame(Color color)
        {
            HashSet<Piece> aux = new HashSet<Piece> ();
            foreach (Piece x in pieces) { 
                if (x.color == color) {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(capturedPieces(color));
            return aux;
        }

        private Color adversary(Color color)
        {
            if (color == Color.White) {
                return Color.Black;
            } else { 
                return Color.White;
            }
        }

        private Piece king(Color color)
        {
            foreach (Piece x in piecesInGame(color)) {
                if (x is King) {
                    return x;
                }
            }
            return null;
        }

        public bool checkmate(Color color)
        {
            Piece K = king(color);
            if (K == null) {
                throw new BoardException("There is any " + color + " king on the board!");
            }
            foreach (Piece x in piecesInGame(adversary(color))) {
                bool[,] mat = x.possibleMoves();
                if (mat[K.position.line, K.position.column]) {
                    return true;
                }
            }
            return false;
        }             

        public bool checkmateTest(Color color)
        {
            if (!checkmate(color)) {
                return false;
            }

            foreach (Piece x in piecesInGame(color)) { 
                bool [,] mat = x.possibleMoves();

                for (int i = 0; i < board.lines; i++) { 
                    for (int j = 0; j < board.columns; j++) {
                        if (mat[i, j]) {
                            Position origin = x.position;
                            Position destiny = new Position(i, j);
                            Piece capturedPiece = executeMoves(origin, destiny);
                            bool checkmateTest = checkmate(color);

                            undoMove(origin, destiny, capturedPiece);
                            if (!checkmateTest) {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void putNewPiece(char column, int line, Piece piece)
        {
            board.piecesOnBoard(piece, new ChessPosition(column, line).toPosition());
            pieces.Add(piece);
        }

        private void putPieces()
        {
            putNewPiece('a', 1, new Rock(board, Color.White));
            putNewPiece('b', 1, new Horse(board, Color.White));
            putNewPiece('c', 1, new Bishop(board, Color.White));
            putNewPiece('d', 1, new Queen(board, Color.White));
            putNewPiece('e', 1, new King(board, Color.White, this));
            putNewPiece('f', 1, new Bishop(board, Color.White));
            putNewPiece('g', 1, new Horse(board, Color.White));
            putNewPiece('h', 1, new Rock(board, Color.White));
            putNewPiece('a', 2, new Pawn(board, Color.White, this));
            putNewPiece('b', 2, new Pawn(board, Color.White, this));
            putNewPiece('c', 2, new Pawn(board, Color.White, this));
            putNewPiece('d', 2, new Pawn(board, Color.White, this));
            putNewPiece('e', 2, new Pawn(board, Color.White, this));
            putNewPiece('f', 2, new Pawn(board, Color.White, this));
            putNewPiece('g', 2, new Pawn(board, Color.White, this));
            putNewPiece('h', 2, new Pawn(board, Color.White, this));

            putNewPiece('a', 8, new Rock(board, Color.Black));
            putNewPiece('b', 8, new Horse(board, Color.Black));
            putNewPiece('c', 8, new Bishop(board, Color.Black));
            putNewPiece('d', 8, new Queen(board, Color.Black));
            putNewPiece('e', 8, new King(board, Color.Black, this));
            putNewPiece('f', 8, new Bishop(board, Color.Black));
            putNewPiece('g', 8, new Horse(board, Color.Black));
            putNewPiece('h', 8, new Rock(board, Color.Black));
            putNewPiece('a', 7, new Pawn(board, Color.Black, this));
            putNewPiece('b', 7, new Pawn(board, Color.Black, this));
            putNewPiece('c', 7, new Pawn(board, Color.Black, this));
            putNewPiece('d', 7, new Pawn(board, Color.Black, this));
            putNewPiece('e', 7, new Pawn(board, Color.Black, this));
            putNewPiece('f', 7, new Pawn(board, Color.Black, this));
            putNewPiece('g', 7, new Pawn(board, Color.Black, this));
            putNewPiece('h', 7, new Pawn(board, Color.Black, this));
        }
    }
}

