using UnityEngine;
using System.Runtime.InteropServices;

public class GetFMODSpectrumData : MonoBehaviour
{
    [FMODUnity.EventRef] public string _eventPath = null;
    public int _windowSize = 512;
    public FMOD.DSP_FFT_WINDOW _windowShape = FMOD.DSP_FFT_WINDOW.RECT;

    private FMOD.Studio.EventInstance _event;
    private FMOD.ChannelGroup _channelGroup;
    private FMOD.DSP _dsp;
    private FMOD.DSP_PARAMETER_FFT _fftparam;

    
    public float[] _samples;

    private void Start()
    {
        //Prepare FMOD event
        PrepareFMODeventInstance();

        _samples = new float[_windowSize];
    }

    private void PrepareFMODeventInstance()
    {
        _event = FMODUnity.RuntimeManager.CreateInstance(_eventPath);
        _event.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        _event.start();

        FMODUnity.RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out _dsp);
        _dsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWTYPE, (int)_windowShape);
        _dsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, _windowSize * 2);

        _event.getChannelGroup(out _channelGroup);
        _channelGroup.addDSP(0, _dsp);
    }

    private void Update()
    {
        GetSpectrumData();
    }

    private void GetSpectrumData()
    {
        System.IntPtr _data;
        uint _length;

        _dsp.getParameterData(2, out _data, out _length);
        _fftparam = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(_data, typeof(FMOD.DSP_PARAMETER_FFT));
        

        if (_fftparam.numchannels == 0)
        {
            _event.getChannelGroup(out _channelGroup);
            _channelGroup.addDSP(0, _dsp);
            //Debug.Log("wait I'm not ready yet!");
        }
        else if (_fftparam.numchannels >= 1)
        {
            for (int s = 0; s < _windowSize; s++)
            {
                float _totalChannelData = 0f;
                for (int c = 0; c < _fftparam.numchannels; c++)
                    _totalChannelData += _fftparam.spectrum[c][s];
                _samples[s] = _totalChannelData / _fftparam.numchannels;
            }
            //Debug.Log("working with: " + fftparam.numchannels + " channels here baby!");
        }
    }

    public void StopFMODEvent()
    {
        _event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}