#version 330 core
layout (location = 0) in vec3 aPosition;

uniform mat4 modelMatrix;
uniform mat4 view;
uniform mat4 projection;

void main()
{
	gl_Position = projection * view * modelMatrix * vec4(aPosition, 1.0f);
}