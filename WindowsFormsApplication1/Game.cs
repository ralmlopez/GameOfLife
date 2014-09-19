using System;
using System.Drawing;
using System.Threading;

namespace WindowsFormsApplication1
{
    public class Game
    {
        public event EventHandler<NewGenerationEventArgs> NewGenerationCreated;

        private int[,] _oldState;
        private int[,] _newState;
        private readonly int _scale;
        private readonly int _delay;

        private bool _running;

        public Game(int[,] seed, int scale, int delay)
        {
            _oldState = seed;
            _scale = scale;
            _delay = delay;
        }

        public void Simulate()
        {
            _running = true;

            while (_running)
            {
                _newState = NextIteration(_oldState);
                var bitmap = Draw(_newState);
                if (NewGenerationCreated != null)
                {
                    NewGenerationCreated(this, new NewGenerationEventArgs { Bitmap = bitmap });
                }
                Thread.Sleep(_delay);
                _oldState = _newState;
            }
        }

        public void Stop()
        {
            _running = false;
        }

        public int[,] NextIteration(int[,] currentIteration)
        {
            var rows = currentIteration.GetUpperBound(0);
            var columns = currentIteration.GetUpperBound(1);

            var nextIteration = new int[rows + 1, columns + 1];

            for (int row = 0; row <= rows; row++)
            {
                for (int column = 0; column <= columns; column++)
                {
                    nextIteration[row, column] = CalculateState(currentIteration, row, column);
                }
            }

            return nextIteration;
        }

        private int CalculateState(int[,] currentIteration, int row, int column)
        {
            var sumOfPreviousRow = GetSumOfRow(currentIteration, row - 1, column);
            var sumOfCurrentRow = GetSumOfRow(currentIteration, row, column);
            var sumOfNextRow = GetSumOfRow(currentIteration, row + 1, column);

            var sum = sumOfPreviousRow + sumOfCurrentRow + sumOfNextRow;
            var state = 0;

            if (sum == 3)
            {
                state = 1;
            }

            if (sum == 4)
            {
                state = currentIteration[row, column];
            }

            return state;
        }

        private int GetSumOfRow(int[,] currentIteration, int rowIndex, int columnIndex)
        {
            var rows = currentIteration.GetUpperBound(0);
            var columns = currentIteration.GetUpperBound(1);

            if (rowIndex < 0 || rowIndex > rows)
            {
                return 0;
            }

            if (columnIndex == columns)
            {
                return currentIteration[rowIndex, columnIndex - 1] + currentIteration[rowIndex, columnIndex];
            }

            if (columnIndex == 0)
            {
                return currentIteration[rowIndex, columnIndex] + currentIteration[rowIndex, columnIndex + 1];
            }

            return currentIteration[rowIndex, columnIndex - 1] +
                currentIteration[rowIndex, columnIndex] +
                currentIteration[rowIndex, columnIndex + 1];
        }

        public Bitmap Draw(int[,] state)
        {
            var rows = state.GetUpperBound(0);
            var columns = state.GetUpperBound(1);

            var bitmap = new Bitmap((columns + 2) * _scale, (rows + 2) * _scale);
            var graphics = Graphics.FromImage(bitmap);

            for (int row = 0; row <= rows; row++)
            {
                for (int column = 0; column <= columns; column++)
                {
                    var value = state[row, column];

                    if (value == 1)
                    {
                        graphics
                            .FillRectangle(Brushes.Black, column * _scale +2, row * _scale +2, _scale-3, _scale-3);
                    }
                    else
                    {
                        graphics.FillRectangle(Brushes.White, column * _scale, row * _scale, _scale, _scale);
                        
                    }
                    graphics.DrawRectangle(Pens.Black, column * _scale, row * _scale, _scale, _scale);
                }
            }

            return bitmap;
        }

    }
}
