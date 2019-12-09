using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public delegate void Clean();
    public delegate bool Stop();
    public enum Condition
    {
        EmptyCalm, EmptyWaiting, FilledCalm, FilledWaiting, FilledChozen
    }
    public enum DeskColors
    {
        White=-1, Enabled=0, Black=1
    }
    public struct Coordinates
    {
        public int X;
        public int Y;
        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    public class Cell
    {
        public static void SetOneColor (Cell cell)
        {
            switch (cell.condition)
            {
                case Condition.EmptyCalm:
                case Condition.FilledCalm:
                    cell.cell.BackColor = (cell.PositionOnTheField.X + cell.PositionOnTheField.Y) % 2 == 0 ? Color.Sienna : Color.Bisque;
                    break;
                case Condition.EmptyWaiting: cell.cell.BackColor = Color.SpringGreen;
                    break;
                case Condition.FilledWaiting: cell.cell.BackColor = Color.Red;
                    break;
                default: cell.cell.BackColor = Color.DarkKhaki;
                    break;
            }
        }
        private void SetColor(HashSet<Cell> cells)
        {
            foreach (Cell cell in cells)
            {
                SetOneColor(cell);
            }
        }
        private void Cell_Click(object sender, EventArgs e)
        {
            switch (condition)
            {
                case Condition.EmptyCalm: break;
                case Condition.FilledCalm:
                    {
                        if (this.figure.color==Chesses.CurrentColor)
                        {
                            if (ChosenCell==null)
                            {
                                ChosenCell = this;
                                ChosenCell.condition = Condition.FilledChozen;
                                SetOneColor(this);
                                ChosenCell.figure.Generate_steps(board);
                                ChosenCell.figure.Show();
                            }
                            else
                            {
                                ChosenCell.figure.Reset();
                                ChosenCell.condition = Condition.FilledCalm;
                                SetOneColor(ChosenCell.figure.currentcell);
                                ChosenCell = this;
                                ChosenCell.condition = Condition.FilledChozen;
                                SetOneColor(this);
                                ChosenCell.figure.Generate_steps(board);
                                ChosenCell.figure.Show();
                            }
                        }
                    }
                    break;
                case Condition.FilledChozen:
                    {
                        ChosenCell.figure.Reset();
                        ChosenCell.condition = Condition.FilledCalm;
                        SetOneColor(ChosenCell.figure.currentcell);
                    }
                    break;
                case Condition.EmptyWaiting:
                    {
                        if (ChosenCell.figure is Pawn)
                            ((Pawn)ChosenCell.figure).isFirstStep = false;
                        this.Change();
                        figure.gameSet[Chesses.CurrentColor].label.ForeColor = Color.Black;
                        Chesses.CurrentColor = (DeskColors)((int)Chesses.CurrentColor * (-1));
                        figure.gameSet[Chesses.CurrentColor].label.ForeColor = Color.Red;   
                    }
                    break;
                case Condition.FilledWaiting:
                    {
                        if (this.figure is King)
                        {
                            string st = "Игра окончена победил: ";
                            if (Chesses.CurrentColor == DeskColors.White)
                                st += figure.gameSet.player1.name;
                            else
                                st += figure.gameSet.player2.name;
                            MessageBox.Show(st, "Игра окончена", MessageBoxButtons.OK);
                            Application.Exit();
                        }
                        this.figure.Dispose();
                        this.Change();
                        figure.gameSet[Chesses.CurrentColor].label.ForeColor = Color.Black;
                        Chesses.CurrentColor = (DeskColors)((int)Chesses.CurrentColor * (-1));
                        figure.gameSet[Chesses.CurrentColor].label.ForeColor = Color.Red;
                    }
                    break;
            }
        }
        public Condition condition { get; set; } 
        const int size = 60;
        public static Cell ChosenCell;
        public PictureBox cell;
        readonly Board board;
        public readonly DeskColors color;
        public Coordinates coordinates { get; }
        public Coordinates PositionOnTheField { get; }
        public Chesses figure { get; set; }
        public Cell (Point point, DeskColors color, int x, int y, Form form, Board board)
        {
            condition = Condition.EmptyCalm;
            this.color = color;
            coordinates = new Coordinates(point.X, point.Y);
            PositionOnTheField = new Coordinates(x, y);
            figure = null;
            this.board = board;
            CreateCell(color, point, form);
        }
        public Cell (Point point, DeskColors color, Coordinates coordinates, Form form, Board board)
            :this(point, color, coordinates.X, coordinates.Y, form, board)

        {
        }
        void CreateCell (DeskColors color, Point point, Form form)
        {
            cell = new PictureBox();
            cell.Location = point;
            cell.Width = cell.Height = size;
            cell.BackColor = color == DeskColors.White ? Color.Bisque : Color.Sienna;
            cell.BorderStyle = BorderStyle.FixedSingle;
            cell.Click += Cell_Click;
            form.Controls.Add(cell);
        }
        static Cell()
        {
            ChosenCell = null;
        }
        public void Change ()
        {
            ChosenCell.figure.Reset();
            board[PositionOnTheField].figure = ChosenCell.figure;
            this.figure = ChosenCell.figure;
            this.condition = Condition.FilledCalm;
            this.cell.Image = ChosenCell.cell.Image;
            this.cell.SizeMode = PictureBoxSizeMode.Zoom;
            this.figure.currentcell = this;
            this.figure.current = PositionOnTheField;
            ChosenCell.condition = Condition.EmptyCalm;
            ChosenCell.figure = null; 
            SetOneColor(ChosenCell);
            SetOneColor(this);
            ChosenCell.cell.Image = null;
            ChosenCell = null;
        }
        public void InDiapazon()
        {
            if (figure == null)
                cell.BackColor = Color.SeaGreen;
            else
                cell.BackColor = Color.Red;
        }
     
    }
    public class Board
    {
        
        const int WidthHeight = 8;
        Cell[,] cells;
        Label[,] Horizontal;
        public readonly Form form;
        Label[,] Vertical;
        Coordinates move { get; set; }
        public Board (Form form )
        {
            MakeLabels(form);
            this.form = form;
            Point point = new Point(60, 90);
            cells = new Cell[WidthHeight, WidthHeight];
            for (int i=0; i<WidthHeight; i++)
            {
                point.X = 60;
                for (int j=0; j<WidthHeight; j++)
                {
                    cells[i, j] = new Cell(point, (i + j) % 2 == 0 ? DeskColors.Black : DeskColors.White, new Coordinates(i, j), form, this);
                    point.X += 60;
                }
                point.Y += 60;
            }
        }
    
        void MakeLabels(Form form)
        {
            Point Hloc = new Point(60, 570), Vloc = new Point(0, 510);
            Horizontal = new Label[1, 8];
            Vertical = new Label[8, 1];
            for (int i=0; i<8; i++)
            {
                string H = new string((char)(65 + i), 1);
                string V = new string((char)(49 + i), 1);

                Horizontal[0, i] = new Label();
                Vertical[i, 0] = new Label();

                Horizontal[0, i].Text =H;
                Vertical[i, 0].Text = V;

                Horizontal[0, i].Location = Hloc;
                Vertical[i, 0].Location = Vloc;

                Vertical[i, 0].Width = Vertical[i, 0].Height = Horizontal[0, i].Height = Horizontal[0, i].Width = 60;
                
                Vloc.Y -= 60;
                Hloc.X += 60;

                Vertical[i, 0].Font= new Font("Arial", 20f, FontStyle.Regular);
                Horizontal[0,i].Font= new Font("Arial", 20f, FontStyle.Regular);

                Vertical[i, 0].TextAlign=Horizontal[0,i].TextAlign = ContentAlignment.MiddleCenter;

                form.Controls.Add(Vertical[i, 0]);
                form.Controls.Add(Horizontal[0, i]);
            }
        }
        public Cell this [int i, int j]
        {
            get
            {
                return cells[i, j];
            }
        }
        public Cell this [Coordinates coordinates]
        {
            get
            {
                return this[coordinates.X, coordinates.Y];
            }
        }
        public static bool IsCorrectCoordinates (int x, int y)
        {
            if (x >= 0 && x < WidthHeight && y >= 0 && y < WidthHeight)
                return true;
            else
                return false;
        }
        public static bool IsCorrectCoordinates(Coordinates coordinates)
        {
            return IsCorrectCoordinates(coordinates.X, coordinates.Y);
        }


    }
    public abstract class Chesses
    {
        public static DeskColors CurrentColor;
        public Cell currentcell;
        public Board  board;
        public readonly GameSet gameSet;
        public DeskColors color;
        public Coordinates current { get; set; }
        protected HashSet<Cell> PossibleSteps;

        public abstract void Generate_steps(Board board);
        public Chesses (DeskColors color, Board board, Coordinates coordinates, GameSet gameSet)
        {
            this.gameSet = gameSet;
            CurrentColor = DeskColors.White;
            PossibleSteps = new HashSet<Cell>();
            this.color = color;
            this.board = board;
            currentcell = board[coordinates];
            board[coordinates].figure = this;
            current = coordinates;
            currentcell.condition = Condition.FilledCalm;
            PossibleSteps.Clear();
            currentcell.cell.SizeMode = PictureBoxSizeMode.StretchImage;
        }
        public void Show ()
        {
            foreach (Cell cell in PossibleSteps)
                cell.InDiapazon();
        }
        public void Reset()
        {
            foreach (Cell cell in PossibleSteps)
            {
                if (cell.figure == null)
                    cell.condition = Condition.EmptyCalm;
                else
                    cell.condition = Condition.FilledCalm;
                Cell.SetOneColor(cell);
            }
        }
        public void Dispose()
        {
            color = DeskColors.Enabled;
            board = null;
            PossibleSteps.Clear();
            current = new Coordinates(-1, -1);
            currentcell.condition = Condition.EmptyCalm;
            currentcell.cell.Image = null;
            currentcell.figure = null;
            currentcell = null;
                     
        }
        public HashSet<Cell> GetSet()
        {
            return PossibleSteps;
        }
    }
    public class Pawn : Chesses
    {
        public bool isFirstStep;
        public override void Generate_steps( Board board)
        {

            PossibleSteps.Clear();
           
            if (Board.IsCorrectCoordinates(current.X+(int)color, current.Y))
            {
                if (board[current.X + (int)color, current.Y ].figure==null)
                {
                    PossibleSteps.Add(board[current.X + (int)color, current.Y]);
                    board[current.X + (int)color, current.Y].condition = Condition.EmptyWaiting;
                    if (isFirstStep)
                    {
                        if (board[current.X + 2 * (int)color, current.Y].figure == null)
                        {
                            PossibleSteps.Add(board[current.X + 2 * (int)color, current.Y]);
                            board[current.X + 2 * (int)color, current.Y].condition = Condition.EmptyWaiting;
                        }
                    }
                }
            }
            if (Board.IsCorrectCoordinates(current.X + (int)color, current.Y-1))
            {
                if (board[current.X + (int)color, current.Y-1].figure != null 
                    && board[current.X + (int)color, current.Y - 1].figure.color!=color)
                {
                    PossibleSteps.Add(board[current.X + (int)color, current.Y - 1]);
                    board[current.X + (int)color, current.Y - 1].condition = Condition.FilledWaiting;
                }
                    
            }
            if (Board.IsCorrectCoordinates(current.X + (int)color, current.Y + 1))
            {
                if (board[current.X + (int)color, current.Y + 1].figure != null 
                    && board[current.X + (int)color, current.Y + 1].figure.color!=color)
                {
                    PossibleSteps.Add(board[current.X + (int)color, current.Y + 1]);
                    board[current.X + (int)color, current.Y + 1].condition = Condition.FilledWaiting;
                }
            }
        }
        public Pawn(DeskColors color, Coordinates position, Board board, Form form, GameSet gameSet) : 
            base(color, board, position, gameSet)
        {
            isFirstStep = true;
            currentcell.cell.Image = color == DeskColors.White ? Properties.Resources.wP : Properties.Resources.bP;
        }
    }
    public class Rook: Chesses
    {
        public Rook (DeskColors color, Coordinates position, Board board, Form form, GameSet gameSet):
            base(color, board, position, gameSet)
        {
            currentcell.cell.Image = color == DeskColors.White ? Properties.Resources.wR : Properties.Resources.bR;
        }
        public override void Generate_steps(Board board)
        {
            PossibleSteps.Clear();
            Generate(true, PossibleSteps, board, current);
            Generate(false, PossibleSteps, board, current);
            Show();
        }
        public static void Generate(bool isver, HashSet<Cell> cells, Board board, Coordinates current)
        {
            bool ok=true;
            Coordinates move = new Coordinates(current.X, current.Y);
            DeskColors desk = board[current].figure.color;
            if (isver)
            {
                do
                {
                    current.Y += (int)desk;
                    ok = Board.IsCorrectCoordinates(current);
                    if (ok)
                        if(board[current].figure==null)
                        {
                            cells.Add(board[current]);
                            board[current].condition = Condition.EmptyWaiting;
                        }
                        else
                        {
                            if (board[current].figure.color != desk)
                            {
                                cells.Add(board[current]);
                                board[current].condition = Condition.FilledWaiting;
                            }
                                
                            ok = false;
                        }
                } while (ok);
                ok = true;
                do
                {
                    move.Y -= (int)desk;
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    ok = Board.IsCorrectCoordinates(move);
                    if (ok)
                        if (board[move].figure==null)
                        {
                            cells.Add(board[move]);
                            board[move].condition = Condition.EmptyWaiting;
                        }
                           
                        else
                        {
                            if (board[move].figure.color != desk)
                            {
                                cells.Add(board[move]);
                                board[move].condition = Condition.FilledWaiting;
                            }       
                            ok = false;
                        }
                } while (ok);
            }
            else
            {
                do
                {
                    current.X += (int)desk;
                    ok = Board.IsCorrectCoordinates(current);
                    if (ok)
                        if (board[current].figure==null)
                        {
                            cells.Add(board[current]);
                            board[current].condition = Condition.EmptyWaiting;
                        }
                            
                        else
                        {
                            if (board[current].figure.color != desk)
                            {
                                cells.Add(board[current]);
                                board[current].condition = Condition.FilledWaiting;
                            }
                            ok = false;
                        }
                } while (ok);
                ok = true;
                do
                {
                    move.X -= (int)desk;
                    ok = Board.IsCorrectCoordinates(move);
                    if (ok)
                        if (board[move].figure==null)
                        {
                            cells.Add(board[move]);
                            board[move].condition = Condition.EmptyWaiting;
                        }
                        else
                        {
                            if (board[move].figure.color != desk)
                            {
                                cells.Add(board[move]);
                                board[move].condition = Condition.FilledWaiting;
                            }
                                
                            ok = false;
                        }
                } while (ok);
            }
        }
    }
    public class Knight:Chesses
    { 
        public Knight(DeskColors color, Coordinates position, Board board, Form form, GameSet gameSet) 
            : base(color, board, position, gameSet)
        {
            currentcell.cell.Image = color == DeskColors.White ? Properties.Resources.wN : Properties.Resources.bN;
        }
        public override void Generate_steps(Board board)
        {
            PossibleSteps.Clear();
            int kv = -1, kh = -1;
            for (int i=1; i<=2; i++)
            {
                for (int j=1; j<=2; j++)
                {
                    bool ok = Board.IsCorrectCoordinates(current.X + 2 * kv*(int)color, current.Y + kh*(int)color);
                    if (ok)
                    {
                        if (board[current.X + 2 * kv*(int)color, current.Y + kh*(int)color].figure == null)
                        {
                            PossibleSteps.Add(board[current.X + 2 * kv*(int)color, current.Y + kh*(int)color]);
                            board[current.X + 2 * kv*(int)color, current.Y + kh*(int)color].condition = Condition.EmptyWaiting;
                        }
                        else
                            if (board[current.X + 2 * kv*(int)color, current.Y + kh*(int)color].figure.color != Chesses.CurrentColor)
                            {
                                PossibleSteps.Add(board[current.X + 2 * kv*(int)color, current.Y + kh*(int)color]);
                                board[current.X + 2 * kv*(int)color, current.Y + kh*(int)color].condition =Condition.FilledWaiting;
                            }
                    }
                    ok= Board.IsCorrectCoordinates(current.X + kv * (int)color, current.Y + kh * 2*(int)color);
                    if (ok)
                    {
                        if (board[current.X +  kv * (int)color, current.Y + kh * 2*(int)color].figure == null)
                        {
                            PossibleSteps.Add(board[current.X +  kv * (int)color, current.Y + kh *2* (int)color]);
                            board[current.X +  kv * (int)color, current.Y + kh *2* (int)color].condition = Condition.EmptyWaiting;
                        }
                        else
                            if (board[current.X +  kv * (int)color, current.Y + kh * 2* (int)color].figure.color != Chesses.CurrentColor)
                        {
                            PossibleSteps.Add(board[current.X +  kv * (int)color, current.Y + kh*2 * (int)color]);
                            board[current.X +  kv * (int)color, current.Y + kh * 2*(int)color].condition = Condition.FilledWaiting;
                        }
                    }
                    kh *= -1;
                }
                kv *= -1;
            }
        }
    }
    public class Bishop: Chesses
    {
        public Bishop(DeskColors color, Coordinates position, Board board, Form form, GameSet gameSet) 
            : base(color, board, position, gameSet)
        {
            currentcell.cell.Image = color == DeskColors.White ? Properties.Resources.wB : Properties.Resources.bB;
        }
        public override void Generate_steps(Board board)
        {
            PossibleSteps.Clear();
            Generate(true, PossibleSteps, board, current);
            Generate(false, PossibleSteps, board, current);
        }
        public static void Generate (bool ispos, HashSet<Cell> cells, Board board, Coordinates current)
        {
            bool ok = true;
            DeskColors desk = board[current].figure.color;
            Coordinates move =new Coordinates( current.X, current.Y);
            if (ispos)
            {
                do
                {
                    current.X += (int)desk;
                    current.Y += (int)desk;
                    if (Board.IsCorrectCoordinates(current))
                    {
                        if (board[current].figure == null)
                        {
                            cells.Add(board[current]);
                            board[current].condition = Condition.EmptyWaiting;
                        }
                            
                        else
                        {
                            if (board[current].figure.color != desk)
                            {
                                cells.Add(board[current]);
                                board[current].condition = Condition.FilledWaiting;
                            }
                            ok = false;
                        }
                    }
                    else
                        ok = false;
                } while (ok);
                ok = true;
                do
                {
                    move.X -= (int)desk;
                    move.Y -= (int)desk;
                    if (Board.IsCorrectCoordinates(move))
                    {
                        if (board[move].figure == null)
                        {
                            cells.Add(board[move]);
                            board[move].condition = Condition.EmptyWaiting;
                        }     
                        else
                        {
                            if (board[move].figure.color != desk)
                            {
                                cells.Add(board[move]);
                                board[move].condition = Condition.FilledWaiting;
                            }
                            ok = false;
                        }
                    }
                    else
                        ok = false;
                } while (ok);
                    
            }
            else
            {
                do
                {
                    current.X -= (int)desk;
                    current.Y += (int)desk;
                    if (Board.IsCorrectCoordinates(current))
                    {
                        if (board[current].figure == null)
                        {
                            cells.Add(board[current]);
                            board[current].condition = Condition.EmptyWaiting;
                        }
                            
                        else
                        {
                            if (board[current].figure.color != desk)
                            {
                                cells.Add(board[current]);
                                board[current].condition = Condition.FilledWaiting;
                            }
                                
                            ok = false;
                        }
                    }
                    else
                        ok = false;
                } while (ok);
                ok = true;
                do
                {
                    move.X += (int)desk;
                    move.Y -= (int)desk;
                    if (Board.IsCorrectCoordinates(move))
                    {
                        if (board[move].figure == null)
                        {
                            cells.Add(board[move]);
                            board[move].condition = Condition.EmptyWaiting;
                        }
                        else
                        {
                            if (board[move].figure.color != desk)
                            {
                                cells.Add(board[move]);
                                board[move].condition = Condition.FilledWaiting;
                            }
                            ok = false;
                        }
                    }
                    else
                        ok = false;
                } while (ok);
            }
        }
    }
    public class Queen: Chesses
    {
        public Queen(DeskColors color, Coordinates position, Board board, Form form, GameSet gameSet) 
            : base(color, board, position, gameSet)
        {
            currentcell.cell.Image = color == DeskColors.White ? Properties.Resources.wQ : Properties.Resources.bQ;
        }
        public override void Generate_steps(Board board)
        {
            PossibleSteps.Clear();
            Rook.Generate(true, PossibleSteps, board, current);
            Rook.Generate(false, PossibleSteps, board, current);
            Bishop.Generate(true, PossibleSteps, board, current);
            Bishop.Generate(false, PossibleSteps, board, current);
        }
    }
    public class King : Chesses
    {
        public King(DeskColors color, Coordinates position, Board board, Form form, GameSet gameSet) 
            : base(color, board, position, gameSet)
        {
            currentcell.cell.Image = color == DeskColors.White ? Properties.Resources.wK : Properties.Resources.bK;
        }
        public override void Generate_steps(Board board)
        {
            PossibleSteps.Clear();
            for (int i=-1; i<2; i++)
            {
                for (int j=-1; j<2; j++)
                {
                    if (Board.IsCorrectCoordinates(current.X+i, current.Y+j))
                    {
                        if (board[current.X + i, current.Y + j].figure == null)
                        {
                            PossibleSteps.Add(board[current.X + i, current.Y + j]);
                            board[current.X + i, current.Y + j].condition = Condition.EmptyWaiting;
                        }
                        else
                            if (board[current.X + i, current.Y + j].figure.color!=color)
                            {
                                PossibleSteps.Add(board[current.X + i, current.Y + j]);
                                board[current.X + i, current.Y + j].condition = Condition.FilledWaiting;
                            }
                    }   
                }
            }
            
        }
        public void Generate_for_end (Board board)
        {
            PossibleSteps.Clear();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (Board.IsCorrectCoordinates(current.X + i, current.Y + j))
                    {
                        if (board[current.X + i, current.Y + j].figure == null)
                        {
                            PossibleSteps.Add(board[current.X + i, current.Y + j]);
                            board[current.X + i, current.Y + j].condition = Condition.EmptyWaiting;
                        }
                    }
                }
            }
        }
    }
    public class SetOfFigures
    {
        readonly DeskColors color;
        Pawn[] pawns;
        Rook[] rooks;
        Knight[] knights;
        Bishop[] bishops;
        Queen queen;
        GameSet gameSet;
        public readonly Board board;
        public King king;
        void initPawns (int line, Board board, Form form)
        {
            for (int i=0; i<8; i++)
                pawns[i] = new Pawn(color, new Coordinates(line, i), board, form, gameSet);
        }
        void initRooks (int line, Board board, Form form)
        {
            rooks[0] = new Rook(color, new Coordinates(line, 0), board, form, gameSet);
            rooks[1] = new Rook(color, new Coordinates(line, 7), board, form, gameSet);
        }
        void initKnights(int line, Board board, Form form)
        {
            knights[0] = new Knight(color, new Coordinates(line, 1), board, form, gameSet);
            knights[1] = new Knight(color, new Coordinates(line, 6), board, form, gameSet);
        }
        void initBishops(int line, Board board, Form form)
        {
            bishops[0] = new Bishop(color, new Coordinates(line, 2), board, form, gameSet);
            bishops[1] = new Bishop(color, new Coordinates(line, 5), board, form, gameSet);
        }
        public SetOfFigures (DeskColors color, Board board, Form form, GameSet gameSet)
        {
            this.board = board;
            this.color = color;
            this.gameSet = gameSet;
            pawns = new Pawn[8];
            rooks= new Rook[2];
            knights = new Knight[2];
            bishops = new Bishop[2];
            initPawns(color == DeskColors.Black ? 1 : 6, board, form);
            initRooks(color == DeskColors.Black ? 0 : 7, board, form);
            initKnights(color == DeskColors.Black ? 0 : 7, board, form);
            initBishops(color == DeskColors.Black ? 0 : 7, board, form);
            queen = new Queen(color, color == DeskColors.Black ? new Coordinates(0, 4) : new Coordinates(7, 4), board, form, gameSet);
            king = new King(color, color == DeskColors.Black ? new Coordinates(0, 3) : new Coordinates(7, 3), board, form, gameSet);
        }

    }
    public class Player
    {
        public readonly DeskColors color;
        public readonly string name;
        public bool isallowed;
        public Label label;
        public SetOfFigures set;
        public readonly Board board;
        public Player(string st, DeskColors color, Board board, Form form, GameSet gameSet)
        {
            this.board = board;
            this.color = color;
            name = st;
            label = new Label();
            label.Text = st;
            label.Width = 210;
            label.Height = 60;
            if (color == DeskColors.White)
            {
                label.Location = new Point(570, 510);
                label.ForeColor = Color.Red;
            }
            else
            {
                label.Location = new Point(570, 150);
                label.ForeColor = Color.Black;
            }
                
            label.Font = new Font("Arial", 20f, FontStyle.Regular);
            form.Controls.Add(label);
            set = new SetOfFigures(this.color, board, form, gameSet);
            isallowed = this.color == DeskColors.White ? true : false;
        }
      
    }
    public class GameSet
    {
        Board board;
        public Player player1;
        public Player player2;
        public GameSet(string s1, string s2, Form form)
        {
            board = new Board(form);
            player1 = new Player(s1, DeskColors.White, board, form, this);
            player2 = new Player(s2, DeskColors.Black, board, form, this);
        }
        public Player this[DeskColors color]
        {
            get
            {
                if (color == DeskColors.White)
                    return player1;
                else
                    return player2;
            }
        }

    }
}
