#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoords;

uniform sampler2D samp;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform float worldWidth = 0.18f;
uniform float worldHeight = 0.18f;

out vec2 TexCoords;
out vec3 WorldPos;
out vec3 Normal;

void main()
{    
	ivec2 size = textureSize(samp, 0);
	
	ivec2 pixelCoords = ivec2(aTexCoords * size);
    float cur = texelFetch(samp, pixelCoords, 0).r;
    float dx1 = texelFetch(samp, pixelCoords - ivec2(1,0), 0).r;
    float dz1 = texelFetch(samp, pixelCoords - ivec2(0,1), 0).r;
    float dx2 = texelFetch(samp, pixelCoords + ivec2(1,0), 0).r;
    float dz2 = texelFetch(samp, pixelCoords + ivec2(0,1), 0).r;

	float diffX = dx2 - dx1;
	float diffZ = dz2 - dz1;

	float deltaX = (2.0f * worldWidth)  / float(size.x); //TODO: maybe scaling factor
	float deltaZ = (2.0f * worldHeight) / float(size.y);
	
	vec3 n1 = vec3(deltaX, diffX, 0.0f);
	vec3 n2 = vec3(0.0f, diffZ, deltaZ);
	Normal = normalize(cross(n2, n1));

	vec4 texel = textureLod(samp, aTexCoords, 0.0f);
	vec3 offsetPos = aPosition + vec3(0.0f, texel.r, 0.0f);
	WorldPos = (model * vec4(offsetPos, 1.0f)).xyz;
	TexCoords = aTexCoords;
	gl_Position = projection * view * model * vec4(offsetPos, 1.0f);
}