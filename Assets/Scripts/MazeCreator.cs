using System;
using System.Collections;
using System.Collections.Generic;


public class MazeCreator
{
    // 2�����z��̖��H���
    private int[,] Maze;
    private int Width { get; set; }
    private int Height { get; set; }

    // ���@��J�n�����W
    private List<Cell> StartCells;

    // �R���X�g���N�^
    public MazeCreator(int width, int height)
    {
        // 5�����̃T�C�Y������ł͐����ł��Ȃ�
        if (width < 5 || height < 5) throw new ArgumentOutOfRangeException();
        if (width % 2 == 0) width++;
        if (height % 2 == 0) height++;

        // ���H����������
        this.Width = width;
        this.Height = height;
        Maze = new int[width, height];
        StartCells = new List<Cell>();
    }

    // ��������
    public int[,] CreateMaze()
    {
        // �S�Ă�ǂŖ��߂�
        // ���@��J�n���(x,y�Ƃ��ɋ���)���W��ێ����Ă���
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (x == 0 || y == 0 || x == this.Width - 1 || y == this.Height - 1)
                {
                    Maze[x, y] = Path;  // �O�ǂ͔���̈גʘH�ɂ��Ă���(�Ō�ɖ߂�)
                }
                else
                {
                    Maze[x, y] = Wall;
                }
            }
        }

        // ���@��J�n
        Dig(1, 1);

        // �O�ǂ�߂�
        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                if (x == 0 || y == 0 || x == this.Width - 1 || y == this.Height - 1)
                {
                    Maze[x, y] = Wall;
                }
            }
        }
        return Maze;
    }

    // ���W(x, y)�Ɍ����@��
    private void Dig(int x, int y)
    {
        // �w����W����@��Ȃ��Ȃ�܂Ŗx�葱����
        var rnd = new Random();
        while (true)
        {
            // �@��i�߂邱�Ƃ��ł�������̃��X�g���쐬
            var directions = new List<Direction>();
            if (this.Maze[x, y - 1] == Wall && this.Maze[x, y - 2] == Wall)
                directions.Add(Direction.Up);
            if (this.Maze[x + 1, y] == Wall && this.Maze[x + 2, y] == Wall)
                directions.Add(Direction.Right);
            if (this.Maze[x, y + 1] == Wall && this.Maze[x, y + 2] == Wall)
                directions.Add(Direction.Down);
            if (this.Maze[x - 1, y] == Wall && this.Maze[x - 2, y] == Wall)
                directions.Add(Direction.Left);

            // �@��i�߂��Ȃ��ꍇ�A���[�v�𔲂���
            if (directions.Count == 0) break;

            // �w����W��ʘH�Ƃ����@������W����폜
            SetPath(x, y);
            // �@��i�߂���ꍇ�̓����_���ɕ��������߂Č@��i�߂�
            var dirIndex = rnd.Next(directions.Count);
            // ���܂��������ɐ�2�}�X����ʘH�Ƃ���
            switch (directions[dirIndex])
            {
                case Direction.Up:
                    SetPath(x, --y);
                    SetPath(x, --y);
                    break;
                case Direction.Right:
                    SetPath(++x, y);
                    SetPath(++x, y);
                    break;
                case Direction.Down:
                    SetPath(x, ++y);
                    SetPath(x, ++y);
                    break;
                case Direction.Left:
                    SetPath(--x, y);
                    SetPath(--x, y);
                    break;
            }
        }

        // �ǂ��ɂ��@��i�߂��Ȃ��ꍇ�A���@��J�n�����W����@��Ȃ���
        // �����W�����݂��Ȃ��Ƃ��A���@�芮��
        var cell = GetStartCell();
        if (cell != null)
        {
            Dig(cell.X, cell.Y);
        }
    }

    // ���W��ʘH�Ƃ���(���@��J�n���W���̏ꍇ�͕ێ�)
    private void SetPath(int x, int y)
    {
        this.Maze[x, y] = Path;
        if (x % 2 == 1 && y % 2 == 1)
        {
            // ���@������W
            StartCells.Add(new Cell() { X = x, Y = y });
        }
    }

    // ���@��J�n�ʒu�������_���Ɏ擾����
    private Cell GetStartCell()
    {
        if (StartCells.Count == 0) return null;

        // �����_���ɊJ�n���W���擾����
        var rnd = new Random();
        var index = rnd.Next(StartCells.Count);
        var cell = StartCells[index];
        StartCells.RemoveAt(index);

        return cell;
    }

    // �f�o�b�O�p����
    public static void DebugPrint(int[,] maze)
    {
        Console.WriteLine($"Width: {maze.GetLength(0)}");
        Console.WriteLine($"Height: {maze.GetLength(1)}");
        for (int y = 0; y < maze.GetLength(1); y++)
        {
            for (int x = 0; x < maze.GetLength(0); x++)
            {
                Console.Write(maze[x, y] == Wall ? "��" : "�@");
            }
            Console.WriteLine();
        }
    }

    // �ʘH�E�Ǐ��
    const int Path = 0;
    const int Wall = 1;

    // �Z�����
    private class Cell
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    // ����
    private enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

}

