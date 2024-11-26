// Connect to the TaskQueue
using System;
using System.Drawing;

namespace DistributedQueue.Common
{
    public class ConsoleHelper
    {
        public ConsoleAdapter ConsoleAdapter { get; set; }

        public ConsoleHelper()
        {
            ConsoleAdapter = new ConsoleAdapter();
            ConsoleAdapter.CursorVisible = false;
            ConsoleAdapter.Size = new Size() { Width = Console.WindowWidth, Height = Console.WindowHeight };
        }

        public void Clear()
        {
            RenderBackgroundBox(new Point(0, 0), ConsoleAdapter.Size, ConsoleAdapter.BackgroundColor);
        }

        public void RenderBackgroundBox(Point point, Size size, ConsoleColor backgroundColor)
        {
            ConsoleColor previousBackground = ConsoleAdapter.BackgroundColor;
            ConsoleAdapter.BackgroundColor = backgroundColor;
            int lineLength = size.Width;

            var str = new string(' ', lineLength);

            for (int y = point.Y; y < point.Y + size.Height; y++)
            {
                ConsoleAdapter.MoveCursor(point.X, y);
                ConsoleAdapter.Write(str);
            }
            ConsoleAdapter.MoveCursor(point.X, point.Y);
            ConsoleAdapter.BackgroundColor = previousBackground;
        }

        public ConsoleHelper RenderText(string text, ConsoleColor foregroundColor = ConsoleColor.Black, 
            ConsoleColor? backgroundColor = null)
        {
            ConsoleColor previousBackground = ConsoleAdapter.BackgroundColor;
            ConsoleAdapter.BackgroundColor = backgroundColor ?? ConsoleAdapter.BackgroundColor;

            ConsoleColor previousForeground = ConsoleAdapter.ForegroundColor;
            ConsoleAdapter.ForegroundColor = foregroundColor;

            ConsoleAdapter.Write(text);

            ConsoleAdapter.BackgroundColor = previousBackground;
            ConsoleAdapter.ForegroundColor = previousForeground;

            return this;
        }

        public ConsoleHelper WriteLine(string v, ConsoleColor foregroundColor = ConsoleColor.Gray,
            ConsoleColor? backgroundColor = null)
        {
            RenderText(v, foregroundColor, backgroundColor);
            ConsoleAdapter.WriteLine();

            return this;
        }

        public ConsoleHelper Write(string v, ConsoleColor foregroundColor = ConsoleColor.Gray,
            ConsoleColor? backgroundColor = null)
        {
            RenderText(v, foregroundColor, backgroundColor);

            return this;
        }
    }
}