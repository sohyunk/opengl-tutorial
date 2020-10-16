#version 330 core
in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

struct Light {
    vec3 position;
	vec3  direction;
	float cutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

	float constant;
    float linear;
    float quadratic;
};

struct Material {
    sampler2D diffuse;
    sampler2D specular;
    float     shininess;
};

uniform vec3 viewPos;
uniform Material material;
uniform Light light;

out vec4 FragColor;

void main()
{	
	vec3 lightDir = normalize(light.position - FragPos);

	float theta = dot(lightDir, normalize(-light.direction));

	if(theta > light.cutOff) 
	{       
	  // Lighting 계산 수행
	  	// ambient
		vec3 ambient = light.ambient * vec3(texture(material.diffuse, TexCoords));
	
	// diffuse
	vec3 norm = normalize(Normal);
	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));

	// sepcular
	vec3 viewDir = normalize(viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = light.specular * spec * vec3(texture(material.specular, TexCoords));
	
	// attenuation
	float distance    = length(light.position - FragPos);
	float attenuation = 1.0 / (light.constant + light.linear * distance + 
						light.quadratic * (distance * distance));

	// ambient  *= attenuation; 
	diffuse  *= attenuation;
	specular *= attenuation;  

	FragColor = vec4(ambient + diffuse + specular, 1.0);
	}
	else  // 아니면, ambient light를 사용하므로 spotlight 외부에 있더라도 완전히 어둡지 않음
	  FragColor = vec4(light.ambient * vec3(texture(material.diffuse, TexCoords)), 1.0);
}