using GameOfLife.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    internal class Game
    {
        public int size;
        public float cellSize;
        public List<bool> cellStates;
        public bool statesUpdated = true;

        // Setup vertex data
        public List<Vector3> gridPoints = new();
        public List<Vector3> squarePoints = new();

        private VAO gridVao;
        private VAO squaresVao;

        private VBO gridVbo;
        private VBO squaresVbo;

        public Game(int size)
        {
            this.size = size;
            cellSize = 2f / size;
            cellStates = new List<bool>();

            for(int i = 0; i < size * size; i++)
            {
                Random rnd = new Random();

                cellStates.Add(rnd.NextSingle() > .66f);
            }
            

            GenerateGridLines();


            gridVao = new();
            gridVbo = new(gridPoints);
            gridVao.LinkToVBO(0, 3, gridVbo);


            squaresVao = new();
            squaresVbo = new(squarePoints);
            GenerateCellTris();  // Ensure initial cell triangles are generated
            squaresVao.LinkToVBO(0, 3, squaresVbo);


        }

        private void GenerateGridLines()
        {
            float pos = -1f;
            // Define horizontal lines
            for (int i = 0; i < size + 1; i++)
            {
                //Horizontal line
                gridPoints.Add(new Vector3(-1f, pos, 0f));
                gridPoints.Add(new Vector3(1f, pos, 0f));

                //Vertical line
                gridPoints.Add(new Vector3(pos, -1f, 0f));
                gridPoints.Add(new Vector3(pos, 1f, 0f));

                pos += cellSize;
            }

        }

        private void GenerateCellTris()
        {
            squarePoints.Clear();
            for(int i = 0; i < cellStates.Count; i++)
            {
                if (cellStates[i])
                {
                    cellStates[i] = true;
                    int xPos = i % size;
                    int yPos = i / size;

                    float left = -1.0f + xPos * cellSize;
                    float right = left + cellSize;
                    float top = 1.0f - yPos * cellSize;
                    float bottom = top - cellSize;

                    // Define two triangles for each square
                    // Triangle 1
                    squarePoints.Add(new Vector3(left, top, 0.0f));    // top-left
                    squarePoints.Add(new Vector3(right, top, 0.0f));   // top-right
                    squarePoints.Add(new Vector3(left, bottom, 0.0f)); // bottom-left

                    // Triangle 2
                    squarePoints.Add(new Vector3(left, bottom, 0.0f)); // bottom-left
                    squarePoints.Add(new Vector3(right, top, 0.0f));   // top-right
                    squarePoints.Add(new Vector3(right, bottom, 0.0f));// bottom-right
                }
            }
            squaresVbo.Update(squarePoints);



        }

        public void DrawGrid()
        {
            gridVao.Bind();
            GL.DrawArrays(PrimitiveType.Lines, 0, gridPoints.Count);
            gridVao.Unbind();
        }

        public void DrawCells()
        {
            //if (!statesUpdated)
            //{
            //    return;
            //}

            GenerateCellTris();
            PrintGrid();

            squaresVao.Bind();
            GL.DrawArrays(PrimitiveType.Triangles, 0, squarePoints.Count);
            squaresVao.Unbind();

            //statesUpdated = false;
        }

        public void OnClick(Vector2 worldPos)
        {
            // Convert the world position to grid coordinates
            int col = (int)((worldPos.X + 1.0f) / cellSize);
            int row = (int)((1.0f - worldPos.Y) / cellSize);

            // Ensure the coordinates are within bounds
            if (col >= 0 && col < size && row >= 0 && row < size)
            {
                // Calculate the index based on row and column
                int index = row * size + col;
                cellStates[index] = !cellStates[index];
                statesUpdated = true;
                Console.WriteLine($"Grid cell index: {index}");
                DrawCells();
            }
            else
            {
                Console.WriteLine("Click is out of grid bounds");
            }
        }


        public void Unload()
        {
            gridVao.Unbind();
            gridVao.Delete();
            gridVbo.Unbind();
            gridVbo.Delete();
        }

        private void PrintGrid()
        {
            for (int i = 0; i < size; i++)
            {
                for (int ii = 0; ii < size; ii++)
                {
                    var index = (i * size) + ii;
                    Console.Write(cellStates[index] ? "1 " : "0 ");
                }
                Console.WriteLine();
            }
        }

    }
}
