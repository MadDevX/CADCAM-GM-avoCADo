#version 400

vec3 bezier2(vec3 a, vec3 b, float t) 
{
   return mix(a, b, t);
}

vec3 bezier3(vec3 a, vec3 b, vec3 c, float t) 
{
   return mix(bezier2(a, b, t), bezier2(b, c, t), t);
}

vec3 bezier4(vec3 a, vec3 b, vec3 c, vec3 d, float t) 
{
   return mix(bezier3(a, b, c, t), bezier3(b, c, d, t), t);
}

layout(isolines, equal_spacing) in;

out vec2 TexCoords;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {
   float div = (gl_TessLevelOuter[0])/(gl_TessLevelOuter[0]-1);
   float u = gl_TessCoord.x;
   float v = gl_TessCoord.y*div;
   vec3 v1 = bezier4(gl_in[0].gl_Position.xyz, gl_in[1].gl_Position.xyz, gl_in[2].gl_Position.xyz, gl_in[3].gl_Position.xyz, v);
   vec3 v2 = bezier4(gl_in[4].gl_Position.xyz, gl_in[5].gl_Position.xyz, gl_in[6].gl_Position.xyz, gl_in[7].gl_Position.xyz, v);
   vec3 v3 = bezier4(gl_in[8].gl_Position.xyz, gl_in[9].gl_Position.xyz, gl_in[10].gl_Position.xyz, gl_in[11].gl_Position.xyz, v);
   vec3 v4 = bezier4(gl_in[12].gl_Position.xyz, gl_in[13].gl_Position.xyz, gl_in[14].gl_Position.xyz, gl_in[15].gl_Position.xyz, v);
   vec3 pos = bezier4(v1, v2, v3, v4, u);

   TexCoords = vec2(u, v);

   gl_Position = projection * view * model * vec4(pos, 1.0f);
}