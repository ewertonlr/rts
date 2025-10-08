shader_type canvas_item;

uniform float outline_size : hint_range(0.0, 5.0, 0.1) = 1.0;
uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 0.0, 1.0);

uniform sampler2D albedo_texture;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec4 current_color = texture(albedo_texture, UV);
	
	vec4 final_color = current_color;

	if (current_color.a == 0.0) {
		for (float x = -outline_size; x <= outline_size; x += 1.0) {
			for (float y = -outline_size; y <= outline_size; y += 1.0) {
				vec2 neighbor_uv = UV + vec2(x, y) * TEXTURE_PIXEL_SIZE;
				vec4 neighbor_color = texture(albedo_texture, neighbor_uv);
				
				if (neighbor_color.a > 0.0) {
					final_color = outline_color;
					x = outline_size + 1.0;
					break;
				}
			}
		}
	}
	COLOR = vec4(0.0, 0.0, 0.0, 0.0);
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
