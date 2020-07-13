#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	TexCoords = clamp(aTexCoords, vec2(0.01f, 0.01f), vec2(0.99f, 0.99f));;
	gl_Position = projection * view * model * vec4(aPosition, 1.0f);
}