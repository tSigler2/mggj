extends Area2D


@export var dialogue_resource: DialogueResource
@export var dialogue_start: String = "start"



func action() -> void:
	print("Action called on: ", self)
	print("Dialogue resource: ", dialogue_resource)
	DialogueManager.show_dialogue_balloon(dialogue_resource,dialogue_start)

func _ready() -> void:
	print("Actionable layer:", self.collision_layer," mask:", self.collision_mask)
	print("Actionable:", self.global_position)
	print("actionable monitorable: ", self.monitorable," monitering: ", self.monitoring)
