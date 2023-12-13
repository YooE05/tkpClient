using FullSerializer;
using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

[System.Serializable]

public class InstructionData
{
    public class InstructionType
    {
        public const string image = "image";
        public const string video = "video";
    }

    public class GameInstruction
    {
        public bool InstructionDownloaded = false;
        public bool wasTrained = false;
        public List<Instruction> instructions = new List<Instruction>();
    }
    public class Instruction
    {
        public string text;
        public string type;
        public string url;
        public Sprite image;
    }

    [fsProperty] public Dictionary<string, GameInstruction> Game = new Dictionary<string, GameInstruction>();

    public InstructionData()
    {
    }

    public InstructionData(string gameName, GameInstruction gameInstruction)
    {
        if (Game.ContainsKey(gameName))
        {
            Game[gameName] = gameInstruction;
        }
        else
        {
            Game.Add(gameName, gameInstruction);
        }
    }
    public GameInstruction GetInstructionInfo(string gameName)
    {
        if (Game.ContainsKey(gameName))
        {
            return Game[gameName];
        }
        else
        {
            Debug.LogError("У данной игры список инструкий пустой!");
            return new GameInstruction();
        }
    }
    public IEnumerator LoadInstruction(string userId)
    {
        bool loadInstruction = false;

        FirebaseManager.Instance.Database.GetJson($"users/{userId}/private/onboardingCompleted", (jsonData) =>
        {
            GameInstruction gameInfo = new GameInstruction();
            if (jsonData.ToString() == "true")
            {
                gameInfo.wasTrained = true;
            }
            else
            {
                gameInfo.wasTrained = false;
            }
            this.Game.Add("Main", gameInfo);

            loadInstruction = true;
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading was trained data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading was trained data. Message - " + exception.Message);
        });

        yield return new WaitUntil(() => loadInstruction);

        foreach (var instruction in this.Game)
        {
            if (instruction.Value.wasTrained == false)
            {
                yield return LoadInstructionData();
            }
            else
            {
                Debug.Log($"Download of instructions for the game {instruction.Key} is complete");
            }
        }
    }

    public IEnumerator LoadInstructionMultiMode(string userId)
    {
        bool loadInstruction = false;

        FirebaseManager.Instance.Database.GetJson($"users/{userId}/private/onboardingCompleted", (jsonData) =>
        {
            JSONNode instrucionInfoJsonObj = JSONNode.Parse(jsonData);

            foreach (var instructinGame in instrucionInfoJsonObj)
            {
                GameInstruction gameInfo = new GameInstruction();
                gameInfo.wasTrained = instructinGame.Value;
                this.Game.Add(instructinGame.Key, gameInfo);
            }
            loadInstruction = true;
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading was trained data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading was trained data. Message - " + exception.Message);
        });

        yield return new WaitUntil(() => loadInstruction);

        foreach (var instruction in this.Game)
        {
            if (instruction.Value.wasTrained == false)
            {
                yield return LoadInstructionData(instruction.Key);
            }
            else
            {
                Debug.Log($"Download of instructions for the game {instruction.Key} is complete");
            }
        }
    }

    private IEnumerator LoadInstructionData()
    {
        bool loadCompletion = false;
        FirebaseManager.Instance.Database.GetJson($"/onboarding", (jsonData) =>
        {
            JSONNode instrucionInfoJsonObj = JSONNode.Parse(jsonData);

            foreach (var instrutcion in instrucionInfoJsonObj)
            {
                Instruction instruction = new Instruction();
                var info = instrutcion.Value;

                instruction.type = info["type"];
                instruction.text = info["text"];
                instruction.url = info["url"];

                this.Game["Main"].instructions.Add(instruction);
            }
            loadCompletion = true;
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading instruction data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading instruction data. Message - " + exception.Message);
        });

        yield return new WaitUntil(() => loadCompletion);

        int count = 0;

        foreach (var instructions in this.Game["Main"].instructions)
        {
            if (instructions.type == InstructionType.image)
                count++;
        }

        if (count == 0)
        {
            this.Game["Main"].InstructionDownloaded = true;
            Debug.Log($"Download of instructions for the game Main is complete");
        }
        else
        {
            int col = 0;
            foreach (var instructions in this.Game["Main"].instructions)
            {
                if (instructions.type == InstructionType.image)
                {
                    yield return (GetSpriteFromURL(instructions.url, (sprite) =>
                    {
                        instructions.image = sprite;
                        col++;

                        if (col == count)
                        {
                            this.Game["Main"].InstructionDownloaded = true;
                            Debug.Log($"Download of instructions for the game Main is complete");
                        }
                    }));
                }
            }
        }
    }

    private IEnumerator LoadInstructionData(string game)
    {
        bool loadCompletion = false;
        FirebaseManager.Instance.Database.GetJson($"/onboarding/{game}", (jsonData) =>
        {
            JSONNode instrucionInfoJsonObj = JSONNode.Parse(jsonData);

            foreach (var instrutcion in instrucionInfoJsonObj)
            {
                Instruction instruction = new Instruction();
                var info = instrutcion.Value;

                instruction.type = info["type"];
                instruction.text = info["text"];
                instruction.url = info["url"];

                this.Game[game].instructions.Add(instruction);
            }
            loadCompletion = true;
        }, (exception) =>
        {
            Debug.LogError("Exception while downloading instruction data. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Error,
                "Exception while downloading instruction data. Message - " + exception.Message);
        });

        yield return new WaitUntil(() => loadCompletion);

        int count = 0;

        foreach (var instructions in this.Game[game].instructions)
        {
            if (instructions.type == InstructionType.image)
                count++;
        }

        if (count == 0)
        {
            this.Game[game].InstructionDownloaded = true;
            Debug.Log($"Download of instructions for the game {game} is complete");
        }
        else
        {
            int col = 0;
            foreach (var instructions in this.Game[game].instructions)
            {
                if (instructions.type == InstructionType.image)
                {
                    yield return (GetSpriteFromURL(instructions.url, (sprite) =>
                    {
                        instructions.image = sprite;
                        col++;

                        if (col == count)
                        {
                            this.Game[game].InstructionDownloaded = true;
                            Debug.Log($"Download of instructions for the game {game} is complete");
                        }
                    }));
                }
            }
        }
    }

    public void SendCompleteInstructionGame(string gameName, string userId)
    {
        Dictionary<string, object> args = new Dictionary<string, object>() { { "userId", userId }, { "game", AppConfigurations.GameName }, { "gameMode", gameName } };
        SendCompleteInstruction(args);
    }

    public void SendCompleteInstructionGame(string userId)
    {
        Dictionary<string, object> args = new Dictionary<string, object>() { { "userId", userId }, { "game", AppConfigurations.GameName } };
        SendCompleteInstruction(args);
    }

    private void SendCompleteInstruction(Dictionary<string, object> args)
    {
        FirebaseManager.Instance.Functions.CallCloudFunction("OnOnboardingCompleted", args, (data) =>
        {
            Debug.Log(data);
        }, (exception) =>
        {
            Debug.LogError("Exception when updating onboarding status. Message - " + exception.Message);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Critical,
                "Exception when updating onboarding status. Message - " + exception.Message);
        });
    }

    public IEnumerator GetSpriteFromURL(string url, Action<Sprite> callback)
    {
        Debug.Log("Downloading texture with url - " + url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        //if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error downloading texture. Error - " + www.error);
            GameAnalyticsSDK.GameAnalytics.NewErrorEvent(GameAnalyticsSDK.GAErrorSeverity.Warning, "Error downloading texture. Error - " + www.error);
            callback.Invoke(null);
        }
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite downloadedSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
            callback.Invoke(downloadedSprite);
        }
    }
}