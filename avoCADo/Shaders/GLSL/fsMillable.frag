#version 330 core
in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

out vec4 FragColor;

uniform sampler2D colorTexture;

uniform vec4 color = vec4(1.0f, 1.0f, 1.0f, 1.0f);
uniform vec4 bgColor = vec4(0.157f, 0.157f, 0.157f, 1.0f);
uniform vec4 filterColor = vec4(1.0f, 1.0f, 1.0f, 1.0f);

uniform vec3 lightPos = vec3(0.0f, 1.0f, 0.0f);
uniform vec3 lightCol = vec3(1.0f, 1.0f, 1.0f);

uniform vec3 cameraPos;

float near = 0.01f; //if changed, also change in Camera.cs
float far = 100.0f; //if changed, also change in Camera.cs
float fogDistance = 90.0f;
float LinearizeDepth(float depth);

void main()
{
    //ambient
    float ambientStrength = 0.1f;
    vec3 ambient = ambientStrength * lightCol;

    //diffuse
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - WorldPos);
    float diff = max(dot(norm, lightDir), 0.0f);
    vec3 diffuse = diff * lightCol;

    //specular
    float specularStrength = 0.5f;
    vec3 viewDir = normalize(cameraPos - WorldPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0f), 8);
    vec3 specular = specularStrength * spec * lightCol;

    vec3 resultColor = (ambient + diffuse + specular) * color.rgb; //TODO: multiply by texture color

    float depth = min(LinearizeDepth(gl_FragCoord.z)/fogDistance, 1.0f);
	FragColor = vec4(mix(resultColor * filterColor.rgb * vec3(TexCoords, 0.0f), bgColor.rgb * filterColor.rgb, depth), color.a);
}


float LinearizeDepth(float depth)
{
    float z = depth * 2.0 - 1.0; // back to NDC 
    return (2.0 * near * far) / (far + near - z * (far - near));	
}