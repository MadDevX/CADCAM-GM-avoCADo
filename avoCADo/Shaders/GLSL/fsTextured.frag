#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
uniform sampler2D trimTexture;
uniform vec4 color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
uniform vec4 bgColor = vec4(0.157f, 0.157f, 0.157f, 1.0f);
uniform vec4 filterColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);

float near = 0.01f; //if changed, also change in Camera.cs
float far = 100.0f; //if changed, also change in Camera.cs
float fogDistance = 90.0f;
float LinearizeDepth(float depth);

void main()
{
    float depth = min(LinearizeDepth(gl_FragCoord.z)/fogDistance, 1.0f);
    vec4 texCol = texture(trimTexture, TexCoords);
    vec4 finalColor = color;
    if(texCol.g < 0.5f) finalColor = vec4(0.0f, 0.0f, 0.0f, 0.0f);
	FragColor = vec4(mix(finalColor.rgb * filterColor.rgb, bgColor.rgb * filterColor.rgb, depth), finalColor.a);
}


float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}