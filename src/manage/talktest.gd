extends BaseDialogueTestScene


func _unhandled_input(event: InputEvent) -> void:
	if Input.is_action_just_pressed("ui_accept"):
		DialogueManager.show_example_dialogue_balloon(load("res://Dialogue/untitled.dialogue"), "start")
		return
