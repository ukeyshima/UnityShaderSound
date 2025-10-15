# UnityShaderSound

Shader Sound for Unity

## Installation
Window > Package Manager > Add package from git URL

Enter the following url and click the add button.

```
https://github.com/ukeyshima/UnityShaderSound.git?path=/Packages/com.ukeyshima.unityshadersound
```

If want to see sample scene, please download from Samples.

## Usage
1. Attach ShaderSound class to GameObject
<img width="410" height="91" alt="スクリーンショット 2025-10-16 1 37 08" src="https://github.com/user-attachments/assets/8b123f3d-fc14-46a6-9610-383a1283c8ea" />

2. Write Compute Shader
```
[numthreads(8, 1, 1)]
void Main (uint3 id : SV_DispatchThreadID)
{
    EARLY_RETURN(id.x)
    float time = TIME(id.x);
    float beat = TIME2BEAT(time);
    float dest = kick(beat);
    UPDATE_DATA(id.x, dest)
}
```

