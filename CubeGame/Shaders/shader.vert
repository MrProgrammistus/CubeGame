#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aData;

out vec3 fragPos;
out vec2 texCoord;
out vec3 normal;
out vec4 color;
out flat int tex;

uniform mat4 view;
uniform float time;

void main(){
	fragPos = aPos;
	color = vec4(1);

	tex = 0;

	if	   ((int(aData) & (255 * 256)) == 2 * 256)  color = vec4(vec3(0.7, 0.7, 0.7) * 0.6, 1.0) + vec4(vec3((fragPos.y)/64), 0);
	else if((int(aData) & (255 * 256)) == 3 * 256)  color = vec4(vec3(0.2, 0.6, 0.0) * 0.6, 1.0) + vec4(vec3((fragPos.y)/32), 0);
	else if((int(aData) & (255 * 256)) == 4 * 256)  color = vec4(vec3(1.0, 0.9, 0.3) * 0.6, 1.0) + vec4(vec3((fragPos.y)/64), 0);
	else if((int(aData) & (255 * 256)) == 5 * 256)  color = vec4(vec3(0.1, 0.1, 1.0) * 1.0, 0.5);

	else if((int(aData) & (255 * 256)) == 6 * 256)  color = vec4(vec3(0.9, 0.9, 0.1) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 7 * 256)  tex = 3;
	else if((int(aData) & (255 * 256)) == 8 * 256)  tex = 2;

	else if((int(aData) & (255 * 256)) == 9 * 256)  color = vec4(vec3(0.9, 0.9, 0.9) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 10 * 256) color = vec4(vec3(0.5, 0.5, 0.5) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 11 * 256) color = vec4(vec3(0.0, 0.0, 0.0) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 12 * 256) color = vec4(vec3(0.5, 0.0, 0.0) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 13 * 256) color = vec4(vec3(0.8, 0.8, 0.0) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 14 * 256) color = vec4(vec3(0.0, 0.8, 0.0) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 15 * 256) color = vec4(vec3(0.0, 0.8, 0.8) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 16 * 256) color = vec4(vec3(0.0, 0.0, 0.7) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 17 * 256) color = vec4(vec3(0.4, 0.0, 0.6) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 18 * 256) color = vec4(vec3(0.8, 0.5, 0.0) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 19 * 256) color = vec4(vec3(0.4, 0.2, 0.0) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 20 * 256) color = vec4(vec3(0.9, 0.4, 0.4) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 21 * 256) color = vec4(vec3(0.4, 0.9, 0.4) * 1.0, 1.0);
	else if((int(aData) & (255 * 256)) == 22 * 256) color = vec4(vec3(0.4, 0.4, 0.9) * 1.0, 1.0);

	else if((int(aData) & (255 * 256)) == 23 * 256){
		if(int(time) == 0 || int(time) == 1) color = vec4(vec3(0.5, 0.0, 0.0) * 1.0, 1.0);
		if(int(time) == 2 || int(time) == 3) color = vec4(vec3(0.0, 0.8, 0.0) * 1.0, 1.0);
		if(int(time) == 4 || int(time) == 5) color = vec4(vec3(0.0, 0.0, 0.7) * 1.0, 1.0);
	}

	if((int(aData) & (255 * 256)) >= 9 * 256 && (int(aData) & (255 * 256)) <= 23 * 256) tex = 1;

	texCoord.x = int((int(aData) & 128) == 128);
	texCoord.y = int((int(aData) & 64) == 64);

	normal.x = int((int(aData) & 16) == 16) - int((int(aData) & 32) == 32);
	normal.y = int((int(aData) & 4)  == 4)  - int((int(aData) & 8)  == 8);
	normal.z = int((int(aData) & 1)  == 1)  - int((int(aData) & 2)  == 2);

	gl_Position = vec4(fragPos, 1) * view;
}