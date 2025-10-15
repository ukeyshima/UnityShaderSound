using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityShaderSound
{
    public class ShaderSound : MonoBehaviour
    {
        [SerializeField] private ComputeShader _computeShader;
        [SerializeField] private float _sampleInterval = 1f;

        private int _sampleRate = 48000;
        private int _sampleDataCount;
        private ComputeBuffer _sampleDataBuffer;
        private float[] _readSampleData;
        private float[] _writeSampleData;
        private int _sampleCount;
        private int _sampleNum;

        private void Start()
        {
            _sampleRate = AudioSettings.outputSampleRate;
            _sampleDataCount = (int)(_sampleInterval * (float)_sampleRate);
            _sampleDataBuffer = new ComputeBuffer(_sampleDataCount, sizeof(float));
            _readSampleData = new float[_sampleDataCount];
            _writeSampleData = new float[_sampleDataCount];
            _sampleCount = 0;
            _sampleNum = 0;
            SampleDispatch(0f);
            SampleDataSync();
            (_readSampleData, _writeSampleData) = (_writeSampleData, _readSampleData);
        }

        private void Update()
        {
            if (Mathf.CeilToInt(_sampleCount / (float)_sampleDataCount) != _sampleNum)
            {
                SampleDispatch(Mathf.CeilToInt(_sampleCount / (float)_sampleDataCount) * _sampleInterval);
                SampleDataAsync();
                _sampleNum++;
            }
        }

        private void OnDestroy()
        {
            _sampleDataBuffer.Release();
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                for (int c = 0; c < channels; c++)
                {
                    data[i + c] = _readSampleData[_sampleCount % _sampleDataCount];
                }

                _sampleCount++;
                if (_sampleCount % _sampleDataCount == 0)
                {
                    (_readSampleData, _writeSampleData) = (_writeSampleData, _readSampleData);
                }
            }
        }

        private void SampleDispatch(float startTime)
        {
            int kernel = 0;
            uint threadNumX, threadNumY, threadNumZ;
            _computeShader.GetKernelThreadGroupSizes(kernel, out threadNumX, out threadNumY, out threadNumZ);
            _computeShader.SetFloat("_SampleDataCount", _sampleDataCount);
            _computeShader.SetInt("_SampleRate", _sampleRate);
            _computeShader.SetFloat("_StartTime", startTime);
            _computeShader.SetBuffer(kernel, "_Result", _sampleDataBuffer);
            _computeShader.Dispatch(kernel, Mathf.CeilToInt(_sampleDataCount / (float)threadNumX), 1, 1);
        }

        private void SampleDataSync()
        {
            _sampleDataBuffer.GetData(_writeSampleData);
        }
        
        private async void SampleDataAsync()
        {
            TaskCompletionSource<AsyncGPUReadbackRequest> readBack = new TaskCompletionSource<AsyncGPUReadbackRequest>();
            AsyncGPUReadback.Request(_sampleDataBuffer, request =>
            {
                if (request.hasError) readBack.SetException(new System.Exception("GPU readback failed"));
                else readBack.SetResult(request);
            });
            AsyncGPUReadbackRequest request = await readBack.Task;
            _writeSampleData = request.GetData<float>().ToArray();
        }
    }
}