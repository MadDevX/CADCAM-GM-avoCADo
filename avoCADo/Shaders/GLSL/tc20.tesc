#version 400 core

layout (vertices = 20) out; // 20 points per patch

uniform int tessLevelOuter0;
uniform int tessLevelOuter1;

void main() 
{
   gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
   if(gl_InvocationID == 0) // levels only need to be set once per patch
   { 
       gl_TessLevelOuter[0] = tessLevelOuter0;
       gl_TessLevelOuter[1] = tessLevelOuter1;
   }
}