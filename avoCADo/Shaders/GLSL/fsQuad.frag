#version 400 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2DMS bufferTexture1;
uniform sampler2DMS bufferTexture2;

void main()
{
    ivec2 size = textureSize(bufferTexture1);
    vec4 colorA = texelFetch(bufferTexture1, ivec2(TexCoords * size), gl_SampleID);
	vec4 colorB = texelFetch(bufferTexture2, ivec2(TexCoords * size), gl_SampleID);

	FragColor = clamp(colorA + colorB, vec4(0.0f, 0.0f, 0.0f, 0.0f), vec4(1.0f, 1.0f, 1.0f, 1.0f));//vec4(TexCoords.x, TexCoords.y, 0.0f, 1.0f); 
}