#version 400

float B(int n, float t)
{
    float sixth = 1.0f / 6.0f;
    float t1 = 1.0f - t;
    if (n == 0) return sixth * t1 * t1 * t1;
    if (n == 1) return sixth * (3.0f * t * t * t - 6.0f * t * t + 4.0f);
    if (n == 2) return sixth * (-3.0f * t * t * t + 3.0f * t * t + 3.0f * t + 1.0f);
    if (n == 3) return sixth * t * t * t;
    return 0.0f;
}

float N(int n, int i, float t)
{
    float tj, T = t * (n - 3);
    int b = -1;
    if ((i - 3.0f <= T) && (T < i - 2.0f)) b = 3;
    if ((i - 2.0f <= T) && (T < i - 1.0f)) b = 2;
    if ((i - 1.0f <= T) && (T < i)) b = 1;
    if ((i <= T) && (T < i + 1.0f)) b = 0;
    if (b == -1) return 0.0f;
    tj = T - i + b;
    return B(b, tj);
}

vec3 DeBoor(vec3 a, vec3 b, vec3 c, vec3 d, float t)
{
    vec3 r = vec3(0.0f, 0.0f, 0.0f);
    r += N(4, 0, t) * a;
    r += N(4, 1, t) * b;
    r += N(4, 2, t) * c;
    r += N(4, 3, t) * d;
    return r;
}


layout(isolines, equal_spacing) in;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main() {
   float div = (gl_TessLevelOuter[0])/(gl_TessLevelOuter[0]-1);
   float u = gl_TessCoord.x;
   float v = gl_TessCoord.y*div;
   vec3 v1 = DeBoor(gl_in[0].gl_Position.xyz, gl_in[1].gl_Position.xyz, gl_in[2].gl_Position.xyz, gl_in[3].gl_Position.xyz, v);
   vec3 v2 = DeBoor(gl_in[4].gl_Position.xyz, gl_in[5].gl_Position.xyz, gl_in[6].gl_Position.xyz, gl_in[7].gl_Position.xyz, v);
   vec3 v3 = DeBoor(gl_in[8].gl_Position.xyz, gl_in[9].gl_Position.xyz, gl_in[10].gl_Position.xyz, gl_in[11].gl_Position.xyz, v);
   vec3 v4 = DeBoor(gl_in[12].gl_Position.xyz, gl_in[13].gl_Position.xyz, gl_in[14].gl_Position.xyz, gl_in[15].gl_Position.xyz, v);
   vec3 pos = DeBoor(v1, v2, v3, v4, u);

   gl_Position = projection * view * model * vec4(pos, 1.0f);
}