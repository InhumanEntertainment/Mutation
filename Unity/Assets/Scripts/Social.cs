using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class MutationSocial
{
    public List<IAchievement> Achievements;

    //============================================================================================================================================//
    public MutationSocial()
    {
    }
	
	//============================================================================================================================================//
    public void CheckAchievements()
    {
        // First Level //
		//if (LevelCompleted) 
		//	CompleteAchievement("FirstLevel", 100);							
    }
    
    //============================================================================================================================================//
    public void Authenticate()
    {
        Social.localUser.Authenticate(AuthenticateCallback);
    }

    //============================================================================================================================================//
    public void CompleteAchievement(string id, double value)
    {
        IAchievement achievement = Social.CreateAchievement();
        achievement.id = id;
        achievement.percentCompleted = value;
        achievement.ReportProgress(ReportCallback);
    }

    //============================================================================================================================================//
    public void AddScore(long value)
    {
        /*string board = "grp.continuum";

        if (Game.Instance.GameMode == Game.Instance.Data.Rush)
            board = "grp.rush";
        else if (Game.Instance.GameMode == Game.Instance.Data.Overload)
            board = "grp.overload";
        
        Social.ReportScore(value, board, ReportCallback);*/
    }

    //============================================================================================================================================//
    void AuthenticateCallback(bool success)
    {
        //InhumanIOS.Popup ("Authentication", success.ToString (), "Ok");

        if (success)
        {
            Debug.Log("Authentication Succesful");
            //Social.LoadAchievements(AchievementsCallback);
            //Social.LoadAchievementDescriptions(DescriptionsCallback);
            //Social.LoadScores("grp.continuum", ScoresCallback);
            //Social.LoadScores("grp.rush", ScoresCallback);
        }
        else
        {
            Debug.Log("Authentication Failed");
        }
    }

    //============================================================================================================================================//
    void AchievementsCallback(IAchievement[] achievements)
    {
        if (achievements.Length == 0)
        {
            Debug.Log("No Achievements Found");
        }
        else
        {
            Debug.Log(achievements.Length + " Achievements Found");
            for (int i = 0; i < achievements.Length; i++)
            {
                Debug.Log("Acievement " + i + ": " + achievements[i].percentCompleted);
            }
        }
    }

    //============================================================================================================================================//
    void DescriptionsCallback(IAchievementDescription[] descriptions)
    {
        if (descriptions.Length == 0)
        {
            Debug.Log("No Descriptions Found");
        }
        else
        {
            Debug.Log(descriptions.Length + " Descriptions Found");
            for (int i = 0; i < descriptions.Length; i++)
            {
                Debug.Log("Description " + i + ": " + descriptions[i].title);
            }
        }
    }

    //============================================================================================================================================//
    void ScoresCallback(IScore[] scores)
    {
        if (scores.Length == 0)
        {
            Debug.Log("No Scores Found");
        }
        else
        {
            Debug.Log(scores.Length + " Scores Found");

            for (int i = 0; i < scores.Length; i++)
            {
                Debug.Log("Scrore " + i + ": " + scores[i].formattedValue);
            }
        }
    }

    //============================================================================================================================================//
    void ReportCallback(bool success)
    {
        //InhumanIOS.Popup ("Report", success.ToString (), "Ok");
        Debug.Log(success ? "Success" : "Fail");
    }
}
