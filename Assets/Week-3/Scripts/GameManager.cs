using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Battleship
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private int[,] grid = new int[,]
        {
            //Top left is [0,0]
            {1, 1, 0, 0, 1 },
            {0, 0, 0, 0, 0 },
            {0, 0, 1, 0, 1 },
            {1, 0, 1, 0, 0 },
            {1, 0, 1, 0 ,1 }
            //Bottom right is [4,4]
        };

        // Start is called before the first frame update
        void Start()
        {
        
        }

     // Update is called once per frame
        void Update()
        {
        
        }

        //Represents where the player has fired
        private bool[,] hits;

        //Total rows and columns we have
        private int nRows;
        private int nCols;
        //Current row/column we are on
        private int row;
        private int col;
        //Correctly hit sips
        private int score;
        //Total time game has been running
        private int time;

        //Parent of all cells
        [SerializeField] Transform gridRoot;
        //Template used to populate the grid
        [SerializeField] GameObject cellPrefab;
        [SerializeField] GameObject winLabel;
        [SerializeField] TextMeshProUGUI timeLabel;
        [SerializeField] TextMeshProUGUI scoreLabel;

        private void Awake()
        {
            //Initalize rows/cols to help us with our operations
            nRows = grid.GetLength(0);
            nCols = grid.GetLength(1);
            //Create identical 2D array to grid that is of the type bool instead of int
            hits = new bool[nRows, nCols];

            //Populate the grid using a loop
            //Needs to execute as many times to fill up the grid
            //Can figure that out by calculating rows * cols
            for(int i = 0; i < nRows * nCols; i++)
            {
                //Create an instance of the prefab and child it to the gridRoot
                Instantiate(cellPrefab, gridRoot);
            }
            SelectCurrentCell();
            InvokeRepeating("IncrementTime", 1f, 1f);

            Restart();
        }
        public void Restart()
        {
            //Unselect current cell
            UnselectCurrentCell();
            //Resets rol and col
            row = 0;
            col = 0;

            //Resets hits to false
            hits = new bool[nRows, nCols];

            time = 0;
            score = 0;

            //Reset UI elements
            winLabel.SetActive(false);
            timeLabel.text = "0:00";
            scoreLabel.text = "Score: 0";

            //Turn off Hit/Miss objects on each cell
            for(int i = 0; i < gridRoot.childCount; i++)
            {
                Transform cell = gridRoot.GetChild(i);
                Transform hit = cell.Find("Hit");
                Transform miss = cell.Find("Miss");
                hit.gameObject.SetActive(false);
                miss.gameObject.SetActive(false);
            }
            //Randomize the ships
            RandomizeShips();
        }
        //Function to randomly change ship potions
        void RandomizeShips()
        {
            for (int row = 0; row < nRows; row++)
            {
                for(int col = 0; col < nCols; col++)
                {
                    //Random roll between 0 to 10
                    int randomRoll = Random.Range(0, 11);
                    //If the number is less than 5, make it a 1
                    grid[row, col] = (randomRoll > 5) ? 1 : 0;
                }
            }
        }

        Transform GetCurrentCell()
        {
            //Child Index of the cell that is part of the grid
            int index = (row * nCols) + col;
            //Return the child by index
            return gridRoot.GetChild(index);
        }
        void SelectCurrentCell()
        {
            //Get the current cell
            Transform cell = GetCurrentCell();
            //Set the "Cursor" image on
            Transform cursor = cell.Find("Cursor");
            cursor.gameObject.SetActive(true);
        }
        void UnselectCurrentCell()
        {
            //Get the current cell
            Transform cell = GetCurrentCell();
            //Set the "Cursor" image off
            Transform cursor = cell.Find("Cursor");
            cursor.gameObject.SetActive(false);
        }
        public void MoveHorizontal(int amt)
        {
            //need to unselect a previous one for new one
            UnselectCurrentCell();

            //Update the column
            col += amt;
            //Makes sure the column stays within bounds of grid
            col = Mathf.Clamp(col, 0, nCols - 1);

            //Select the new cell
            SelectCurrentCell();
        }
        public void MoveVertical(int amt)
        {
            UnselectCurrentCell();
            row += amt;
            row = Mathf.Clamp(row, 0, nRows - 1);
            SelectCurrentCell();
        }
        void ShowHit()
        {
            //Get current cell
            Transform cell = GetCurrentCell();
            //Set "Hit" image on
            Transform hit = cell.Find("Hit");
            hit.gameObject.SetActive(true);
        }
        void ShowMiss()
        {
            Transform cell = GetCurrentCell();
            Transform miss = cell.Find("Miss");
            miss.gameObject.SetActive(true);
        }
        void IncrementScore()
        {
            //Add 1 to the score
            score++;
            //Udate the score label with current score
            scoreLabel.text = string.Format("Score: {0}, score");
        }
        public void Fire()
        {
            //Checks if the cell in the hits data is true or false
            //If its true that means we already fired a shot in the current cell
            //and shouldn't do anything
            if (hits[row, col]) return;

            //Marks this cell as fired at
            hits[row, col] = true;

            //If this cell is a ship
            if (grid[row, col] == 1)
            {
                //Hit it
                //Increment score
                ShowHit();
                IncrementScore();
            }
            //If the cell is open water
            else
            {
                //Show miss
                ShowMiss();
            }
       
        }
        void TryEndGame()
        {
            //Check every row
            for(int row = 0; row < nRows; row++)
            {
                //and check every column
                for(int col = 0; col <nCols; col++)
                {
                    //if a cell is not a ship then ignore
                    if (grid[row, col] == 0) continue;
                    //if cell is a ship and hasn't been scored
                    //then simple return because we haven't finished the game
                    if (hits[nRows, col] == false) return;
                }
            }
            //if loop successfully completed
            //ships have been destroyed and show winLabel
            winLabel.SetActive(true);
            //Stops timer
            CancelInvoke("IncrementTime");
        }
        void IncrementTime()
        {
            //Add 1 to the time
            time++;
            //Update time with current
            //Format it mm:ss where m is mins and s is secs
            //ss always 2 digits
            //mm display as necessary
            timeLabel.text = string.Format("{0}:{1}", time / 60, (time % 60).ToString("00"));
        }
    }
}

