#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

uniform sampler2D samp;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 TexCoords;

void main()
{
	vec4 texel = textureLod(samp, aTexCoords, 0.0f);
	vec3 offsetPos = aPosition + vec3(0.0f, texel.r, 0.0f);
	TexCoords = aTexCoords;
	gl_Position = projection * view * model * vec4(offsetPos, 1.0f);
}