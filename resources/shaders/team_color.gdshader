shader_type canvas_item; // Define que é um shader 2D (CanvasItem)

// A cor que o shader irá PROCURAR no sprite (Target Color: RGB 99, 155, 255)
// 99/255 ≈ 0.3882
// 155/255 ≈ 0.6078
// 255/255 = 1.0
uniform vec4 target_color : source_color = vec4(0.3882, 0.6078, 1.0, 1.0);

// A cor para a qual o shader irá CONVERTER a cor alvo (Replacement Color: Vermelho do Time 2)
// Exemplo: Vermelho Vivo (R=1.0, G=0.2, B=0.2). Ajuste no Editor/Código conforme seu vermelho desejado.
uniform vec4 replacement_color : source_color = vec4(1.0, 0.2, 0.2, 1.0);

// Margem de tolerância. Define o quão "próxima" do target_color a cor do pixel deve estar.
// 0.1 é um bom valor de partida, mas ajuste (ex: 0.15 ou 0.2) se tiver sombras/brilhos no azul.
uniform float color_tolerance : hint_range(0.0, 1.0) = 0.1;

void fragment() {
    // Pega a cor original do pixel na textura (base do Sprite2D)
    vec4 original_color = texture(TEXTURE, UV);

    // 1. Calcula a distância (diferença) entre a cor do pixel e a cor alvo (o azul)
    float distance_to_target = distance(original_color.rgb, target_color.rgb);

    // 2. Checa se o pixel está dentro da margem de tolerância
    if (distance_to_target < color_tolerance) {

        // 3. Calcula a intensidade da substituição
        // Cores mais próximas do target_color (menor distância) terão um mix_amount maior.
        float mix_amount = 1.5 - (distance_to_target / color_tolerance);

        // 4. Interpola linearmente a cor original com a cor de substituição (vermelho).
        vec3 final_rgb = mix(original_color.rgb, replacement_color.rgb, mix_amount);

        // 5. Atribui a cor final, mantendo o canal alpha (opacidade) original.
        COLOR = vec4(final_rgb, original_color.a);
		//COLOR = vec4(1.0, 1.0, 1.0, 1.0);
		//COLOR = vec4(0.0, 0.2, 0.2, 1.0);
    } else {
        // Se a cor do pixel não for próxima do azul alvo, mantém a cor original.
        COLOR = original_color;
    }
}