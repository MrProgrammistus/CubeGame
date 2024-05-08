#version 420 core

layout (points) in;
layout (triangle_strip, max_vertices = 4) out;

in A{
    vec3 fragPos;
    vec4 color;
    flat int aData;
    float yx;
	float yz;
	float yxz;
} a_in[];

out vec3 fragPos;
out vec2 texCoord;
out vec3 normal;
out vec4 color;
out flat int aData;

uniform mat4 view;

void main() {
    
    fragPos = a_in[0].fragPos;
    color = a_in[0].color;
    aData = a_in[0].aData;
    
    gl_Position = gl_in[0].gl_Position * view;
    EmitVertex();

    fragPos = vec3(a_in[0].fragPos.x, a_in[0].yz, a_in[0].fragPos.z + 1);
    gl_Position = vec4(gl_in[0].gl_Position.x, a_in[0].yz, gl_in[0].gl_Position.z + 1, 1) * view;
    EmitVertex();

    fragPos = vec3(a_in[0].fragPos.x + 1, a_in[0].yx, a_in[0].fragPos.z);
    gl_Position = vec4(gl_in[0].gl_Position.x + 1, a_in[0].yx, gl_in[0].gl_Position.z, 1) * view;
    EmitVertex();

    fragPos = vec3(a_in[0].fragPos.x + 1, a_in[0].yxz, a_in[0].fragPos.z + 1);
    gl_Position = vec4(gl_in[0].gl_Position.x + 1, a_in[0].yxz, gl_in[0].gl_Position.z + 1, 1) * view;
    EmitVertex();

    EndPrimitive();
}  