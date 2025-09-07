extends Area2D

@export var dialogue_resource: DialogueResource
@export var dialogue_start: String = "start"

var _is_player_in_range: bool = false

func action() -> void:
	print("Action called on: ", self)
	print("Dialogue resource: ", dialogue_resource)
	DialogueManager.show_dialogue_balloon(dialogue_resource, dialogue_start)

func _ready() -> void:
	print("Actionable layer:", collision_layer, " mask:", collision_mask)
	print("Actionable:", global_position)
	print("actionable monitorable: ", monitorable, " monitoring: ", monitoring)

	# Connect Area2D signals
	body_entered.connect(Callable(self, "_on_body_entered"))
	body_exited.connect(Callable(self, "_on_body_exited"))

func _on_body_entered(body: Node) -> void:
	if body.is_in_group("Player"):
		_is_player_in_range = true
		print("Player entered interaction range")

func _on_body_exited(body: Node) -> void:
	if body.is_in_group("Player"):
		_is_player_in_range = false
		print("Player exited interaction range")
