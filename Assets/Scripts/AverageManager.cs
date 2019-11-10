using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AverageManager : MonoBehaviour
{
    public RawImage DisplayImage;
    public string ImageFilePath;
    public string RequestFilePath;

    public Slider MinAgeSlider;
    public Slider MaxAgeSlider;

    public Text MinAgeValueText;
    public Text MaxAgeValueText;

    public Toggle MaleToggle;
    public Toggle FemaleToggle;

    public Text ErrorText;

    public GameObject LoadingPanel;
    public float Timeout;

    bool RequestPending = false;

    [System.Serializable]
    private struct RequestDefinition
    {
        public int minAge;
        public int maxAge;
        public string gender;
    }

    bool ImageLoading = false;
    float Timer;

    // Start is called before the first frame update
    void Start()
    {
        MinAgeSlider.onValueChanged.AddListener(OnNewMinAgeValue);
        MaxAgeSlider.onValueChanged.AddListener(OnNewMaxAgeValue);
    }

    void OnNewMinAgeValue(float value)
    {
        MinAgeSlider.value = Mathf.Min(value, MaxAgeSlider.value);
    }

    void OnNewMaxAgeValue(float value)
    {
        MaxAgeSlider.value = Mathf.Max(value, MinAgeSlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        MinAgeValueText.text = MinAgeSlider.value.ToString();
        MaxAgeValueText.text = MaxAgeSlider.value.ToString();
        
        if(RequestPending)
        {
            Timer -= Time.deltaTime;

            if(Timer <= 0.0f)
            {
                ErrorText.text = "Aucun résultat";
                StopCoroutine(LoadImage());
                ImageLoading = false;
                RequestPending = false;
                LoadingPanel.SetActive(false);
            }

            if (!ImageLoading && File.Exists(ImageFilePath))
            {
                StartCoroutine(LoadImage());
            }
        }
        
    }

    public void GenerateRequest()
    {
        ErrorText.text = "";

        if (!MaleToggle.isOn && !FemaleToggle.isOn)
        {
            ErrorText.text = "Vous devez spécifier au moins un genre";
            return;
        }

        LoadingPanel.SetActive(true);

        File.Delete(ImageFilePath);

        RequestDefinition request = new RequestDefinition();
        request.minAge = (int)MinAgeSlider.value;
        request.maxAge = (int)MaxAgeSlider.value;
        if(MaleToggle.isOn)
            request.gender += "M";

        if(FemaleToggle.isOn)
            request.gender += "F";

        File.WriteAllText(RequestFilePath, JsonUtility.ToJson(request, true));

        RequestPending = true;
        Timer = Timeout;
    }

    IEnumerator LoadImage()
    {
        ImageLoading = true;
        
        while (!File.Exists(ImageFilePath))
        {
            yield return new WaitForSeconds(0.1f);
        }

       if( File.Exists(ImageFilePath))
        {
            yield return new WaitForSeconds(0.1f);

            UnityWebRequest www = UnityWebRequestTexture.GetTexture("file://" + System.Environment.CurrentDirectory + "/" + ImageFilePath);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                UnityEngine.Debug.Log(www.error);
            }
            else
            {
                DisplayImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            }
        }

        ImageLoading = false;
        RequestPending = false;
        LoadingPanel.SetActive(false);
    }
}
