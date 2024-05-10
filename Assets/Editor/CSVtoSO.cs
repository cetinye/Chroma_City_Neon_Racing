using UnityEngine;
using UnityEditor;
using System.IO;

namespace Chroma_City_Neon_Racing
{
    public class CSVtoSO
    {
        //Check .csv path
        private static string CSVPath = "/Editor/LevelCSV.csv";

        [MenuItem("Tools/CSV_to_SO/Chroma_City_Neon_Racing/Generate")]
        public static void GenerateSO()
        {
            int startingNamingIndex = 1;
            string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);

            for (int i = 1; i < allLines.Length; i++)
            {
                allLines[i] = RedefineString(allLines[i]);
            }

            for (int i = 1; i < allLines.Length; i++)
            {
                string[] splitData = allLines[i].Split(';');

                //Check data indexes
                LevelSO level = ScriptableObject.CreateInstance<LevelSO>();
                level.levelId = int.Parse(splitData[0]);
                level.minSpeedRange = float.Parse(splitData[1]);
                level.maxSpeedRange = float.Parse(splitData[2]);
                level.ballSpeedChangeAmount = float.Parse(splitData[3]);
                level.speedPenatlyAmount = float.Parse(splitData[4]);
                level.pathLength = int.Parse(splitData[5]);
                level.shieldPowerup = int.Parse(splitData[6]);
                level.speedPowerup = int.Parse(splitData[7]);
                level.timePowerup = int.Parse(splitData[8]);
                level.durationOfPowerups = int.Parse(splitData[9]);
                level.timeLimit = int.Parse(splitData[10]);
                level.maxScore = int.Parse(splitData[11]);

                AssetDatabase.CreateAsset(level, $"Assets/Data/Chroma_City_Neon_Racing/{"CCNR_Level " + startingNamingIndex}.asset");
                startingNamingIndex++;
            }

            AssetDatabase.SaveAssets();

            static string RedefineString(string val)
            {
                char[] charArr = val.ToCharArray();
                bool isSplittable = true;

                for (int i = 0; i < charArr.Length; i++)
                {
                    if (charArr[i] == '"')
                    {
                        charArr[i] = ' ';
                        isSplittable = !isSplittable;
                    }

                    if (isSplittable && charArr[i] == ',')
                        charArr[i] = ';';

                    if (isSplittable && charArr[i] == '.')
                        charArr[i] = ',';
                }

                return new string(charArr);
            }
        }
    }
}