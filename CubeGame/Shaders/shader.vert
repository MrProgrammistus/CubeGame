#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aData;

out vec3 fragPos;
out vec2 texCoord;
out vec3 normal;
out vec4 color;

uniform mat4 view;

void main(){
	fragPos = aPos;
	color = vec4(1);


	if((int(aData) & (255 * 256)) == 2 * 256) color = vec4(vec3(0.7, 0.7, 0.7) * 0.6, 1.0) + vec4(vec3((fragPos.y)/64), 0);
	if((int(aData) & (255 * 256)) == 3 * 256) color = vec4(vec3(0.2, 0.6, 0.0) * 0.6, 1.0) + vec4(vec3((fragPos.y)/32), 0);
	if((int(aData) & (255 * 256)) == 4 * 256) color = vec4(vec3(1.0, 0.9, 0.3) * 0.6, 1.0) + vec4(vec3((fragPos.y)/64), 0);
	if((int(aData) & (255 * 256)) == 5 * 256) color = vec4(vec3(0.1, 0.1, 1.0) * 1.0, 0.5);

	texCoord.x = int((int(aData) & 128) == 128);
	texCoord.y = int((int(aData) & 64) == 64);

	normal.x = int((int(aData) & 16) == 16) - int((int(aData) & 32) == 32);
	normal.y = int((int(aData) & 4)  == 4)  - int((int(aData) & 8)  == 8);
	normal.z = int((int(aData) & 1)  == 1)  - int((int(aData) & 2)  == 2);
	
	gl_Position = vec4(fragPos, 1) * view;
}