using UnityEngine;
using System.Collections.Generic;
using FMODUnity;
using FMOD;
using FMOD.Studio;
using System.Runtime.InteropServices;

public class FMODAudioVisualizer : MonoBehaviour
{
    [Header("Game Performance")]
    [SerializeField] private int fps = 0;
    [Header("FMOD Event")]
    [EventRef] [SerializeField] private string eventPath = null;
    [SerializeField] private bool playOnAwake = true;
    [Header("Audio Sample Data Settings")]
    [SerializeField] private int windowSize = 512;
    [SerializeField] private DSP_FFT_WINDOW windowShape = DSP_FFT_WINDOW.RECT;
    [Header("Select Metering Object Prefab")]
    [SerializeField] private GameObject MeterObject = null;
    [SerializeField] private float meterIntensity = 10f;
    [SerializeField] private float SpaceBetweenMeters = 0.5f;
    [Header("Meter Speed Settings")]
    [SerializeField] private float bufferStartSpeed = 0.005f;
    [SerializeField] private float bufferAccelRate = 1.2f;
    [Header("Testing")]
    private List<float> freqRanges = new List<float>();
    private int numSampleInFirstBand = 1;

    private EventInstance _event;
    private ChannelGroup channelGroup;
    private DSP DSPFFT;
    private DSP_PARAMETER_FFT fftparam;

    private GameObject[] bandMeters;

    private float[] _samples;
    public static float[] freqBands = new float[7];
    public static float[] bandBuffer = new float[7];
    private float[] bufferDecrease = new float[7];

    private float time = 0f;
    private int frameCount = 0;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        //Prepare FMOD event
        PrepareFMODeventInstance();
        
        //find out how many meters are needed
        SetNumberOfMeters();

        //spawn meters.
        SpawnMeterObjects();

        _samples = new float[windowSize];
        freqBands = new float[freqRanges.Count];
        bandBuffer = new float[freqRanges.Count];
        bufferDecrease = new float[freqRanges.Count];
    }

    private void PrepareFMODeventInstance()
    {
        _event = RuntimeManager.CreateInstance(eventPath);
        _event.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        if (playOnAwake)
            _event.start();

        RuntimeManager.CoreSystem.createDSPByType(DSP_TYPE.FFT, out DSPFFT);
        DSPFFT.setParameterInt((int)DSP_FFT.WINDOWTYPE, (int)windowShape);
        DSPFFT.setParameterInt((int)DSP_FFT.WINDOWSIZE, windowSize * 2);

        _event.getChannelGroup(out channelGroup);
        channelGroup.addDSP(0, DSPFFT);
    }

    private void SetNumberOfMeters()
    {
        float singleSizeOfOneSample = 24000f / windowSize;
        float HzForFirstBand = singleSizeOfOneSample;

        while (HzForFirstBand < 60f)
        {
            numSampleInFirstBand++;
            HzForFirstBand += singleSizeOfOneSample;
        }

        freqRanges.Clear();
        freqRanges.Add(HzForFirstBand);
        float hzRange = HzForFirstBand;
        float hzSize = HzForFirstBand;
        while (hzRange < 24000f)
        {
            hzSize *= 2;
            hzRange += hzSize;
            if (hzRange < 24000f)
                freqRanges.Add(hzRange);
        }
    }
    
    private void SpawnMeterObjects()
    {
        int posOffSet = 0;
        float spaceOffset = 0;
        float spaceOffset2 = 0.5f;
        bandMeters = new GameObject[freqRanges.Count];
        for (int i = 0; i < freqRanges.Count; i++)
        {
            bandMeters[i] = Instantiate(MeterObject, transform.position, transform.rotation);
            bandMeters[i].transform.position = new Vector3(transform.position.x + posOffSet + spaceOffset, transform.position.y, transform.position.z);

            if (bandMeters[i].GetComponent<ParamCube>() != null)
            {
                bandMeters[i].GetComponent<ParamCube>()._band = posOffSet;
            }
            posOffSet++;
            spaceOffset2 += 0.5f;
            spaceOffset += SpaceBetweenMeters - 0.5f;
        }
    }

    private void Update()
    {
        GetSpectrumData();
        FrequencyBands();
        BandBuffer();
        countFPS();
    }

    private void GetSpectrumData()
    {
        System.IntPtr data;
        uint length;

        DSPFFT.getParameterData(2, out data, out length);
        fftparam = (DSP_PARAMETER_FFT)Marshal.PtrToStructure(data, typeof(DSP_PARAMETER_FFT));

        if (fftparam.numchannels == 0)
        {
            _event.getChannelGroup(out channelGroup);
            channelGroup.addDSP(0, DSPFFT);
            //Debug.Log("wait I'm not ready yet!");
        }
        else if (fftparam.numchannels >= 1)
        {
            for (int b = 0; b < windowSize; b++)
            {
                float totalChannelData = 0f;
                for (int c = 0; c < fftparam.numchannels; c++)
                    totalChannelData += fftparam.spectrum[c][b];
                _samples[b] = totalChannelData / fftparam.numchannels;
            }
            //Debug.Log("working with: " + fftparam.numchannels + " channels here baby!");
        }
    }

    private void FrequencyBands()
    {
        int counter = 0;
        for (int i = 0; i < freqRanges.Count; i++)
        {
            float average = 0f;
            int numSampleInThisBand = numSampleInFirstBand * (int)Mathf.Pow(2, i);

            for (int j = 0; j < numSampleInThisBand; j++)
            {
                average += _samples[counter] * (counter + 1);
                counter++;
            }
            average /= counter;
            freqBands[i] = average * meterIntensity;
        }
    }
    
    private void BandBuffer()
    {
        for (int i = 0; i < freqRanges.Count; i++)
        {
            if(freqBands[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBands[i];
                bufferDecrease[i] = bufferStartSpeed;
            }

            if(freqBands[i] < bandBuffer[i])
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= bufferAccelRate;
            }

            if (bandBuffer[i] < 0)
                bandBuffer[i] = 0f;
        }
    }

    private void countFPS()
    {
        time += Time.deltaTime;
        if (time > 1f)
        {
            time = 0f;
            fps = 0;
            fps += frameCount;
            frameCount = 0;
        }
        frameCount++;
    }

    public void PlayFMODEvent()
    {
        _event.start();
    }

    public void StopFMODEvent()
    {
        _event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void PauseFMODEvent()
    {
        bool p = false;
        _event.getPaused(out p);
        p = !p;
        _event.setPaused(p);
    }

    private void OnDestroy()
    {
        _event.release();
    }
}