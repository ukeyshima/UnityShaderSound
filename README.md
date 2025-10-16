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
1. Attach ShaderSound class and AudioSource to GameObject
<img width="404" height="487" alt="スクリーンショット 2025-10-16 9 24 23" src="https://github.com/user-attachments/assets/9b49ff1f-4de0-4059-8f29-be1019f9ff5a" />

※ Note: An AudioListener is required for any component.

2. Write Compute Shader
```
#include "Packages/com.ukeyshima.unityshadersound/Runtime/Shaders/Include/ShaderSound.hlsl"

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

