#version 420 core

layout(binding = 0) uniform sampler2D texture0;

in vec4 color;
in flat int data;
in vec3 pos;

out vec4 outClor;

void main(){
	outClor = color;
	
	if(data == 0){
		outClor.rgb *= (length(pos) - 300) / 50;
	}
}