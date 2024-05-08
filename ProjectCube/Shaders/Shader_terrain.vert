#version 420 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in float aYx;
layout (location = 2) in float aYz;
layout (location = 3) in float aYxz;
layout (location = 4) in float aData;

out A{
	vec3 fragPos;
	vec4 color;
	flat int aData;
	float yx;
	float yz;
	float yxz;
} a_out;

void main(){
	a_out.fragPos = aPos;
	a_out.color = vec4(1);

	//if((int(aData) & 255 * 256) == 2 * 256) a_out.color = vec4(1, 1, 1, 0.8);
	if((int(aData) & 255 * 256) == 2 * 256) a_out.color = vec4(vec3(0.5, 0.7, 0.9), 0.1);

	a_out.yx = aYx;
	a_out.yz = aYz;
	a_out.yxz = aYxz;
	
	a_out.aData = int(aData);

	gl_Position = vec4(aPos, 1);
}