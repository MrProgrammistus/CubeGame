#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aData;

out vec4 color;

uniform mat4 view;

void main(){
	if(aData == 0) color = vec4(vec3(0.0,0.0,0.0),1);
	if(aData == 1) color = vec4(vec3(0.5,0.5,0.5),1);
	if(aData == 2) color = vec4(vec3(1.0,0.0,0.0),1);
	if(aData == 3) color = vec4(vec3(0.0,1.0,0.0),1);
	if(aData == 4) color = vec4(vec3(0.0,0.0,1.0),1);

	gl_Position = vec4(aPos, 1) * view;
}