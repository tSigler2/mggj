extends Node


@export var actionable_finder: Area2D
@export var target: Area2D

func _unhandled_input(event: InputEvent) -> void:
	if Input.is_action_just_pressed("ui_accept"):
		var actionables = actionable_finder.get_overlapping_bodies()
		print("overlapping: ", actionable_finder.overlaps_area(target))
		print("Overlapping areas: ", actionables.size())
		if actionables.size() > 0:
			actionables[0].action()
			return
		#DialogueManager.show_dialogue_balloon(load("res://Dialogue/untitled.dialogue"), "start")
func _ready() -> void:
	print("Finder layer:",actionable_finder.collision_layer," mask:",actionable_finder.collision_mask)
	print("Finder:", actionable_finder.global_position)
	print("finder monitorable: ", actionable_finder.monitorable," monitering: ", actionable_finder.monitoring)
	actionable_finder.body_entered.connect(_on_area_entered)

func _on_area_entered(area):
	print("Entered:", area.name)
