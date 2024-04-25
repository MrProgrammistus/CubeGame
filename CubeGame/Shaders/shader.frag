#version 460 core

layout(binding = 0) uniform sampler2D texture0;
layout(binding = 1) uniform sampler2D texture1;
layout(binding = 2) uniform sampler2D texture2;
layout(binding = 3) uniform sampler2D texture3;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;
in vec4 color;
in flat int tex;

out vec4 outClor;

void main(){
	outClor = color;
	if     (tex == 0) outClor *= texture(texture0, texCoord);
	else if(tex == 1) outClor *= texture(texture1, texCoord);
	else if(tex == 2) outClor *= texture(texture2, texCoord);
	else			  outClor *= texture(texture3, texCoord);

	vec2 bigTexCoord = vec2((int(abs(fragPos.x + 0.499)) + int(abs(fragPos.y + 0.499)) % 16) / 16.0,
							(int(abs(fragPos.z + 0.499)) + int(abs(fragPos.y + 0.499)) % 16) / 16.0);

	outClor = mix(texture(texture0, bigTexCoord) * outClor, outClor, 0.5);
}