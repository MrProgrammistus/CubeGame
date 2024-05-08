#version 420 core

layout(binding = 0) uniform sampler2D texture0;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;
in vec4 color;
in flat int aData;

out vec4 outClor;

uniform vec3 playerPos;

void main(){
	outClor = color;
	//outClor *= texture(texture0, texCoord);
	outClor.rgb *= (fragPos.y / 128 + 1) / 2 - 0.5;

	if(int(fragPos.x) == 0) outClor.r += 0.1;
	//if(int(fragPos.y) == 64) outClor.g += 0.1;
	if(int(fragPos.z) == 0) outClor.b += 0.1;

	if((int(aData) & 255 * 256) == 2 * 256){
		vec3 deltaPos = abs(playerPos - fragPos);

		float dxy = sqrt(deltaPos.x * deltaPos.x + deltaPos.z * deltaPos.z);

		float tg = deltaPos.y / dxy;

		outClor.a = 1 / tg / 5;

		//outClor = mix(outClor, vec4(vec3(0.5, 0.7, 0.9) * 0.1, 1), (64 - fragPos.y) / 20);
	}
}