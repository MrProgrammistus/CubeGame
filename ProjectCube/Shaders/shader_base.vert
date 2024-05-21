#version 420 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aData;

uniform mat4 view;

out vec4 color;
out flat int data;
out vec3 pos;

void main(){
	
	color = vec4(1, 1, 1, 1);

	if(int(aData) == 1) color = vec4(1, 0, 0, 1);
	if(int(aData) == 2) color = vec4(0, 1, 0, 1);
	if(int(aData) == 3) color = vec4(0, 0, 1, 1);
	if(int(aData) == 4) color = vec4(0, 1, 1, 1);
	if(int(aData) == 5) color = vec4(1, 0, 1, 1);
	if(int(aData) == 6) color = vec4(1, 1, 0, 1);
	if(int(aData) == 7) color = vec4(1, 1, 1, 1);

	data = int(aData);
	pos = aPos;

	gl_Position = vec4(aPos, 1) * view;
}