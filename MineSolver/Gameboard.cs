using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Windows.Forms;
using DataStore;
using ScreenAreaSelector;

namespace MineSolver
{
    public class Gameboard
    {
        public Field[] GameboardFields { get; set; }

        private ScreenReader screenReader;
        private DataStorage dataStorage;

        [STAThread]
        static void Main()
        {
            var app = new Gameboard();
            app.dataStorage = DataStorage.GetInstance();
            Application.Run(new SelectRetangle());

            int[] dimensions = app.dataStorage.Data["Dimensions"];
            app.screenReader = new ScreenReader
            {
                XBoundsStart = dimensions[2],
                YBoundsStart = dimensions[3],
                XBoundsEnd = dimensions[0],
                YBoundsEnd = dimensions[1]
            };
            app.screenReader.Screenshot();

            app.Run();
        }

        public void Run()
        {
            var done = false;
            //TODO: GameboardFields[] needs to be populated AND a list of neighbour fields needs to be made for every Field
            //TODO: First move needs to be made before entering whileloop.
            //TODO: coloured fields needs to be active the grey fields are always inactive.
            while (!done)
            {
                foreach (var field in GameboardFields.Where(x => x.FieldType == FieldType.Numbered))
                {
                    NextMove(field); //This method determines if you can update a value arround the current field.
                    //TODO: screen Values need to be transfered into GameboardFields[] here. (update GameboardFields from screen)  
                }

                // Determine when the puzzle is solved.
                done = GameboardFields.All(x => x.FieldType != FieldType.Unknown);
            }
        }

        public void NextMove(Field field)
        {
            if (GetMines(field) == field.Value) //if the field already has the correct number of mines
            {
                SetNumbered(field); //all remaining unNumbered fields are not mines
                //TODO: Set this field to inactive, and if new fields appear update these aswell
                return;
            }
            if (GetUnknown(field) + GetMines(field) == field.Value)
            {
                SetMine(field); //all remaining unNumbered fields are mines
                //TODO: set this field to inactive
                return;
            }
            //Consider only running this after none of the other options can solve anymore since its PRETTY HEAVY.
            if (2 < 1)
            {
                TwoPointLogic(field);
            }
        }

        public int GetMines(Field field) //How many of my neighbours do i know are mines
        {
            return field.Neighbours.Count(id => GameboardFields[id].FieldType == FieldType.Mine);
        }

        public int GetNumbered(Field field) //How many of my neighbours do i know are Numbered
        {
            return field.Neighbours.Count(id => GameboardFields[id].FieldType == FieldType.Numbered);
        }

        public int GetUnknown(Field field) //How many of my neighbours do i know are Unknown
        {
            return field.Neighbours.Count(id => GameboardFields[id].FieldType == FieldType.Unknown);
        }

        public void SetMine(Field field) //Sets all neighbours of 'field' to be mines
        {
            foreach (var id in field.Neighbours.Where(id => GameboardFields[id].FieldType == FieldType.Unknown))
            {
                GameboardFields[id].FieldType = FieldType.Mine;
            }
            //GameboardFields[field.Id].IsActive = false;
        }

        public void SetNumbered(Field field) //Sets all neighbours of 'field' to be Numbered
        {
            foreach (var id in field.Neighbours.Where(id => GameboardFields[id].FieldType == FieldType.Unknown))
            {
                GameboardFields[id].FieldType = FieldType.Numbered;
                //update Value from screen
            }
            //GameboardFields[field.Id].IsActive = false;
        }

        public int GetOverLapsBetweenFields(Field current, Field neighbour)
        {
            int overlap = 0;
            foreach (var field in current.Neighbours)
            {
                foreach (var nField in neighbour.Neighbours)
                {
                    if (field == nField)
                    {
                        overlap++;
                    }
                }
            }
            return overlap;
        }

        public void TwoPointLogic(Field field)
        {
            var missingMines = field.Value - GetMines(field);
            var unknownFields = GetUnknownFieldIds(field);
            List<List<int>> combinations = PermutationsOf(unknownFields, missingMines);
            var solutions = new List<List<int>>(); 
            foreach (var combination in combinations)
            {
                bool isSolution = true;
                foreach (var nField in field.Neighbours)
                {
                    //TODO: Kig på om kombinationen opfylder naboernes krav
                    //1. Naboerne må aldrig tildeles flere miner end deres værdi
                    //2. Naboerne skal have unknowns tilbage efter at en løsning er blevet valgt hvor deres værdi ikke fuldt er nået.
                    if (field.Value - GetMines(field) < FieldsIncluded(combination, nField) || FieldsIncluded(combination, nField) > 0)
                    {//not a valid solution
                        isSolution = false;
                        break;
                    }
                }
                if (isSolution)
                {
                    solutions.Add(combination);
                }
            }

            if (solutions.Count != 0)
            {
                var comparisonMap = new Dictionary<int, int>();
                foreach (var neighbour in field.Neighbours)
                {
                    comparisonMap[neighbour] = 0; 
                }
                foreach (var candidate in solutions)
                {
                    foreach (var id in field.Neighbours)
                    {
                        if (candidate.Contains(field.Neighbours[id]))
                        {
                            comparisonMap[id] = comparisonMap[id] + 1;
                        }    
                    }
                }
                foreach (var id in field.Neighbours)
                {
                    if (comparisonMap[id] == 0)
                    {
                        SetNumbered(GameboardFields[id]);
                    }
                    if (comparisonMap[id] == solutions.Count)
                    {
                        SetMine(GameboardFields[id]);
                    }
                }
            }
        }

        public List<int> GetUnknownFieldIds(Field field)
        {
            var returnList = new List<int>();
            foreach (var id in field.Neighbours)
            {
                if (GameboardFields[id].FieldType == FieldType.Unknown)
                {
                    returnList[returnList.Count()] = id;
                }
            }
            return returnList;
        }

        public List<List<int>> PermutationsOf(List<int> fields, int mineCount)
        {
            var returnList = new List<List<int>>();
            var indexs = new List<int>();
            bool done = false;
            for (int i = 0; i < mineCount - 1; i++) //check that it only loops the needed times
            {
                indexs[i] = i;
            }
            while (!done)
            {
                var combination = new List<int>();
                for (int i = 0; i < mineCount - 1; i++) //check that it only loops the needed times
                {
                    combination[i] = fields[indexs[i]];
                }
                returnList.Add(combination);
                if (indexs[0] == fields.Count() - indexs.Count())
                {
                    done = true;
                }
                for (int i = indexs.Count() - 1; i > 0; i--) //check that it only loops the needed times
                {
                    if (i == indexs.Count() - 1)
                    {
                        if (indexs[i] != fields.Count() - 1)
                        {
                            indexs[i]++;
                            break;
                        }
                    }
                    else
                    {
                        if (indexs[i] != indexs[i + 1] - 1)
                        {
                            indexs[i]++;
                            for (int j = i; j < indexs.Count() - 1; j++) //check that it only loops the needed times
                            {
                                indexs[j + 1] = indexs[j] + 1;
                            }
                            break;
                        }
                    }
                }
            }
            return returnList;
        }

        public int FieldsIncluded(List<int> mines, int id)
        {
            var field = GameboardFields[id];
            var hits = 0;
            foreach (var mine in mines)
            {
                if (field.Neighbours.Contains(mine))
                {
                    hits++;
                }
            }
            return hits;
        }
    }
}
