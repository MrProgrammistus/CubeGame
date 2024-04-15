#version 460 core

layout(binding = 0) uniform sampler2D texture0;

in vec3 fragPos;
in vec2 texCoord;
in vec3 normal;
in vec4 color;

out vec4 outClor;

void main(){
	outClor = color;
	outClor *= texture(texture0, texCoord);
}