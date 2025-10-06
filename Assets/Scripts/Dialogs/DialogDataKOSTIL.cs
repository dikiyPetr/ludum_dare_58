namespace Dialogs
{
    public class DialogDataKOSTIL
    {
        public static string GetJsonText()
        {
          return @"
{
  ""dialogs"": [
    {
      ""id"": ""exit_basement"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Мысли"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Покинуть подвал?"",
          ""options"": [
            {
              ""text"": ""Да"",
              ""nextNodeId"": null
            },
            {
              ""text"": ""Нет"",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""map_0"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""The answers might be with the head of the morgue... or in the morgue itself"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""condition"": {
                ""type"": ""SuspectAliveAndFree"",
                ""suspectId"": ""0""
              },
              ""text"": ""Find him and make him talk"",
              ""nextNodeId"": ""_map_result_is_stels:false_0""
            },
            {
              ""condition"": {
                ""type"": ""NotCondition"",
                ""condition"": {
                  ""type"": ""HasEvidence"",
                  ""id"": ""2""
                }
              },
              ""text"": ""Get into the morgue"",
              ""nextNodeId"": ""_map_result_is_stels:true_0""
            },
            {
              ""text"": ""..."",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:false_0"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Caught the head of the morgue outside the hospital. I'll make him talk down in the basement"",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_outside_result_is_stels:false_0""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:true_0"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Broke into the morgue. Found a note from the head doctor: Ignore the relatives. Don't touch bodies 957 and 963"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""Need to get out before the find me"",
              ""nextNodeId"": ""_outside_result_is_stels:true_0""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""skip_day"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""The bed calls for rest... to forget for a while"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""Sleep"",
              ""nextNodeId"": ""_sleep""
            },
            {
              ""text"": ""There's still work to do"",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""news_1"",
      ""startNodeId"": ""1"",
      ""speaker"": ""News"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Hospital Y refuses to release the body to the relatives. Doctors decline to comment"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_news_1_end""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""news_2"",
      ""startNodeId"": ""1"",
      ""speaker"": ""News"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Company X's deputy director visited city Y to support an orphanage. The move appears to be an attempt to improve the company's image"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_news_2_end""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""news_3"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Ad"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""~Cutting-edge therapies methods that will extend your life~"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""2""
            }
          ]
        },
        {
          ""id"": ""2"",
          ""text"": ""~Health and longevity, as if two people lived inside your body~"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""... and a familiar voice"",
              ""nextNodeId"": ""_news_3_end""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""dialogue_0"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Morgue manager"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""What the hell? What do you want from me?"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""Why don't the hospital release the bodies to the victims' families?"",
              ""nextNodeId"": ""2""
            },
            {
              ""text"": ""Forget everything, and I'll let you go"",
              ""nextNodeId"": ""4""
            }
          ]
        },
        {
          ""id"": ""2"",
          ""text"": ""What do I have to do with this? I was told, I didn't release anything!"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""Who told you that?"",
              ""nextNodeId"": ""3""
            }
          ]
        },
        {
          ""id"": ""3"",
          ""text"": ""I don’t know… but the relatives of the deceased came to me and said it wasn’t the hospital, but a private company, company X, that contacted them. I don’t know anything else. I swear"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_dialogue_0:clue_3""
            }
          ]
        },
        {
          ""id"": ""4"",
          ""text"": ""Yes, I swear on my mother, you won't remember me"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""But we'll have to take a little ride."",
              ""nextNodeId"": ""_dialogue_0:release""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""dialogue_1"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Deputy director"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""If you want ransom, talk to my superiors"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""What's going on at the hospital?"",
              ""nextNodeId"": ""2""
            },
            {
              ""text"": ""Forget everything, and I'll let you go"",
              ""nextNodeId"": ""4""
            }
          ]
        },
        {
          ""id"": ""2"",
          ""text"": ""It's not my business. Security's taking care of it"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""I need name"",
              ""nextNodeId"": ""3""
            }
          ]
        },
        {
          ""id"": ""3"",
          ""text"": ""You better stay away from him. He's somehow connected with mafia, and I have no idea how he ended up here."",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""NAME"",
              ""nextNodeId"": ""4""
            }
          ]
        },
        {
          ""id"": ""4"",
          ""text"": ""PAVEl! His name's Pavel Z. But beyond the rumors, I don't know anything about him"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_dialogue_1:clue_5""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""dialogue_2"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Head doctor"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Do you need information about the deceased?"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""condition"": {
                ""type"": ""NotCondition"",
                ""condition"": {
                  ""type"": ""HasEvidence"",
                  ""id"": ""8""
                }
              },
              ""text"": ""?"",
              ""nextNodeId"": ""2""
            },
            {
              ""condition"": {
                ""type"": ""HasEvidence"",
                ""id"": ""8""
              },
              ""text"": ""What else do you know?"",
              ""nextNodeId"": ""4""
            }
          ]
        },
        {
          ""id"": ""2"",
          ""text"": ""I regret what I did, but I had no choice"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""What..."",
              ""nextNodeId"": ""3""
            }
          ]
        },
        {
          ""id"": ""3"",
          ""text"": ""I let the bodies be taken from the hospital - and... some of them were alive"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""Where are they?"",
              ""nextNodeId"": ""4""
            }
          ]
        },
        {
          ""id"": ""4"",
          ""text"": ""If i could, I'd help you - but I'm helpless here. I have nothing to tell you. I don't care what happens next"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_dialogue_2:clue_8""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""map_1"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Deputy director of company X. Her company is connected to the hospital and the victims' families"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""condition"": {
                ""type"": ""SuspectAliveAndFree"",
                ""suspectId"": ""1""
              },
              ""text"": ""Catch and make her talk"",
              ""nextNodeId"": ""_map_result_is_stels:false_1""
            },
            {
              ""condition"": {
                ""type"": ""NotCondition"",
                ""condition"": {
                  ""type"": ""HasEvidence"",
                  ""id"": ""8""
                }
              },
              ""text"": ""Follow her"",
              ""nextNodeId"": ""_map_result_is_stels:true_1""
            },
            {
              ""text"": ""..."",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""map_2"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Hospital head. All this is going on in his hospital."",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""condition"": {
                ""type"": ""SuspectAliveAndFree"",
                ""suspectId"": ""2""
              },
              ""text"": ""Catch and make him talk"",
              ""nextNodeId"": ""_map_result_is_stels:false_2""
            },
            {
              ""condition"": {
                ""type"": ""NotCondition"",
                ""condition"": {
                  ""type"": ""MultiCondition"",
                  ""logicType"": ""OR"",
                  ""conditions"": [
                    {
                      ""type"": ""HasEvidence"",
                      ""id"": ""5""
                    },
                    {
                      ""type"": ""SuspectAliveAndFree"",
                      ""suspectId"": ""2""
                    }
                  ]
                }
              },
              ""text"": ""Pay him a visit"",
              ""nextNodeId"": ""_map_result_is_stels:true_2""
            },
            {
              ""text"": ""..."",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""map_3"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Thoughts"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""That voice from the ad"",
          ""notebookEntries"": [],
          ""highlights"": [],
          ""options"": [
            {
              ""text"": ""He sounds familiar. Address isn't far"",
              ""nextNodeId"": ""_map_result_is_stels:true_3""
            },
            {
              ""text"": ""..."",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:false_1"",
      ""startNodeId"": ""1"",
      ""speaker"": ""In the park"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""She was easy to find. No security - we could talk. In my basement"",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_outside_result_is_stels:false_1""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:true_1"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Outside"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""After the interview, some old man was hitting on her. I recognized him - the head doctor of hospital Y. Now he owes me a few answers."",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_outside_result_is_stels:true_1""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:true_2"",
      ""startNodeId"": ""1"",
      ""speaker"": ""In the head doctor's house"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""I slipped into the head doctor's house. The door wasn't even locked. Chaos everywhere. On his desk - a tape in an opened envelope."",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_outside_result_is_stels:true_2""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:false_2"",
      ""startNodeId"": ""1"",
      ""speaker"": ""In the park"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""He kept turning around as I followed him. Our eyes met - he knew I was there, but fear pinned him in place."",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""_outside_result_is_stels:false_2""
            }
          ]
        }
      ]
    },
    {
      ""id"": ""cutscene_is_stels:true_3"",
      ""startNodeId"": ""1"",
      ""speaker"": ""On the way"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""As I headed to the address from the ad, I kept wondering where I'd heard that voice before"",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": ""2""
            }
          ]
        },
        {
          ""id"": ""2"",
          ""text"": ""'Chapter one ends'"",
          ""options"": [
          ]
        }
      ]
    },
    {
      ""id"": ""need_to_release"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Мысли"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""У меня уже есть гость. Нужно сначала решить, что с ним делать, перед тем, как отправляться за вторым"",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    },
    {
      ""id"": ""need_to_sleep"",
      ""startNodeId"": ""1"",
      ""speaker"": ""Мысли"",
      ""nodes"": [
        {
          ""id"": ""1"",
          ""text"": ""Глаза слипаются. Пора спать"",
          ""options"": [
            {
              ""text"": ""..."",
              ""nextNodeId"": null
            }
          ]
        }
      ]
    }
  ]
}

";
        }
    }
}