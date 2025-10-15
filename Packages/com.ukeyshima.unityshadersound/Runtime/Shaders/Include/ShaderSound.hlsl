#ifndef INCLUDED_SHADER_SOUND
#define INCLUDED_SHADER_SOUND

RWStructuredBuffer<float> _Result;
float _StartTime;
float _SampleRate;
int _SampleDataCount;

#define EARLY_RETURN(ID) if((int)id.x >= _SampleDataCount) return; 
#define TIME(ID) (_StartTime + (float)id.x / _SampleRate)
#define UPDATE_DATA(ID, RESULT) _Result[ID] = clamp(RESULT, -1.0f, 1.0f);

#endif