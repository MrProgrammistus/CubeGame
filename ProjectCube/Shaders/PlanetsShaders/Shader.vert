#version 420 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aData;

uniform mat4 view;

uniform mat4 transform[5];

out vec4 color;
out flat int data;
out vec3 pos;

out float radius;

void main(){
	
	color = vec4(1, 1, 1, 1);

	if((int(aData) & 63) == 1) color = vec4(1, 0, 0, 1);
	if((int(aData) & 63) == 2) color = vec4(0, 1, 0, 1);
	if((int(aData) & 63) == 3) color = vec4(0, 0, 1, 1);
	if((int(aData) & 63) == 4) color = vec4(0, 1, 1, 1);
	if((int(aData) & 63) == 5) color = vec4(1, 0, 1, 1);
	if((int(aData) & 63) == 6) color = vec4(1, 1, 0, 1);
	if((int(aData) & 63) == 7) color = vec4(1, 1, 1, 1);

	data = int(aData);
	pos = aPos;
	int planetID = (int(aData) & (63 * 64)) / 64;
	if(planetID == 1) radius = 300;
	if(planetID == 2) radius = 50;
	if(planetID == 3) radius = 20;
	gl_Position = vec4(aPos, 1) * transform[planetID] * view;
}