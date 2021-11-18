shader_type spatial;
uniform sampler2D sdf_texture;
void fragment(){
	float a = texture(sdf_texture,UV).r;
	ALPHA_SCISSOR = 0.5;
	ALPHA = round(a);
}