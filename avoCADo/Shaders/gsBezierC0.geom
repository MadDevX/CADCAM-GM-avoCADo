#version 330 core
layout(lines_adjacency) in;
layout(line_strip, max_vertices = 256) out;


vec4 bezier2(vec4 a, vec4 b, float t)
{
    return mix(a, b, t);
}
vec4 bezier3(vec4 a, vec4 b, vec4 c, float t)
{
    return mix(bezier2(a,b,t), bezier2(b,c,t), t);
}
vec4 bezier4(vec4 a, vec4 b, vec4 c, vec4 d, float t)
{
    return mix(bezier3(a, b, c, t), bezier3(b, c, d, t), t);
}

void main(void)
{

    vec4 knots[4];
    knots[0] = gl_in[0].gl_Position;
    knots[1] = gl_in[1].gl_Position;
    knots[2] = gl_in[2].gl_Position;
    knots[3] = gl_in[3].gl_Position;

    if(isnan(knots[2].x)) 
    {
     gl_Position = knots[0];
     EmitVertex();
     gl_Position = knots[1];
     EmitVertex();
    } 
    else if(isnan(knots[3].x)) //TODO: check if it works in vertex shader (nan coords should be mapped to nan coords in screenspace)
    {
        float mag = distance(knots[0].xy, knots[1].xy) + 
                    distance(knots[1].xy, knots[2].xy);
        int divisions = min(int((mag+1) * 50), 256);
        float delta = 1.0f / float(divisions);
        for (int i = 0; i <= divisions; i++){
            gl_Position = bezier3(knots[0], knots[1], knots[2], float(i) * delta);
            EmitVertex();
         }
    } 
    else 
    {
        float mag = distance(knots[0].xy, knots[1].xy) + 
                    distance(knots[1].xy, knots[2].xy) + 
                    distance(knots[2].xy, knots[3].xy);
        int divisions = min(int((mag+1) * 50), 255);
        float delta = 1.0f / float(divisions);
        for (int i = 0; i <= divisions; i++)
        {
            gl_Position = bezier4(knots[0], knots[1], knots[2], knots[3], float(i) * delta);
            EmitVertex();
        }
    }  
    EndPrimitive();
}
