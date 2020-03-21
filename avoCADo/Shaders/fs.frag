#version 330 core
out vec4 FragColor;

uniform vec4 color = vec4(1.0f, 1.0f, 1.0f, 1.0f);

void main()
{
	FragColor = color;
}