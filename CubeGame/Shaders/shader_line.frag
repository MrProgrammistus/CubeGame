#version 460 core

layout(binding = 0) uniform sampler2D texture0;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;

out vec4 outClor;

void main(){
	outClor = vec4(vec3(0), 1);
}