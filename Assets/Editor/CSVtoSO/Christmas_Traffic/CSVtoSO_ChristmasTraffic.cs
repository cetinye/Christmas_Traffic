using UnityEngine;
using UnityEditor;
using System.IO;

namespace Christmas_Traffic
{
    public class CSVtoSO_ChristmasTraffic
    {
        //Check .csv path
        private static string CSVPath = "/Editor/CSVtoSO/Christmas_Traffic/LevelCSV_ChristmasTraffic.csv";

        [MenuItem("Tools/CSV_to_SO/Christmas_Traffic/Generate")]
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
                level.LevelId = int.Parse(splitData[0]);
                level.TotalSantaAmount = int.Parse(splitData[1]);
                level.NumOfSantasRequiredToLand = int.Parse(splitData[2]);
                level.MooseSantaAmount = int.Parse(splitData[3]);
                level.RocketSantaAmount = int.Parse(splitData[4]);
                level.BalloonSantaAmount = int.Parse(splitData[5]);
                level.ActiveLaneCount = int.Parse(splitData[6]);
                level.TotalTime = int.Parse(splitData[7]);
                level.LevelUpCriteria = int.Parse(splitData[8]);
                level.LevelDownCriteria = int.Parse(splitData[9]);
                level.MaxInGame = int.Parse(splitData[10]);
                level.MinScore = int.Parse(splitData[11]);
                level.PenaltyPoints = int.Parse(splitData[12]);

                AssetDatabase.CreateAsset(level, $"Assets/Data/Christmas_Traffic/Levels/{"ChristmasTraffic_Level " + startingNamingIndex}.asset");
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