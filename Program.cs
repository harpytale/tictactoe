using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tic_tac_toe
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
        }
        private NetworkStream stream;
        private TcpClient connection;
        private Thread outputThread;
        private BinaryWriter writer;
        private BinaryReader reader;
        private Square[] board;
        private Square currentSquare;
        private char point;
        private bool turn;
        private SolidBrush brush;
        private bool done = false;
        private void Client_Load(object sender, EventArgs e)
        {
            board = new Square[4, 4];
            board[0, 0] = new Square(board0Panel, ' ', 0);
            board[0, 1] = new Square(board1Panel, ' ', 1);
            board[0, 2] = new Square(board2Panel, ' ', 2);
            board[0, 3] = new Square(board3Panel, ' ', 3);
            board[1, 0] = new Square(board4Panel, ' ', 4);
            board[1, 1] = new Square(board5Panel, ' ', 5);
            board[1, 2] = new Square(board6Panel, ' ', 6);
            board[1, 3] = new Square(board7Panel, ' ', 7);
            board[2, 0] = new Square(board8Panel, ' ', 8);
            board[2, 1] = new Square(board9Panel, ' ', 9);
            board[2, 2] = new Square(board10Panel, ' ', 10);
            board[2, 3] = new Square(board11Panel, ' ', 11);
            board[3, 0] = new Square(board12Panel, ' ', 12);
            board[3, 1] = new Square(board13Panel, ' ', 13);
            board[3, 2] = new Square(board14Panel, ' ', 14);
            board[3, 3] = new Square(board15Panel, ' ', 15);

            brush = new SolidBrush(Color.Black);
            connection = new TcpClient("127.0.0.1", 50000);
            stream = connection.GetStream();
            writer = new BinaryWriter(stream);
            reader = new BinaryReader(stream);
            outputThread = new Thread(new ThreadStart(Run));
            outputThread.Start();
        }
        private void Client_paint(object sender,
           PaintEventArgs e)
        {
            PaintSquares();
        }
        private void Client_close(object sender,
           FormClosingEventArgs e)
        {
            done = true;
            System.Environment.Exit(System.Environment.ExitCode);
        }
        private delegate void DisplayDelegate(string message);
        private void DisplayMessage(string message)
        {
            if (displayTextBox.InvokeRequired)
            {
                Invoke(new DisplayDelegate(DisplayMessage),
                   new object[] { message });
            }
            else
                displayTextBox.Text += message;
        }
        private delegate void ChangeIdLabelDelegate(string message);
        private void ChangeIdLabel(string label)
        {
            if (idLabel.InvokeRequired)
            {
                Invoke(new ChangeIdLabelDelegate(ChangeIdLabel),
                   new object[] { label });
            }
            else
                idLabel.Text = label;
        }
        public void PaintSquares()
        {
            Graphics g;
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    g = board[row, column].SquarePanel.CreateGraphics();
                    g.DrawString(board[row, column].Mark.ToString(),
                       board0Panel.Font, brush, 10, 8);
                }
            }
        }
        private void square_MouseUp(object sender,
           System.Windows.Forms.MouseEventArgs e)
        {

            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (board[row, column].SquarePanel == sender)
                    {
                        CurrentSquare = board[row, column];
                        SendClickedSquare(board[row, column].Location);
                    }
                }
            }
        }
        public void Run()
        {

            point = reader.ReadChar();
            ChangeIdLabel("Игрок\"" + point + "\"");
            turn = (point == 'X' ? true : false);
            try
            {
                while (!done)
                    ProcessMessage(reader.ReadString());
            }


    }
        public void ProcessMessage(string message)
        {
            if (message == "...")
            {
                currentSquare.Mark = point;
                PaintSquares();
            }
            else if (message == "Неверный ход. Попрбуйте снова")
            {
                DisplayMessage(message + "\r\n");
                turn = true;
            }
            else if (message == "Ход противника.")
            {

                int location = reader.ReadInt32();
                board[location / 3, location % 3].Mark =
                   (point == 'X' ? 'O' : 'X');
                PaintSquares();

                DisplayMessage("Ваш ход.\r\n");
                turn = true;
            }
            else
                DisplayMessage(message + "\r\n");
        }


        public void SendClickedSquare(int location)
        {
            if (turn)
            {
                writer.Write(location);
                turn = false;
            }
        }
        public Square CurrentSquare
        {
            set
            {
                currentSquare = value;
            }
        }
    }



