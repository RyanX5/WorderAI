using UnityEngine;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class Word2VecLoader : MonoBehaviour
{
    public TextAsset word2VecTextFile;  // Reference to your Word2Vec text file

    private Dictionary<string, List<float>> wordVectors = new Dictionary<string, List<float>>();

    [SerializeField] TMP_InputField inp1;
    [SerializeField] TMP_InputField inp2;
    [SerializeField] TMP_Text result;

    [SerializeField] TMP_Text[] closest;
    [SerializeField] Slider[] sliders;

    string word1;
    string word2;

    Dictionary<float, string> maxWords = new Dictionary<float, string>();

    List<string> maxStrings = new List<string>();

    // word1 is target word, word2 is user input


    void Start()
    {
        inp1.onEndEdit.AddListener(AssignWords);
        inp2.onEndEdit.AddListener(AssignWords2);

        if (word2VecTextFile != null)
        {
            string[] lines = word2VecTextFile.text.Split('\n');
            foreach (string line in lines)
            {
                string[] parts = line.Trim().Split(' ');
                string word = parts[0];
                List<float> vector = new List<float>();
                for (int i = 1; i < parts.Length; i++)
                {
                    vector.Add(float.Parse(parts[i]));
                }
                wordVectors[word] = vector;
            }
            print("Loaded asset!");

            word1 = ChooseRandom();

            print("Set random word: " + word1);
        }
        else
        {
            Debug.LogError("Word2Vec text file not assigned.");
        }
    }




    void AssignWords(string s)
    {
        word1 = s;
        print(":word1 set to: " + word1);


    }

    void AssignWords2(string s)
    {
        word2 = s;
        print(":Word2 set to: " + word2);
        GenerateVector(word1, word2);


    }


    void GenerateVector(string word1, string word2)
    {
        // for every word input, compare get relative similarity
        // for every word2, find similarity and insert into the vector

        if (wordVectors.ContainsKey(word1) && wordVectors.ContainsKey(word2))
        { 

            List<float> vec1 = wordVectors[word1];
            List<float> vec2 = wordVectors[word2];

            float similarity = CalculateSimilarity(vec1, vec2);

            maxWords[similarity] = word2;

            //print("Sorting maxWords");

            maxStrings = sortMaxWords(maxWords);

            //print("Sorted maxWords");

            PrintSortedMax();
        }

        else
        {
            print("word not found in the database");
        }
    }

    float CalculateRelativeSimilarity(float similarity)
    {
        float relative = similarity * 100.0f;
        return relative;
    }

    private void PrintSortedMax()
    {
        foreach(string s in maxStrings)
        {
            print("String: " + s + ": " + CalculateSimilarity(wordVectors[word1], wordVectors[s]));
        }

        for (int i = 0; i < maxStrings.Count(); ++i)
        {
            if (i < 3)
            {
                closest[i].text = maxStrings[i].ToString();
                sliders[i].value = CalculateSimilarity(wordVectors[word1], wordVectors[maxStrings[i]]);
            }
        }

    }

    public float CalculateSimilarity(List<float> vec1, List<float> vec2)
    {
        float dotProduct = 0f;
        float norm1 = 0f;
        float norm2 = 0f;

        float similarity;

        for (int i = 0; i < vec1.Count; ++i)
        {
            dotProduct += vec1[i] * vec2[i];
            norm1 += vec1[i] * vec1[i];
            norm2 += vec2[i] * vec2[i];
        }

        similarity = dotProduct / (Mathf.Sqrt(norm1) * Mathf.Sqrt(norm2));

        return similarity;
    }

    private string ChooseRandom()
    {
        List<string> keys = new List<string>(wordVectors.Keys);

        int randomIndex = (int)Random.Range(0, keys.Count);

        string randomWord = keys[randomIndex];

        return randomWord;
    }

    List<string> sortMaxWords(Dictionary<float, string> dict)
    {

        List<float> sortedKeys = dict.Keys.ToList();
        sortedKeys.Sort();
        sortedKeys.Reverse();

        List<string> stringResult = new List<string>();

        foreach(float key in sortedKeys)
        {
            stringResult.Add(dict[key]);
        }


        return stringResult;


    }

}



//    public void CalculateMaxSimilarity(string word)
//    {
//        float dotProduct = 0f;
//        float norm1 = 0f;
//        float norm2 = 0f;
//        List<float> vector2;

//        float similarity;
//        float maxsim = 0f;

//        string wordf;

//        int count = 0;


//        List<float> vector1 = wordVectors[word];

//        foreach (var pair in wordVectors)
//        {
//            if (pair.Key != word)
//            {
//                vector2 = pair.Value;
//                for (int j = 0; j < vector1.Count; j++)
//                {
//                    if (vector2.Count == vector1.Count)
//                    {
//                        dotProduct += vector1[j] * vector2[j];
//                        norm1 += vector1[j] * vector1[j];
//                        norm2 += vector2[j] * vector2[j];
//                    }

//                }
//                similarity = dotProduct / (Mathf.Sqrt(norm1) * Mathf.Sqrt(norm2));

//                dotProduct = 0f;
//                norm1 = 0f;
//                norm2 = 0f;

//                if (similarity > maxsim && similarity != float.NaN && !maxWords.ContainsKey(similarity) && count <= 10)
//                {
//                    maxsim = similarity;
//                    maxWords[similarity] = pair.Key;
//                    count++;

//                }
//            }

//        }

//    }



//    public void CompareWords(string word1, string word2)
//    {
//        if (wordVectors.ContainsKey(word1) && wordVectors.ContainsKey(word2))
//        {
//            List<float> vector1 = wordVectors[word1];
//            List<float> vector2 = wordVectors[word2];

//            float similarity = CalculateSimilarity(vector1, vector2);
//            CalculateMaxSimilarity(word1);

//            //float relativeSimilarity = 100f * (similarity/maxSimilarity);

//            //if (relativeSimilarity >= 100f)
//            //    relativeSimilarity = 100f;

//            //result.text = "The word " + word2 + " is " + (int)relativeSimilarity + "% close to the word " + word1;

//            print("here's the sorted list: " + maxWords);
//        }
//        else
//        {
//            print(-1f); // Words not found in the Word2Vec data
//            result.text = "Sorry, one of the words were not found in the database. Try again!";
//        }
//    }

//    public void Retry()
//    {
//        AssignWords("");
//        AssignWords2("");

//        inp1.text = "";
//        inp2.text = "";
//    }

//    

//}
