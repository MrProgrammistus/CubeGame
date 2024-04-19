#version 460 core

layout(binding = 0) uniform sampler2D texture0;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;
in vec4 color;

out vec4 outClor;

void main(){
	outClor = color;
	outClor *= texture(texture0, texCoord);

	vec2 bigTexCoord = vec2((int(abs(fragPos.x + 0.499)) + int(abs(fragPos.y + 0.499)) % 16) / 16.0,
							(int(abs(fragPos.z + 0.499)) + int(abs(fragPos.y + 0.499)) % 16) / 16.0);

	outClor = mix(texture(texture0, bigTexCoord) * outClor, outClor, 0.5);
}