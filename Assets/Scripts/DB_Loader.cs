using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DB_Loader : DataDownloader
{


    public static DB_Loader Instance;

    private void Awake() {
        Instance = this;
        path = "Data";
    }

    int lineIndex = 0;

    public override void GetCell(int rowIndex, List<string> cells) {
        Debug.Log($"[CSV] Ligne {rowIndex} → {cells.Count} cellules : "
                + string.Join(" | ", cells));
        base.GetCell(rowIndex, cells);

        if (sheetIndex == 4) {
            if (!string.IsNullOrEmpty(cells[3])) {
                // add color
                var cc = new DisplayMedia.ColorCode();
                cc.name = cells[2];
                cc.hexa = cells[3];
                Color color;
                if (!ColorUtility.TryParseHtmlString($"#{cc.hexa}", out color)) {
                    Debug.LogError($"no color match hexa : {cc.hexa}");
                }
                cc.color = color;
                cc.index = DisplayMedia.Instance.colorCodes.Count;
                DisplayMedia.Instance.colorCodes.Add(cc);
            }

            if (rowIndex == 1) {
                MissionIntroDisplay.Instance.mail_adress = cells[6];
            } else if (rowIndex == 3) {
                MissionIntroDisplay.Instance.mail_subject = cells[6];
            }

        }

        if (rowIndex < 1)
            return;

        if ( sheetIndex == 4) {

            switch (rowIndex) {
                case 1:
                case 4:
                    MissionIntroDisplay.Instance.gameIntroduction = cells[1];
                    break;
                case 5:
                    MissionIntroDisplay.Instance.gameConclusion= cells[1];
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    MissionIntroDisplay.Instance.biaisTitles.Add(cells[0]);
                    MissionIntroDisplay.Instance.biaisDefs.Add(cells[1]);
                    break;
                case 10:
                case 11:
                case 12:
                case 13:
                    MissionIntroDisplay.Instance.hpTitles.Add(cells[0]);
                    MissionIntroDisplay.Instance.hpDefs.Add(cells[1]);
                    break;
                case 14:
                case 15:
                case 16:
                case 17:
                    MissionIntroDisplay.Instance.missionIntroductions.Add(cells[1]);
                    break;
                default:
                    break;
            }
            return;
        }

        if (!string.IsNullOrEmpty(cells[0])) {
            lineIndex = 0;
            var newDocument = new Document();
            newDocument.types = cells[0].Split(" / ").ToList();
            LevelManager.Instance.levels[sheetIndex].documents.Add(newDocument);
        }

        var lastDocument = LevelManager.Instance.levels[sheetIndex].documents.Last();
        switch (sheetIndex) {
            // FPF.csv
            case 0:
                if (lineIndex == 0) {
                    lastDocument.medias.Add(cells[1]);
                    lastDocument.medias.Add(cells[2]);
                    lastDocument.fake = cells[3] == "Fake";
                    lastDocument.explanation_Good = cells[8];
                    lastDocument.explanation_Bad = cells[9];
                    lastDocument.clue = cells[10];
                }
                    lastDocument.colorNames.Add(cells[6]);
                lastDocument.interactibleElements.Add(cells[4]);

                break;
            // FHO.csv
            case 1:
                lastDocument.medias.Add(cells[1]);
                lastDocument.correctStatement = cells[2];
                lastDocument.explanation_Good = cells[3];
                lastDocument.explanation_Bad= cells[4];
                lastDocument.clue = cells[5];
                break;
            // LBC.csv
            case 2:
                lastDocument.medias.Add(cells[1]);
                lastDocument.correctStatement = cells[2];
                lastDocument.explanation_Good = cells[3];
                lastDocument.explanation_Bad = cells[4];
                lastDocument.clue = cells[5];
                break;
            // QC.csv
            case 3:
                for (int i = 0;i < 4; ++i) {
                    if ( lineIndex == 0)
                        lastDocument.medias.Add(cells[i + 1]);
                    else
                        lastDocument.statements.Add(cells[i+1]);
                }
                lastDocument.explanation_Good = cells[5];
                lastDocument.explanation_Bad = cells[6];
                lastDocument.clue = cells[7];
                break;
            default:
                break;
        }

        ++lineIndex;
    }

    void LoadFakeNoFake(List<string> cells) {
        
    }

    void Load_HVSOP() {

    }

    void Load_Biais() {

    }

    void Load_QuoiCroire() {

    }
}
