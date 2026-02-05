shader_type canvas_item;

// Настройка количества "пикселей" по горизонтали и вертикали
uniform vec2 amount = vec2(48.0, 48.0);

void fragment() {
	// Получаем координаты текстуры (от 0 до 1)
	vec2 grid_uv = round(UV * amount) / amount;
	
	// Читаем цвет из текстуры по новым "грубым" координатам
	vec4 pixel_color = texture(TEXTURE, grid_uv);
	
	COLOR = pixel_color;
}