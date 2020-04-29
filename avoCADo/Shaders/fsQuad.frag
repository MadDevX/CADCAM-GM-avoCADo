#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D bufferTexture1;
uniform sampler2D bufferTexture2;

void main()
{
    vec4 colorA = texture(bufferTexture1, TexCoords);
	vec4 colorB = texture(bufferTexture2, TexCoords);
	FragColor = vec4(TexCoords.x, TexCoords.y, 0.0f, 1.0f); //colorA + colorB;
}